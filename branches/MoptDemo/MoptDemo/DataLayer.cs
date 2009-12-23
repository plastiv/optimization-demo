//-----------------------------------------------------------------------
// <copyright file="DataLayer.cs" company="Home Corporation">
//     Copyright (c) Home Corporation 2009. All rights reserved.
// </copyright>
// <author>Sergii Pechenizkyi</author>
//-----------------------------------------------------------------------

namespace MoptDemo
{
    using System.Windows;
    using System.Windows.Media;
    using OptimizationMethods;

    internal class DataLayer
    {
        #region Public Fields
        internal ManyVariable Function
        {
            get;
            set;
        }

        internal int SolutionCount
        {
            get { return solutionCount; }
        }
        #endregion

        #region Private Fields
        private double[][] solutions;
        private int solutionCount;
        #endregion

        #region Constructors

        #endregion

        #region Properties

        #endregion

        #region Public Methods
        internal PointCollection GetSolutionPoints(object methodIndex, double[] startingPoint)
        {
            switch ((Methods)methodIndex)
            {
                case (Methods.Gradient):
                    solutions = Minimum.GradientDescentExtended(Function, 2, startingPoint);
                    solutionCount = solutions.Length;
                    return GetPoints();
                    break;
                case (Methods.Hooke_Jeves):
                    solutions = Minimum.HookeJeveesExtended(Function, 2, startingPoint);
                    solutionCount = solutions.Length;
                    return GetPoints();
                    break;
                default:
                    return null;
                    break;
            }
        }

        internal Point GetCurrPoint(int index)
        {
            return new Point(solutions[index][0], solutions[index][1]);
        }
        #endregion

        #region Private Methods
        private PointCollection GetPoints()
        {
            PointCollection solPoint = new PointCollection(solutions.Length);

            for (int i = 0; i < solutions.Length; i++)
            {
                solPoint.Add(new Point(solutions[i][0], solutions[i][1]));
            }

            return solPoint;
        }
        #endregion

        #region Structs
        internal enum Methods
        {
            Gradient,
            Hooke_Jeves
        }
        #endregion
    }
}