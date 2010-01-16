//-----------------------------------------------------------------------
// <copyright file="Hooke-Jevees.cs" company="Home Corporation">
//     Copyright (c) Home Corporation 2009. All rights reserved.
// </copyright>
// <author>Sergii Pechenizkyi</author>
//-----------------------------------------------------------------------

namespace Optimization.Methods.ZerothOrder
{
    using System.Diagnostics;
    using Optimization.Methods;

    /// <summary>
    /// Нахождение безусловного минимума функции многих переменных методом Хука-Дживса
    /// </summary>
    internal class Hooke_Jevees
    {
        #region Private Fields
        /// <summary>
        /// Ссылка на функциональную зависимость.
        /// </summary>
        private readonly ManyVariable Function;

        /// <summary>
        /// Количество перменных
        /// </summary>
        private readonly int Dimension;

        /// <summary>
        /// ускоряющий множитель lyamda > 0
        /// </summary>
        private readonly double AccelerateCoefficient;

        /// <summary>
        /// коэффициент уменьшения шага аlfa > 1.
        /// </summary>
        private readonly double CoefficientReduction;

        /// <summary>
        /// Значение шага по каждой из координат.
        /// </summary>
        private double[] step;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="Hooke_Jevees"/> class.
        /// </summary>
        /// <param name="inputFunc">The input function.</param>
        /// <param name="inputParams">The input method parameters.</param>
        public Hooke_Jevees(ManyVariable inputFunc, int dimension, double accelerateCoefficient, double coefficientReduction, double[] step)
        {
            Debug.Assert(accelerateCoefficient > 0, "Accelerate coefficient lyamda is unexepectedly less or equal zero");
            Debug.Assert(coefficientReduction > 1, "Coefficient reduction alfa is unexepectedly less or equal 1");
            Debug.Assert(dimension > 1, "Dimension is unexepectedly less or equal 1");
            this.AccelerateCoefficient = accelerateCoefficient;
            this.CoefficientReduction = coefficientReduction;
            this.step = step;

            Debug.Assert(inputFunc != null, "Input function reference is unexepectedly null");
            this.Function = inputFunc;
            this.Dimension = dimension;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Hooke_Jevees"/> class.
        /// </summary>
        /// <param name="inputFunc">The input function.</param>
        /// <param name="funcDimension">Количество переменных.</param>
        public Hooke_Jevees(ManyVariable inputFunc, int funcDimension)
        {
            this.Function = inputFunc;
            this.AccelerateCoefficient = 1.5;
            this.CoefficientReduction = 4;
            this.Dimension = funcDimension;
            this.step = new double[funcDimension];
            for (int i = 0; i < funcDimension; i++)
            {
                this.step[i] = 1;
            }
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

            double[] newBasis = startPoint;
            double[] oldBasis = startPoint;

            while (true)
            {
                // Шаг 2. Осуществить исследующий поиск по выбранному координатному направлению (i)
                newBasis = this.ExploratarySearch(newBasis);

                // Проверить успешность исследующего поиска:
                if (this.Function(newBasis) < this.Function(oldBasis))
                {
                    // перейти к шагу 4;

                    // Сформируем х[k]
                    double[] oldOldBasis = new double[this.Dimension];
                    for (int i = 0; i < this.Dimension; i++)
                    {
                        oldOldBasis[i] = oldBasis[i];
                    }

                    // Шаг 4. Провести поиск по образцу. Положить x[k + 1] = yn+l,
                    for (int i = 0; i < this.Dimension; i++)
                    {
                        oldBasis[i] = newBasis[i];
                    }

                    // y[0] = x[k + 1] + AccelerateCoefficient * (x[k + 1] - x[k]);
                    newBasis = this.PatternSearch(oldOldBasis, oldBasis);

                    // перейти к шагу 2.
                    continue;
                }
                else
                {
                    // перейти к шагу 5.

                    // Шаг 5. Проверить условие окончания:
                    if (!this.AllStepsLessPrecision(precision))
                    {
                        for (int index = 0; index < this.Dimension; index++)
                        {
                            // Для значений шагов, больших точности
                            if (this.step[index] > precision)
                            {
                                // Уменьшить величину шага
                                this.step[index] /= this.CoefficientReduction;
                            }
                        }

                        for (int i = 0; i < this.Dimension; i++)
                        {
                            newBasis[i] = oldBasis[i];
                        }

                        // перейти к шагу 2.
                        continue;
                    }
                    else
                    {
                        // Значение всех шагов меньше точности
                        // Поиск закончен
                        return oldBasis;
                    }
                }
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Пробный шаг.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns>Новую точку.</returns>
        private double[] ExploratarySearch(double[] point)
        {
            Point result = new Point(point);

            for (int i = 0; i < this.Dimension; i++)
            {
                if (this.Function(this.GetPositiveProbe(result.ToDouble(), i)) < this.Function(result.ToDouble()))
                {
                    // шаг считается удачным
                    result = new Point(this.GetPositiveProbe(result.ToDouble(), i));
                }
                else
                {
                    // шаг неудачен, делаем шаг в противоположном направлении
                    if (this.Function(this.GetNegativeProbe(result.ToDouble(), i)) < this.Function(result.ToDouble()))
                    {
                        // шаг в противоположном направлении считается удачным
                        result = new Point(this.GetNegativeProbe(result.ToDouble(), i));
                    }
                    else
                    {
                        // оба шага неудачны
                        // y[i + 1] = y[i];
                    }
                }
            }

            return result.ToDouble();
        }

        /// <summary>
        /// Пробный шаг в положительном направлении.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <param name="i">Координата, по которой делаем шаг.</param>
        /// <returns>Новую точку.</returns>
        private double[] GetPositiveProbe(double[] point, int i)
        {
            Point ptPoint = new Point(point);
            double[] solution = ptPoint.ToDouble();
            solution[i] += this.step[i];
            return solution;
        }

        /// <summary>
        /// Пробный шаг в отрицательном направлении.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <param name="i">Координата, по которой делаем шаг.</param>
        /// <returns>Новую точку.</returns>
        private double[] GetNegativeProbe(double[] point, int i)
        {
            Point ptPoint = new Point(point);
            double[] solution = ptPoint.ToDouble();
            solution[i] -= this.step[i];
            return solution;
        }

        /// <summary>
        /// Поиск по образцу. y[0] = x[k + 1] + AccelerateCoefficient * (x[k + 1] - x[k])
        /// </summary>
        /// <param name="oldBasis">The old basis.</param>
        /// <param name="basis">The basis.</param>
        /// <returns>Новую точку.</returns>
        private double[] PatternSearch(double[] oldBasis, double[] basis)
        {
            Point oldBas = new Point(oldBasis);
            Point bas = new Point(basis);
            return (bas + (this.AccelerateCoefficient * (bas - oldBas))).ToDouble();
        }

        /// <summary>
        /// Alls the steps less precision.
        /// </summary>
        /// <param name="precision">The precision.</param>
        /// <returns>True, если все</returns>
        private bool AllStepsLessPrecision(double precision)
        {
            for (int index = 0; index < this.Dimension; index++)
            {
                if (this.step[index] > precision)
                {
                    return false;
                }
            }

            return true;
        }
        #endregion

        #region Structs

        #endregion
    }
}