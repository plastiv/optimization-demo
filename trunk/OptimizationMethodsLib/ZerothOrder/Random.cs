//-----------------------------------------------------------------------
// <copyright file="Random.cs" company="Home Corporation">
//     Copyright (c) Home Corporation 2009. All rights reserved.
// </copyright>
// <author>Sergii Pechenizkyi</author>
//-----------------------------------------------------------------------

/// TODO: метод расходится на некоторых функциях. проверить унимодальные ли это функции?
namespace OptimizationMethods.ZerothOrder
{
    using System.Diagnostics;

    /// <summary>
    /// Нахождение безусловного минимума функции многих переменных методом случайного поиска
    /// </summary>
    internal class Random
    {
        #region Private Fields
        /// <summary>
        /// Ссылка на функциональную зависимость.
        /// </summary>
        private readonly ManyVariable func;

        /// <summary>
        /// Параметры метода.
        /// </summary>
        private readonly MethodParams param;

        /// <summary>
        /// Случайная величина.
        /// </summary>
        private System.Random rnd;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="Random"/> class.
        /// </summary>
        /// <param name="inputFunc">The input function.</param>
        /// <param name="inputParams">The input method parameters.</param>
        public Random(ManyVariable inputFunc, MethodParams inputParams)
        {
            Debug.Assert(inputFunc != null, "Input function reference is unexepectedly null");
            this.func = inputFunc;

            Debug.Assert(inputParams.Alfa >= 1, "Coefficient expansion alfa is unexepectedly less 1");
            Debug.Assert(inputParams.Beta > 0 && inputParams.Beta < 1, "Coefficient compression beta is unexepectedly less or equal 0 or greater 1");
            Debug.Assert(inputParams.Dimension > 1, "Dimension is unexepectedly less or equal 1");
            Debug.Assert(inputParams.ExaminationCount > 0, "Examination Count is unexepectedly less or equal 0");
            Debug.Assert(inputParams.IterationCount > 0, "Iteration Count is unexepectedly less or equal 0");
            Debug.Assert(inputParams.MinStepValue > 0, "Minimum Step Value is unexepectedly less or equal 0");
            Debug.Assert(inputParams.StartStepValue > 0, "Start Step Value Value is unexepectedly less or equal 0");
            this.param = inputParams;

            this.rnd = new System.Random();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Random"/> class.
        /// </summary>
        /// <param name="inputFunc">The input function.</param>
        /// <param name="funcDimension">Количество переменных.</param>
        public Random(ManyVariable inputFunc, int funcDimension)
        {
            Debug.Assert(inputFunc != null, "Input function reference is unexepectedly null");
            this.func = inputFunc;

            this.param.Alfa = 1.618; // Шумер и Стейглиц [Schumer М.А., Steiglitz К.] рекомендуют
            this.param.Beta = 0.618; // Шумер и Стейглиц [Schumer М.А., Steiglitz К.] рекомендуют
            Debug.Assert(funcDimension > 1, "Dimension is unexepectedly less or equal 1");
            this.param.Dimension = funcDimension;
            this.param.IterationCount = 10;
            this.param.ExaminationCount = 3 * this.param.IterationCount;
            this.param.StartStepValue = 1;
            this.param.MinStepValue = 0.8;

            this.rnd = new System.Random();
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Gets the minimum.
        /// </summary>
        /// <param name="startPoint">The start point.</param>
        /// <param name="precision">The precision.</param>
        /// <returns>Вектор значений х, при котором функция достигает минимума.</returns>
        internal double[] GetMinimum(double[] startPoint, double precision)
        {
            // Шаг 1. Задать начальную точку л:0
            // число е>0 для остановки алгоритма
            Debug.Assert(precision > 0, "Precision is unexepectedly less or equal zero");

            double step = this.param.StartStepValue;

            double[] x = new double[this.param.Dimension];
            double[] y = new double[this.param.Dimension];
            double[] z = new double[this.param.Dimension];

            for (int i = 0; i < this.param.Dimension; i++)
            {
                x[i] = startPoint[i];
            }

            int iteration = 0;
            int examination = 0;

            while (true)
            {
                // Шаг 2. Сгенерировать вектор случ. чисел
                double[] eps = new double[this.param.Dimension];
                for (int i = 0; i < this.param.Dimension; i++)
                {
                    eps[i] = this.rnd.NextDouble();
                    eps[i] = (2 * eps[i]) - 1;
                }

                // Шаг 3. Вычислить y = x + step * eps
                for (int i = 0; i < this.param.Dimension; i++)
                {
                    y[i] = x[i] + (step * (eps[i] / this.GetEuclideanNorm(eps)));
                }

                // Шаг 4. Проверить выполнение условий
                if (this.func(y) < this.func(x))
                {
                    // Шаг удачный
                    // Положить z = x + alfa * (y - x)
                    for (int i = 0; i < this.param.Dimension; i++)
                    {
                        double difference = y[i] - x[i];
                        z[i] = x[i] + (this.param.Alfa * difference);
                    }

                    // Определить является ли текущее направление (y - x) удачным
                    if (this.func(z) < this.func(x))
                    {
                        // Направление поиска удачное
                        // Положить x = z
                        for (int i = 0; i < this.param.Dimension; i++)
                        {
                            x[i] = z[i];
                        }

                        step = step * this.param.Alfa;
                        iteration++;

                        // Проверить условие окончания
                        if (iteration < this.param.IterationCount)
                        {
                            examination = 0;

                            // Перейти к шагу 2
                            continue;
                        }
                        else
                        {
                            // Поиск завершить
                            return x;
                        }
                    }
                    else
                    {
                        // Направление поиска неудачное
                        // Перейти к шагу 5
                    }
                }
                else
                {
                    // Шаг неудачный
                    // Перейти к шагу 5
                }

                // Шаг 5. Оценить число неудачных шагов из текущей точки
                if (examination < this.param.ExaminationCount)
                {
                    examination++;

                    // Перейти к шагу 2
                    continue;
                }
                else
                {
                    // Проверить условие окончания
                    if (step < this.param.MinStepValue)
                    {
                        // Процесс закончен
                        return x;
                    }
                    else
                    {
                        step = step * this.param.Beta;
                        examination = 0;

                        // Перейти к шагу 2
                        continue;
                    }
                }
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Получить евклидову норму вектора.
        /// </summary>
        /// <param name="vector">The vector.</param>
        /// <returns>Евклидова норма вектора.</returns>
        private double GetEuclideanNorm(double[] vector)
        {
            double eps = 0;

            for (int i = 0; i < this.param.Dimension; i++)
            {
                eps += vector[i] * vector[i];
            }

            eps = System.Math.Sqrt(eps);
            return eps;
        }
        #endregion

        #region Structs
        /// <summary>
        /// Параметры метода.
        /// </summary>
        internal struct MethodParams
        {
            /// <summary>
            /// Количество перменных
            /// </summary>
            internal int Dimension;

            /// <summary>
            /// Коэффициент расширения alfa >= 1
            /// </summary>
            internal double Alfa;

            /// <summary>
            /// коэффициент сжатия beta>0, 1>beta.
            /// </summary>
            internal double Beta;

            /// <summary>
            /// Минимальная величина шага.
            /// </summary>
            internal double MinStepValue;

            /// <summary>
            /// Начальная величина шага.
            /// </summary>
            internal double StartStepValue;

            /// <summary>
            /// Максимальное количество испытаний.
            /// </summary>
            internal int ExaminationCount;

            /// <summary>
            /// Максимальное количество итераций алгоритма.
            /// </summary>
            internal int IterationCount;
        }
        #endregion
    }
}
