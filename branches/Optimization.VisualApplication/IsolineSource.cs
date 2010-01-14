//-----------------------------------------------------------------------
// <copyright file="DataSource.cs" company="Home Corporation">
//     Copyright (c) Home Corporation 2009. All rights reserved.
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
    internal static class IsolineSource
    {
        #region Public Fields

        #endregion

        #region Private Fields
        private readonly ManyVariable function;
        private readonly int minValueX;
        private readonly int maxValueX;
        private readonly int pointCountX;
        private readonly int minValueY;
        private readonly int maxValueY;
        private readonly int pointCountY;
        #endregion

        #region Constructors
        public IsolineSource(ManyVariable func, int minX, int maxX, int minY, int maxY, int pointCountX, int pointCountY)
        {
            this.function = func;
            this.minValueX = minX;
            this.maxValueX = maxX;
            this.minValueY = minY;
            this.maxValueY = maxY;
            this.pointCountX = pointCountX;
            this.pointCountY = pointCountY;
        }

        public IsolineSource(ManyVariable func, int minValue, int maxValue, int pointCount)
        {
            this.function = func;
            this.minValueX = minValue;
            this.maxValueX = maxValue;
            this.minValueY = minValue;
            this.maxValueY = maxValue;
            this.pointCountX = pointCount;
            this.pointCountY = pointCount;
        }
        #endregion

        #region Properties

        #endregion

        #region Public Methods
        /// <summary>
        /// Gets the data source.
        /// </summary>
        /// <param name="func">Функция двух переменных.</param>
        /// <param name="minValue">Минимальное значение x1,x2.</param>
        /// <param name="maxValue">Максимальное значение x1,x2.</param>
        /// <param name="pointCount">Количество расчитываемых точек в диапазоне |xmax-xmin|.</param>
        /// <returns>Значения f(x1,x2), x1, x2.</returns>
        internal static WarpedDataSource2D<double> GetDataSource(ManyVariable func, int minValue, int maxValue, int pointCount)
        {
            return new WarpedDataSource2D<double>(
                GetData(func, minValue, maxValue, pointCount),
                GetGridData(minValue, maxValue, pointCount));
        }
        #endregion

        #region Private Methods
        private double[] GetPoints(int maxValue, int minValue, int pointCount)
        {
            double step = (double)(maxValue - minValue) / pointCount;
            double[] result = new double[pointCount];

            for (int i = 0; i < pointCount; i++)
            {
                result[i] = minValue + (i * step);
            }

            return result;
        }

        /// <summary>
        /// Gets сетку точек x1,x2 для которых будет построена линия уровня.
        /// </summary>
        /// <param name="minX">Минимальное значение x1, с которого начинать.</param>
        /// <param name="maxX">Максимальное значение x1, которым заканчивать.</param>
        /// <param name="minY">Минимальное значение x2, с которого начинать.</param>
        /// <param name="maxY">Максимальное значение x2, которым заканчивать</param>
        /// <param name="pointCountX">Количество расчитываемых точек в диапазоне |x1max-x1min|.</param>
        /// <param name="pointCountY">Количество расчитываемых точек в диапазоне |x2max-x2min|.</param>
        /// <returns>Точки[x1, x2].</returns>
        private static Point[,] GetGridData(int minX, int maxX, int minY, int maxY, int pointCountX, int pointCountY)
        {
            double stepX = (double)(maxX - minX) / pointCountX;
            double stepY = (double)(maxY - minY) / pointCountY;

            double[] pointX = new double[pointCountX];
            double[] pointY = new double[pointCountY];
            for (int row = 0; row < pointCountX; row++)
            {
                pointX[row] = minX + (row * stepX);
            }

            for (int column = 0; column < pointCountY; column++)
            {
                pointY[column] = minY + (column * stepY);
            }

            Point[,] gridData = new Point[pointCountY, pointCountX];
            for (int row = 0; row < pointCountX; row++)
            {
                for (int column = 0; column < pointCountY; column++)
                {
                    gridData[column, row] = new Point(pointY[column], pointX[row]);
                }
            }

            return gridData;
        }

        /// <summary>
        /// Gets сетку точек x1,x2 для которых будет построена линия уровня.
        /// </summary>
        /// <param name="minValue">Минимальное значение x1,x2.</param>
        /// <param name="maxValue">Максимальное значение x1,x2.</param>
        /// <param name="pointCount">Количество расчитываемых точек в диапазоне |xmax-xmin|.</param>
        /// <returns>Точки[x1, x2].</returns>
        private static Point[,] GetGridData(int minValue, int maxValue, int pointCount)
        {
            return GetGridData(minValue, maxValue, minValue, maxValue, pointCount, pointCount);
        }

        // ------------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets значение функции в точках x1,x2 для которых будет построена линия уровня.
        /// </summary>
        /// <param name="func">Функция двух переменных.</param>
        /// <param name="minX">Минимальное значение x1, с которого начинать.</param>
        /// <param name="maxX">Максимальное значение x1, которым заканчивать.</param>
        /// <param name="minY">Минимальное значение x2, с которого начинать.</param>
        /// <param name="maxY">Максимальное значение x2, которым заканчивать</param>
        /// <param name="pointCountX">Количество расчитываемых точек в диапазоне |x1max-x1min|.</param>
        /// <param name="pointCountY">Количество расчитываемых точек в диапазоне |x2max-x2min|.</param>
        /// <returns>Значение функции в точках x1,x2.</returns>
        private static double[,] GetData(ManyVariable func, int minX, int maxX, int minY, int maxY, int pointCountX, int pointCountY)
        {
            double stepX = (double)(maxX - minX) / pointCountX;
            double stepY = (double)(maxY - minY) / pointCountY;

            double[] pointX = new double[pointCountX];
            double[] pointY = new double[pointCountY];
            for (int row = 0; row < pointCountX; row++)
            {
                pointX[row] = minX + (row * stepX);
            }

            for (int column = 0; column < pointCountY; column++)
            {
                pointY[column] = minY + (column * stepY);
            }

            double[,] data = new double[pointCountX, pointCountY];
            for (int row = 0; row < pointCountY; row++)
            {
                for (int column = 0; column < pointCountX; column++)
                {
                    data[column, row] = func(new double[] { pointY[column], pointX[row] });
                }
            }

            return data;
        }

        /// <summary>
        /// Gets значение функции в точках x1,x2 для которых будет построена линия уровня.
        /// </summary>
        /// <param name="func">Функция двух переменных.</param>
        /// <param name="minValue">Минимальное значение x1,x2.</param>
        /// <param name="maxValue">Максимальное значение x1,x2.</param>
        /// <param name="pointCount">Количество расчитываемых точек в диапазоне |xmax-xmin|.</param>
        /// <returns>Значение функции в точках x1,x2.</returns>
        private static double[,] GetData(ManyVariable func, int minValue, int maxValue, int pointCount)
        {
            return GetData(func, minValue, maxValue, minValue, maxValue, pointCount, pointCount);
        }
        #endregion

        #region Structs

        #endregion
    }
}
