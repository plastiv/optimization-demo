/*
                 __________                                      
    _____   __ __\______   \_____  _______  ______  ____ _______ 
   /     \ |  |  \|     ___/\__  \ \_  __ \/  ___/_/ __ \\_  __ \
  |  Y Y  \|  |  /|    |     / __ \_|  | \/\___ \ \  ___/ |  | \/
  |__|_|  /|____/ |____|    (____  /|__|  /____  > \___  >|__|   
        \/                       \/            \/      \/        
  Copyright (C) 2007-2009 Ingo Berg

  Permission is hereby granted, free of charge, to any person obtaining a copy of this 
  software and associated documentation files (the "Software"), to deal in the Software
  without restriction, including without limitation the rights to use, copy, modify, 
  merge, publish, distribute, sublicense, and/or sell copies of the Software, and to 
  permit persons to whom the Software is furnished to do so, subject to the following conditions:

  The above copyright notice and this permission notice shall be included in all copies or 
  substantial portions of the Software.

  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT
  NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND 
  NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, 
  DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
*/
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Optimization.Parser
{
    //---------------------------------------------------------------------------
    class ParserException : System.Exception
    {
        public ParserException( string sExpr, 
                                string sMsg, 
                                int nPos, 
                                string sTok )
        {
            m_sExpr = sExpr;
            m_sTok  = sTok;
            m_nPos  = nPos;
            m_sMsg  = sMsg;
        }

        public string Expression
        {
            get
            {
                return m_sExpr;
            }
        }

        override public string Message
        {
            get
            {
                return m_sMsg;
            }
        }

        public string Token
        {
            get
            {
                return m_sTok;
            }
        }

        public int Position
        {
            get 
            {
                return m_nPos;
            }
        }

        private string m_sMsg;
        private string m_sExpr;
        private string m_sTok;
        private int m_nPos;
    }

    //---------------------------------------------------------------------------
    class ParserVariable
    {
      public unsafe ParserVariable()
      {
        m_pVar = mupCreateVar();
        *((double*)m_pVar.ToPointer()) = 0;
      }

      public unsafe ParserVariable(double val)
      {
        m_pVar = mupCreateVar();
        *((double*)m_pVar.ToPointer()) = val;
      }

     ~ParserVariable()
      {
        mupReleaseVar(m_pVar);
      }

      public unsafe double Value
      {
        get
        {
          return *((double*)m_pVar.ToPointer());
        }
        
        set
        {
          *((double*)m_pVar.ToPointer()) = value;
        }
      }

      public IntPtr Pointer
      {
        get
        {
          return m_pVar;
        }
      }
      
      private IntPtr m_pVar;
      
      #region DLL imports
      
      [DllImport("muparser.dll")]
      protected static extern IntPtr mupCreateVar();

      [DllImport("muparser.dll")]
      protected static extern IntPtr mupReleaseVar(IntPtr var);
      
      #endregion
    }

    //---------------------------------------------------------------------------
    class Parser 
    {
      private IntPtr m_parser = IntPtr.Zero;

      // Keep the delegate in order to prevent deletion
      private List<Delegate> m_binOprtDelegates = new List<Delegate>();

      // Keep the delegate in order to prevent deletion
      private List<Delegate> m_funDelegates = new List<Delegate>();

      // Buffer with all parser variables
      private Dictionary<string, ParserVariable> m_varBuf = new Dictionary<string, ParserVariable>();

      // Keep reference to the delegate of the error function
      private ErrorDelegate m_errCallback;
     
      //------------------------------------------------------------------------------
      public enum EPrec
      {
        // binary operators
        prLOGIC = 1,
        prCMP = 2,
        prADD_SUB = 3,
        prMUL_DIV = 4,
        prPOW = 5,

        // infix operators
        prINFIX = 4,
        prPOSTFIX = 4
      };

      
      //---------------------------------------------------------------------------
      // Delegates
      //---------------------------------------------------------------------------
      #region Delegate definitions

      //[UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl)]
      //protected delegate IntPtr FactoryDelegate(String name, IntPtr parser);

      // Value identification callback
      [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl)]
      public delegate int IdentFunDelegate(String name, ref int pos, ref double val);

      // Callback for errors 
      [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl)]
      protected delegate void ErrorDelegate();

      // Functions taking double arguments
      [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl)]
      public delegate double Fun1Delegate(double val1);
      
      [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl)]
      public delegate double Fun2Delegate(double val1, double val2);
      
      [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl)]
      public delegate double Fun3Delegate(double val1, double val2, double val3);
      
      [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl)]
      public delegate double Fun4Delegate(double val1, double val2, double val3, double val4);
      
      [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl)]
      public delegate double Fun5Delegate(double val1, double val2, double val3, double val4, double val5);

      // Functions taking an additional string parameter
      [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl)]
      public delegate double StrFun1Delegate(String name);
      
      [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl)]
      public delegate double StrFun2Delegate(String name, double val1);

      [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl)]
      public delegate double StrFun3Delegate(String name, double val1, double val2);

      // Functions taking an additional string parameter
      [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl)]
      public delegate double MultFunDelegate(
                  [MarshalAs(UnmanagedType.LPArray, SizeParamIndex=1)] double[] array,
                  int size);

      #endregion

      //---------------------------------------------------------------------------
      // Parser methode wrappers
      //---------------------------------------------------------------------------
      #region Parser methode wrappers

      public Parser()
      {
        m_parser = mupCreate();
        Debug.Assert(m_parser != null, "Parser object is null");
        m_errCallback = new ErrorDelegate(RaiseException);
        mupSetErrorHandler(m_parser, m_errCallback);
      }

     ~Parser()
      {
          mupRelease(m_parser);
      }

      protected void RaiseException()
      {
        string s = mupGetExpr(m_parser);

        ParserException exc = new ParserException( mupGetExpr(m_parser),
                                                   mupGetErrorMsg(m_parser),
                                                   mupGetErrorPos(m_parser),
                                                   mupGetErrorToken(m_parser) );
        throw exc;
      }

      public void AddValIdent(IdentFunDelegate fun)
      {
        mupAddValIdent(m_parser, fun);
      }

      public void SetExpr(string expr)
      {
        mupSetExpr(m_parser, expr);
      }

      public string GetExpr() 
      {
        return mupGetExpr(m_parser);
      }

      public double Eval()
      {
        return mupEval(m_parser);
      }

      public void DefineConst(string name, double val )
      {
          mupDefineConst(m_parser, name, val);
      }

      public void DefineStrConst(string name, String str )
      {
          mupDefineStrConst(m_parser, name, str);
      }
      
      public void DefineVar(string name, ParserVariable var)
      {
        mupDefineVar(m_parser, name, var.Pointer);
        m_varBuf[name] = var;
      }

      public void RemoveVar(string name)
      {
        mupRemoveVar(m_parser, name);
        m_varBuf.Remove(name);
      }

      public void ClearVar()
      {
          mupClearVar(m_parser);
      }

      public void ClearConst()
      {
          mupClearConst(m_parser);
      }

      public void ClearOprt()
      {
        m_binOprtDelegates.Clear();  
        mupClearOprt(m_parser);
      }

      public void ClearFun()
      {
        m_funDelegates.Clear();
        mupClearFun(m_parser);
      }

      public void DefineFun(string name, Fun1Delegate function)
      {
        m_funDelegates.Add(function);
        mupDefineFun1(m_parser, name, function, 0);
      }

      public void DefineFun(string name, Fun2Delegate function)
      {
        m_funDelegates.Add(function);
        mupDefineFun2(m_parser, name, function, 0);
      }

      public void DefineFun(string name, Fun3Delegate function)
      {
        m_funDelegates.Add(function);
        mupDefineFun3(m_parser, name, function, 0);
      }

      public void DefineFun(string name, Fun4Delegate function)
      {
        m_funDelegates.Add(function);
        mupDefineFun4(m_parser, name, function, 0);
      }

      public void DefineFun(string name, Fun5Delegate function)
      {
        m_funDelegates.Add(function);
        mupDefineFun5(m_parser, name, function, 0);
      }

      public void DefineFun(string name, StrFun1Delegate function)
      {
        m_funDelegates.Add(function);
        mupDefineStrFun1(m_parser, name, function);
      }

      public void DefineFun(string name, StrFun2Delegate function)
      {
        m_funDelegates.Add(function);
        mupDefineStrFun2(m_parser, name, function);
      }

      public void DefineFun(string name, StrFun3Delegate function)
      {
        m_funDelegates.Add(function);
        mupDefineStrFun3(m_parser, name, function);
      }

      public void DefineFun(string name, MultFunDelegate function)
      {
        m_funDelegates.Add(function);
        mupDefineMultFun(m_parser, name, function, 0);
      }

      public void DefineOprt(string name, Fun2Delegate function, int precedence)
      {
        m_binOprtDelegates.Add(function);
        mupDefineOprt(m_parser, name, function, precedence, 0);
      }

      public void DefinePostfixOprt(string name, Fun1Delegate oprt)
      {
        m_binOprtDelegates.Add(oprt);
        mupDefinePostfixOprt(m_parser, name, oprt, 0);    
      }

      public void DefineInfixOprt(string name, Fun1Delegate oprt, EPrec precedence)
      {
        m_binOprtDelegates.Add(oprt);
        mupDefineInfixOprt(m_parser, name, oprt, 0 );    
      }

      public Dictionary<string, double> GetConst()
      {
        int num = mupGetConstNum(m_parser);

        Dictionary<string, double> map = new Dictionary<string, double>();
        for (int i = 0; i < num; ++i)
        {
          string name = "";
          double value = 0;
          mupGetConst(m_parser, i, ref name, ref value);

          map[name] = value;
        }

        return map;
      }

      public Dictionary<string, ParserVariable> GetVar()
      {
        return m_varBuf;
      }

      public Dictionary<string, IntPtr> GetExprVar()
      {
        int num = mupGetExprVarNum(m_parser);

        Dictionary<string, IntPtr> map = new Dictionary<string, IntPtr>();
        for (int i = 0; i < num; ++i)
        {
          string name = "";
          IntPtr ptr = IntPtr.Zero;
          mupGetExprVar(m_parser, i, ref name, ref ptr);

          map[name] = ptr;
        }

        return map;
      }

      public void SetArgSep(char cArgSep)
      {
        mupSetArgSep(m_parser, Convert.ToByte(cArgSep));  
      }

      public void SetDecSep(char cDecSep)
      {
        mupSetDecSep(m_parser, Convert.ToByte(cDecSep));
      }

      public void SetThousandsSep(char cThSep)
      {
        mupSetThousandsSep(m_parser, Convert.ToByte(cThSep));
      }

      #endregion


      #region DLL function bindings

      //----------------------------------------------------------
      // Basic operations / initialization  
      //----------------------------------------------------------

      [DllImport("muparser.dll")]
      protected static extern IntPtr mupCreate();
      
      [DllImport("muparser.dll")]
      protected static extern void mupRelease(IntPtr a_pParser);
      
      [DllImport("muparser.dll")]
      protected static extern string mupGetExpr(IntPtr a_pParser);
      
      [DllImport("muparser.dll")]
      protected static extern void mupSetExpr(IntPtr a_pParser, string a_szExpr);
      
      [DllImport("muparser.dll")]
      protected static extern void mupSetErrorHandler(IntPtr a_pParser, ErrorDelegate errFun);

      //---------------------------------------------------------------------------
      // Non numeric callbacks
      //---------------------------------------------------------------------------

      //[DllImport("muparser.dll")]
      //protected static extern void mupSetVarFactory(HandleRef a_pParser, muFacFun_t a_pFactory, void* pUserData);

      [DllImport("muparser.dll")]
      protected static extern void mupAddValIdent(IntPtr a_parser, IdentFunDelegate fun);

      //----------------------------------------------------------
      // Defining variables and constants
      //----------------------------------------------------------

      [DllImport("muparser.dll")]
      protected static extern void mupDefineConst( IntPtr a_pParser, 
                                                   string a_szName, 
                                                   double a_fVal );

      [DllImport("muparser.dll")]
      protected static extern void mupDefineStrConst( IntPtr parser, 
                                                      string name, 
                                                      string val );

      [DllImport("muparser.dll")]
      protected static extern void mupDefineVar( IntPtr parser, 
                                                 string name,
                                                 IntPtr var );

      //----------------------------------------------------------
      // Querying variables / expression variables / constants
      //----------------------------------------------------------

      [DllImport("muparser.dll")]
      protected static extern int mupGetExprVarNum(IntPtr a_parser);

      [DllImport("muparser.dll")]
      protected static extern void mupGetExprVar(IntPtr a_parser,
                                                 int idx,
                                                 ref string name,
                                                 ref IntPtr ptr);

      [DllImport("muparser.dll")]
      protected static extern int mupGetVarNum(IntPtr a_parser);

      [DllImport("muparser.dll")]
      protected static extern void mupGetVar(IntPtr a_parser,
                                             int idx,
                                             ref string name,
                                             ref IntPtr ptr);

      [DllImport("muparser.dll")]
      protected static extern int mupGetConstNum(IntPtr a_parser);

      [DllImport("muparser.dll")]
      protected static extern void mupGetConst( IntPtr a_parser, 
                                                int idx, 
                                                ref string str,
                                                ref double value);

      //[DllImport("muparser.dll")]
      //protected static extern void mupGetExprVar(IntPtr a_parser, unsigned a_iVar, const muChar_t** a_pszName, muFloat_t** a_pVar);

      //----------------------------------------------------------
      // Remove all / single variables
      //----------------------------------------------------------

      [DllImport("muparser.dll")]
      protected static extern void mupRemoveVar(IntPtr a_parser, string name);

      [DllImport("muparser.dll")]
      protected static extern void mupClearVar(IntPtr a_parser);
      
      [DllImport("muparser.dll")]
      protected static extern void mupClearConst(IntPtr a_parser);

      [DllImport("muparser.dll")]
      protected static extern void mupClearOprt(IntPtr a_parser);

      [DllImport("muparser.dll")]
      protected static extern void mupClearFun(IntPtr a_parser);

      //----------------------------------------------------------
      // Define character sets for identifiers
      //----------------------------------------------------------

      [DllImport("muparser.dll")]
      protected static extern void mupDefineNameChars(IntPtr a_parser, string charset);

      [DllImport("muparser.dll")]
      protected static extern void mupDefineOprtChars(IntPtr a_parser, string charset);

      [DllImport("muparser.dll")]
      protected static extern void mupDefineInfixOprtChars(IntPtr a_parser, string charset);

      //----------------------------------------------------------
      // Defining callbacks / variables / constants
      //----------------------------------------------------------

      [DllImport("muparser.dll")]
      protected static extern void mupDefineFun1(IntPtr a_parser, string name, Fun1Delegate fun, int optimize);

      [DllImport("muparser.dll")]
      protected static extern void mupDefineFun2(IntPtr a_parser, string name, Fun2Delegate fun, int optimize);
        
      [DllImport("muparser.dll")]
       protected static extern void mupDefineFun3(IntPtr a_parser, string name, Fun3Delegate fun, int optimize);
       
      [DllImport("muparser.dll")]
      protected static extern void mupDefineFun4(IntPtr a_parser, string name, Fun4Delegate fun, int optimize);

      [DllImport("muparser.dll")]
      protected static extern void mupDefineFun5(IntPtr a_parser, string name, Fun5Delegate fun, int optimize);

      // string functions
      [DllImport("muparser.dll")]
      protected static extern void mupDefineStrFun1(IntPtr a_parser, string name, StrFun1Delegate fun);
        
      [DllImport("muparser.dll")]
      protected static extern void mupDefineStrFun2(IntPtr a_parser, string name, StrFun2Delegate fun);

      [DllImport("muparser.dll")]
      protected static extern void mupDefineStrFun3(IntPtr a_parser, string name, StrFun3Delegate fun);

      // Multiple argument functions
      [DllImport("muparser.dll")]
      protected static extern void mupDefineMultFun(IntPtr a_parser, string name, MultFunDelegate fun, int optimize);

      //----------------------------------------------------------
      // Operator definitions
      //----------------------------------------------------------

      [DllImport("muparser.dll")]
      protected static extern void mupDefineOprt(IntPtr a_pParser, 
                                                 string name, 
                                                 Fun2Delegate fun, 
                                                 int precedence, 
                                                 int optimize);

      [DllImport("muparser.dll")]
      protected static extern void mupDefinePostfixOprt(IntPtr a_pParser, 
                                                        string id, 
                                                        Fun1Delegate fun, 
                                                        int optimize );

      [DllImport("muparser.dll")]
      protected static extern void mupDefineInfixOprt(IntPtr a_pParser, 
                                                      string id, 
                                                      Fun1Delegate fun,
                                                      int optimize);
      
      //----------------------------------------------------------
      // 
      //----------------------------------------------------------

      [DllImport("muparser.dll")]
      protected static extern double mupEval(IntPtr a_pParser);

      [DllImport("muparser.dll")]
      protected static extern int mupError(IntPtr a_pParser);

      [DllImport("muparser.dll")]
      protected static extern void mupErrorReset(IntPtr a_pParser);

      [DllImport("muparser.dll")]
      protected static extern int mupGetErrorCode(IntPtr a_pParser);

      [DllImport("muparser.dll")]
      protected static extern int mupGetErrorPos(IntPtr a_pParser);

      [DllImport("muparser.dll", CallingConvention=CallingConvention.StdCall)] //[return : MarshalAs(UnmanagedType.LPStr)]
      protected static extern string mupGetErrorMsg(IntPtr a_pParser);

      [DllImport("muparser.dll")]
      protected static extern string mupGetErrorToken(IntPtr a_pParser);
        
      //[DllImport("muparser.dll")]
      //protected static extern string muGetErrorExpr(IntPtr a_pParser);

      //----------------------------------------------------------
      // 
      //----------------------------------------------------------
      
      [DllImport("muparser.dll")]
      protected static extern void mupSetArgSep(IntPtr a_pParser, byte cArgSep);
      
      [DllImport("muparser.dll")]
      protected static extern void mupSetDecSep(IntPtr a_pParser, byte cArgSep);
      
      [DllImport("muparser.dll")]
      protected static extern void mupSetThousandsSep(IntPtr a_pParser, byte cArgSep);
        
      #endregion
    }
}
