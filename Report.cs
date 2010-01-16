//-----------------------------------------------------------------------
// <copyright file="Report.cs" company="Home Corporation">
//     Copyright (c) Home Corporation 2010. All rights reserved.
// </copyright>
// <author>Sergii Pechenizkyi</author>
//-----------------------------------------------------------------------

namespace Optimization.VisualApplication
{
    using System.Windows;
    using Microsoft.Research.DynamicDataDisplay.DataSources.MultiDimensional;
    using Optimization.Methods;

    /// <summary>
    /// Класс, отвечающий за построение значений, для линии уровня функции (массив значений f(x1,x2) и сами значения x1,x2).
    /// </summary>
    internal class Report
    {
        #region Public Fields

        #endregion

        #region Private Fields

        #endregion

        #region Constructors

        #endregion

        #region Properties
        public int Iteration { get; set; }
        public double PointX { get; set; }
        public double PointY { get; set; }
        public double FuncValue { get; set; }
        #endregion

        #region Public Methods

        #endregion

        #region Private Methods

        #endregion

        #region Structs

        #endregion
    }
}
