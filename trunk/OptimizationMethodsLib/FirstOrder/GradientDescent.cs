//-----------------------------------------------------------------------
// <copyright file="GradientDescent.cs" company="Home Corporation">
//     Copyright (c) Home Corporation 2009. All rights reserved.
// </copyright>
// <author>Sergii Pechenizkyi</author>
//-----------------------------------------------------------------------

namespace OptimizationMethods.FirstOrder
{
    /// <summary>
    /// Ссылка на функциональную зависимость f(x1,х2,...,хn).
    /// </summary>
    /// <param name="x">Вектор значений переменных x1,х2,...,хn.</param>
    /// <returns>Значение функции у=f(x1,х2,...,хn).</returns>
    public delegate double GetManyVariableFunctionValue(double[] x);

    /// <summary>
    /// Ссылка на функциональную зависимость df/dxi, где xi = x1,х2,...,хn.
    /// </summary>
    /// <param name="x">Вектор значений переменных x1,х2,...,хn.</param>
    /// <returns>Значение градиента у=df/dxi, где xi = x1,х2,...,хn.</returns>
    public delegate double[] GetGradientOfFunction(double[] x);

    /// <summary>
    /// Метод градиентного спуска с постоянным шагом
    /// </summary>
    public class GradientDescent
    {
        #region Private Fields
        /// <summary>
        /// Количество переменных в минимизируемом уравнении.
        /// </summary>
        private readonly int dimension;

        /// <summary>
        /// Малое положительное число, ограничивающее евклидову норму градиента. 
        /// </summary>
        private readonly double epsilon1;

        /// <summary>
        /// Малое положительное число, ограничивающее разность значений функции в текущей и предыдущей точке. 
        /// </summary>
        private readonly double epsilon2;

        /// <summary>
        /// Максимальное число итераций для метода.
        /// </summary>
        private readonly int maxIteration;

        /// <summary>
        /// Искомая функция.
        /// </summary>
        private readonly GetManyVariableFunctionValue searchFunc;

        /// <summary>
        /// Первая производная искомой функции.
        /// </summary>
        private readonly GetGradientOfFunction searchGradient;

        /// <summary>
        /// Значение градиента в текущей точке.
        /// </summary>
        private double[] gradientValue;

        /// <summary>
        /// Величина шага deltax.
        /// </summary>
        private double step;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="GradientDescent"/> class.
        /// </summary>
        /// <param name="initParam">The initial parameters.</param>
        public GradientDescent(MethodParams initParam)
        {
            this.maxIteration = initParam.MaxIteration;
            this.dimension = initParam.Dimension;
            this.epsilon1 = initParam.Epsilon1;
            this.epsilon2 = initParam.Epsilon2;
            this.step = initParam.Step;
            this.searchFunc = initParam.SearchFunc;
            this.searchGradient = initParam.SearchGradient;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Нахождение безусловного локального минимума функции многих переменных.
        /// </summary>
        /// <param name="startPoint">Начальная точка.</param>
        /// <returns>Вектор значений х, при котором функция достигает минимума.</returns>
        public double[] GetMinimum(double[] startPoint)
        {
            int count = 0;
            double[] currPoint = startPoint;
            double[] prevPoint = startPoint;
            double[] prevPrevPoint = startPoint;

            this.gradientValue = this.searchGradient(startPoint);

            while (count < this.maxIteration && this.GetEuclideanNorm(this.gradientValue) > this.epsilon1)
            {
                count++;
                prevPrevPoint = prevPoint;
                prevPoint = currPoint;
                currPoint = this.GetNextPoint(prevPoint);
                if (this.GetEuclideanNorm(currPoint, prevPoint) < this.epsilon2
                    && this.searchFunc(currPoint) - this.searchFunc(prevPoint) < this.epsilon2)
                {
                    if (this.GetEuclideanNorm(prevPoint, prevPrevPoint) < this.epsilon2
                        && this.searchFunc(prevPoint) - this.searchFunc(prevPrevPoint) < this.epsilon2)
                    {
                        return currPoint;
                    }
                }
            }

            return currPoint;
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Получить значения х для следующей точки.
        /// </summary>
        /// <param name="previousPoint">Предыдущая точка.</param>
        /// <returns>Значения х.</returns>
        private double[] GetNextPoint(double[] previousPoint)
        {
            double[] refPoint = new double[this.dimension];
            this.gradientValue = this.searchGradient(previousPoint);

            while (true)
            {
                for (int i = 0; i < this.dimension; i++)
                {
                    refPoint[i] = previousPoint[i] - (this.step * this.gradientValue[i]);
                }

                if (this.searchFunc(refPoint) - this.searchFunc(previousPoint) < 0)
                {
                    return refPoint;
                }

                this.step /= 2;
            }
        }

        /// <summary>
        /// Получить евклидову норму разности двух векторов.
        /// </summary>
        /// <param name="vector1">The vector1.</param>
        /// <param name="vector2">The vector2.</param>
        /// <returns>Евклидова норма вектора.</returns>
        private double GetEuclideanNorm(double[] vector1, double[] vector2)
        {
            double eps = 0;

            for (int i = 0; i < this.dimension; i++)
            {
                eps += System.Math.Abs(vector1[i] - vector2[i]) * System.Math.Abs(vector1[i] - vector2[i]);
            }

            eps = System.Math.Sqrt(eps);
            return eps;
        }

        /// <summary>
        /// Получить евклидову норму вектора.
        /// </summary>
        /// <param name="vector">The vector.</param>
        /// <returns>Евклидова норма вектора.</returns>
        private double GetEuclideanNorm(double[] vector)
        {
            double eps = 0;

            for (int i = 0; i < this.dimension; i++)
            {
                eps += vector[i] * vector[i];
            }

            eps = System.Math.Sqrt(eps);
            return eps;
        }
        #endregion

        #region Structs
        /// <summary>
        /// Передаваемые в метод параметры.
        /// </summary>
        public struct MethodParams
        {
            /// <summary>
            /// Количество переменных в минимизируемом уравнении.
            /// </summary>
            public int Dimension;

            /// <summary>
            /// Малое положительное число, ограничивающее евклидову норму градиента. 
            /// </summary>
            public double Epsilon1;

            /// <summary>
            /// Малое положительное число, ограничивающее евклидову норму градиента. 
            /// </summary>
            public double Epsilon2;

            /// <summary>
            /// Величина шага deltax.
            /// </summary>
            public double Step;

            /// <summary>
            /// Максимальное число итераций для метода.
            /// </summary>
            public int MaxIteration;

            /// <summary>
            /// Искомая функция.
            /// </summary>
            public GetManyVariableFunctionValue SearchFunc;

            /// <summary>
            /// Первая производная искомой функции.
            /// </summary>
            public GetGradientOfFunction SearchGradient;
        }
        #endregion
    }
}
