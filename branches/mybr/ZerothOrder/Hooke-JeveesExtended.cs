//-----------------------------------------------------------------------
// <copyright file="Hooke-JeveesExtended.cs" company="Home Corporation">
//     Copyright (c) Home Corporation 2009. All rights reserved.
// </copyright>
// <author>Sergii Pechenizkyi</author>
//-----------------------------------------------------------------------

namespace OptimizationMethods.ZerothOrder
{
    using System.Diagnostics;
    using OptimizationMethods;
    using System.Collections.Generic;

    /// <summary>
    /// Нахождение безусловного минимума функции многих переменных методом Хука-Дживса
    /// </summary>
    internal class Hooke_JeveesExtended
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
        public Hooke_JeveesExtended(ManyVariable inputFunc, MethodParams inputParams)
        {
            Debug.Assert(inputParams.AccelerateCoefficient > 0, "Accelerate coefficient lyamda is unexepectedly less or equal zero");
            Debug.Assert(inputParams.CoefficientReduction > 1, "Coefficient reduction alfa is unexepectedly less or equal 1");
            Debug.Assert(inputParams.Dimension > 1, "Dimension is unexepectedly less or equal 1");
            this.param = inputParams;
            this.step = inputParams.Step;

            Debug.Assert(inputFunc != null, "Input function reference is unexepectedly null");
            this.func = inputFunc;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Hooke_Jevees"/> class.
        /// </summary>
        /// <param name="inputFunc">The input function.</param>
        /// <param name="funcDimension">Количество переменных.</param>
        public Hooke_JeveesExtended(ManyVariable inputFunc, int funcDimension)
        {
            this.func = inputFunc;
            this.param.AccelerateCoefficient = 1.5;
            this.param.CoefficientReduction = 4;
            this.param.Dimension = funcDimension;
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
        internal double[][] GetMinimum(double[] startPoint, double precision)
        {
            // Шаг 1. Задать начальную точку л:0
            // число е>0 для остановки алгоритма
            Debug.Assert(precision > 0, "Precision is unexepectedly less or equal zero");
            List<double[]> mylist = new List<double[]>();
            // MAGIC KEY
            //int count = 40;
            //int solIndex = 0;
            //double[][] sols = new double[count][];
            //for (int i = 0; i < count; i++)
            //{
            //    sols[i] = new double[this.param.Dimension];
            //}

            double[] newBasis = startPoint;
            double[] oldBasis = startPoint;
            mylist.Add(newBasis);

            while (true)
            {
                //for (int i = 0; i < this.param.Dimension; i++)
                //{
                //    sols[solIndex][i] = newBasis[i];
                //}

                //solIndex++;
                

                // Шаг 2. Осуществить исследующий поиск по выбранному координатному направлению (i)
                newBasis = this.ExploratarySearch(newBasis);
                mylist.Add(newBasis);

                // Проверить успешность исследующего поиска:
                if (this.func(newBasis) < this.func(oldBasis))
                {
                    // перейти к шагу 4;

                    // Сформируем х[k]
                    double[] oldOldBasis = new double[this.param.Dimension];
                    for (int i = 0; i < this.param.Dimension; i++)
                    {
                        oldOldBasis[i] = oldBasis[i];
                    }

                    // Шаг 4. Провести поиск по образцу. Положить x[k + 1] = yn+l,
                    oldBasis = newBasis;

                    // y[0] = x[k + 1] + param.AccelerateCoefficient * (x[k + 1] - x[k]);
                    newBasis = this.PatternSearch(oldOldBasis, oldBasis);
                    mylist.Add(newBasis);
                    // перейти к шагу 2.
                    continue;
                }
                else
                {
                    // перейти к шагу 5.

                    // Шаг 5. Проверить условие окончания:
                    if (!this.AllStepsLessPrecision(precision))
                    {
                        for (int index = 0; index < this.param.Dimension; index++)
                        {
                            // Для значений шагов, больших точности
                            if (this.step[index] > precision)
                            {
                                // Уменьшить величину шага
                                this.step[index] /= this.param.CoefficientReduction;
                            }
                        }

                        newBasis = oldBasis;
                        mylist.Add(newBasis);
                        //solIndex--;
                        // перейти к шагу 2.
                        continue;
                    }
                    else
                    {
                        // Значение всех шагов меньше точности
                        // Поиск закончен
                        //double[][] newResult = new double[solIndex][];
                        //for (int i = 0; i < solIndex; i++)
                        //{
                        //    newResult[i] = new double[this.param.Dimension];
                        //    for (int j = 0; j < this.param.Dimension; j++)
                        //    {
                        //        newResult[i][j] = sols[i][j];
                        //    }
                        //}
                        
                        return mylist.ToArray();
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
            for (int i = 0; i < this.param.Dimension; i++)
            {
                if (this.func(this.GetPositiveProbe(point, i)) < this.func(point))
                {
                    // шаг считается удачным
                    point = this.GetPositiveProbe(point, i);
                }
                else
                {
                    // шаг неудачен, делаем шаг в противоположном направлении
                    if (this.func(this.GetNegativeProbe(point, i)) < this.func(point))
                    {
                        // шаг в противоположном направлении считается удачным
                        point = this.GetNegativeProbe(point, i);
                    }
                    else
                    {
                        // оба шага неудачны
                        // y[i + 1] = y[i];
                    }
                }
            }

            return point;
        }

        private double[] ExploratarySearchExtended(double[] point)
        {
            for (int i = 0; i < this.param.Dimension; i++)
            {
                if (this.func(this.GetPositiveProbe(point, i)) < this.func(point))
                {
                    // шаг считается удачным
                    point = this.GetPositiveProbe(point, i);
                }
                else
                {
                    // шаг неудачен, делаем шаг в противоположном направлении
                    if (this.func(this.GetNegativeProbe(point, i)) < this.func(point))
                    {
                        // шаг в противоположном направлении считается удачным
                        point = this.GetNegativeProbe(point, i);
                    }
                    else
                    {
                        // оба шага неудачны
                        // y[i + 1] = y[i];
                    }
                }
            }

            return point;
        }

        /// <summary>
        /// Пробный шаг в положительном направлении.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <param name="i">Координата, по которой делаем шаг.</param>
        /// <returns>Новую точку.</returns>
        private double[] GetPositiveProbe(double[] point, int i)
        {
            double[] solution = new double[this.param.Dimension];
            for (int j = 0; j < this.param.Dimension; j++)
            {
                solution[j] = point[j];
            }

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
            double[] solution = new double[this.param.Dimension];
            for (int j = 0; j < this.param.Dimension; j++)
            {
                solution[j] = point[j];
            }

            solution[i] -= this.step[i];
            return solution;
        }

        /// <summary>
        /// Поиск по образцу. y[0] = x[k + 1] + param.AccelerateCoefficient * (x[k + 1] - x[k])
        /// </summary>
        /// <param name="oldBasis">The old basis.</param>
        /// <param name="basis">The basis.</param>
        /// <returns>Новую точку.</returns>
        private double[] PatternSearch(double[] oldBasis, double[] basis)
        {
            double[] solution = new double[this.param.Dimension];
            for (int index = 0; index < this.param.Dimension; index++)
            {
                solution[index] = basis[index] + (this.param.AccelerateCoefficient * (basis[index] - oldBasis[index]));
            }

            return solution;
        }

        /// <summary>
        /// Alls the steps less precision.
        /// </summary>
        /// <param name="precision">The precision.</param>
        /// <returns>True, если все</returns>
        private bool AllStepsLessPrecision(double precision)
        {
            for (int index = 0; index < this.param.Dimension; index++)
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
        /// <summary>
        /// Параметры метода.
        /// </summary>
        internal struct MethodParams
        {
            /// <summary>
            /// начальные величины шагов по координатным направлениям di,...,dn > 0
            /// </summary>
            public double[] Step;

            /// <summary>
            /// Количество перменных
            /// </summary>
            public int Dimension;

            /// <summary>
            /// ускоряющий множитель lyamda > 0
            /// </summary>
            public double AccelerateCoefficient;

            /// <summary>
            /// коэффициент уменьшения шага аlfa > 1.
            /// </summary>
            public double CoefficientReduction;
        }
        #endregion
    }
}