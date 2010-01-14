//-----------------------------------------------------------------------
// <copyright file="Point.cs" company="Home Corporation">
//     Copyright (c) Home Corporation 2010. All rights reserved.
// </copyright>
// <author>Sergii Pechenizkyi</author>
//-----------------------------------------------------------------------

namespace Optimization.Methods
{
    /// <summary>
    /// Метафора точки x[n0,n1,n2...,n].
    /// </summary>
    internal struct Point
    {
        #region Private Member Variables
        /// <summary>
        /// Набор переменных, описывающих точку (вершину).
        /// </summary>
        private double[] point;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="Point"/> struct.
        /// </summary>
        /// <param name="dimension">The dimension.</param>
        public Point(int dimension)
        {
            this.point = new double[dimension];
            for (int i = 0; i < dimension; i++)
            {
                this.point[i] = 0;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Point"/> struct.
        /// </summary>
        /// <param name="point">The point.</param>
        public Point(double[] point)
        {
            this.point = new double[point.Length];
            for (int i = 0; i < point.Length; i++)
            {
                this.point[i] = point[i];
            }
        }
        #endregion

        #region Public Properties

        #endregion

        #region Public Methods
        /// <summary>
        /// Implements the operator +.
        /// </summary>
        /// <param name="leftOperand">The left operand.</param>
        /// <param name="rigthOperand">The rigth operand.</param>
        /// <returns>The result of the operator.</returns>
        public static Point operator +(Point leftOperand, Point rigthOperand)
        {
            Point sum = new Point(leftOperand.point.Length);
            for (int i = 0; i < leftOperand.point.Length; i++)
            {
                sum.point[i] = leftOperand.point[i] + rigthOperand.point[i];
            }

            return sum;
        }

        /// <summary>
        /// Implements the operator -.
        /// </summary>
        /// <param name="leftOperand">The left operand.</param>
        /// <param name="rigthOperand">The rigth operand.</param>
        /// <returns>The result of the operator.</returns>
        public static Point operator -(Point leftOperand, Point rigthOperand)
        {
            Point sub = new Point(leftOperand.point.Length);
            for (int i = 0; i < leftOperand.point.Length; i++)
            {
                sub.point[i] = leftOperand.point[i] - rigthOperand.point[i];
            }

            return sub;
        }

        /// <summary>
        /// Implements the operator /.
        /// </summary>
        /// <param name="leftOperand">The left operand.</param>
        /// <param name="number">The number.</param>
        /// <returns>The result of the operator.</returns>
        public static Point operator /(Point leftOperand, int number)
        {
            Point div = new Point(leftOperand.point.Length);
            for (int i = 0; i < leftOperand.point.Length; i++)
            {
                div.point[i] = leftOperand.point[i] / number;
            }

            return div;
        }

        /// <summary>
        /// Implements the operator *.
        /// </summary>
        /// <param name="number">The number.</param>
        /// <param name="rigthOperand">The rigth operand.</param>
        /// <returns>The result of the operator.</returns>
        public static Point operator *(double number, Point rigthOperand)
        {
            Point mul = new Point(rigthOperand.point.Length);
            for (int i = 0; i < rigthOperand.point.Length; i++)
            {
                mul.point[i] = rigthOperand.point[i] * number;
            }

            return mul;
        }

        /// <summary>
        /// Sets the equal.
        /// </summary>
        /// <param name="rigthOperand">The rigth operand.</param>
        public void SetEqual(Point rigthOperand)
        {
            for (int i = 0; i < rigthOperand.point.Length; i++)
            {
                this.point[i] = rigthOperand.point[i];
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder(this.point[0].ToString());

            for (int i = 1; i < this.point.Length; i++)
            {
                sb.Append(" ");
                sb.Append(this.point[i].ToString());
            }

            return sb.ToString();
        }

        /// <summary>
        /// Returns a <see cref="System.Double"/> that represents this instance.
        /// </summary>
        /// <returns> A <see cref="System.Double"/> that represents this instance.</returns>
        public double[] ToDouble()
        {
            return this.point;
        }
        #endregion
    }
}
