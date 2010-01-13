//-----------------------------------------------------------------------
// <copyright file="DataLayer.cs" company="Home Corporation">
//     Copyright (c) Home Corporation 2009. All rights reserved.
// </copyright>
// <author>Sergii Pechenizkyi</author>
//-----------------------------------------------------------------------

namespace Optimization.VisualApplication
{
    using System.Windows;
    using System.Windows.Media;
    using Optimization.Methods;

    internal class LineSource
    {
        #region Public Fields
        
        #endregion

        #region Private Fields
        private readonly ManyVariable function;
        private double[][] solutions;
        private int pointsCount;
        #endregion

        #region Constructors
        public LineSource(ManyVariable function)
        {
            this.function = function;
        }
        #endregion

        #region Properties
        internal int PointsCount
        {
            get { return pointsCount; }
        }
        #endregion

        #region Public Methods
        internal PointCollection GetPointCollection(object methodIndex, double[] startingPoint)
        {
            switch ((Methods)methodIndex)
            {
                case (Methods.Gradient):
                    solutions = Minimum.GradientDescentExtended(function, 2, startingPoint);
                    pointsCount = solutions.Length;
                    return GetPoints();
                    break;
                case (Methods.Hooke_Jeves):
                    solutions = Minimum.HookeJeveesExtended(function, 2, startingPoint);
                    pointsCount = solutions.Length;
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