
namespace OptimizationMethods.ZerothOrder
{
    using OptimizationMethods.ZerothOrder;
    using System.Diagnostics;

    /// <summary>
    /// Нахождение безусловного минимума функции многих переменных методом Хука-Дживса
    /// </summary>
    public class Hooke_Jevees
    {
        #region Private Fields
        private ManyVariable func;
        private MethodParams param;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="Hooke_Jevees"/> class.
        /// </summary>
        /// <param name="inputFunc">The input function.</param>
        /// <param name="inputParams">The input method parameters.</param>
        public Hooke_Jevees(OptimizationMethods.ZerothOrder.ManyVariable inputFunc, MethodParams inputParams)
        {
            for (int i = 0; i < inputParams.Dimension; i++)
            {
                Debug.Assert(inputParams.Step[i] > 0, "Step value is unexepectedly less or equal zero");
            }

            Debug.Assert(inputParams.AccelerateCoefficient > 0, "Accelerate coefficient lyamda is unexepectedly less or equal zero");
            Debug.Assert(inputParams.CoefficientReduction > 1, "Coefficient reduction alfa is unexepectedly less or equal 1");
            Debug.Assert(inputParams.Dimension > 1, "Dimension is unexepectedly less or equal 1");
            param = inputParams;

            Debug.Assert(inputFunc != null, "Input function reference is unexepectedly null");
            func = inputFunc;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Gets the minimum.
        /// </summary>
        /// <param name="startPoint">The start point.</param>
        /// <param name="precision">The precision.</param>
        /// <returns></returns>
        public double[] GetMinimum(double[] startPoint, double precision)
        {
            // Шаг 1. Задать начальную точку л:0
            // число е>0 для остановки алгоритма
            Debug.Assert(precision > 0, "Precision is unexepectedly less or equal zero");

            double[] newBasis = startPoint;
            double[] oldBasis = startPoint;

            while (true)
            {
                //Шаг 2. Осуществить исследующий поиск по выбранному координатному направлению (i)
                newBasis = ExploratarySearch(newBasis);
                // Проверить успешность исследующего поиска:
                if (func(newBasis) < func(oldBasis))
                {
                    // перейти к шагу 4;

                    // Шаг 4. Провести поиск по образцу. Положить xk+l = yn+l,
                    oldBasis = newBasis;
                    // y[0] = x[k + 1] + param.AccelerateCoefficient * (x[k + 1] - x[k]);
                    newBasis = PatternSearch(oldBasis);
                    // перейти к шагу 2.
                    continue;
                }
                else
                {
                    // перейти к шагу 5.

                    // Шаг 5. Проверить условие окончания:
                    if (!AllStepsLessPrecision(precision))
                    {
                        for (int index = 0; index < param.Dimension; index++)
                        {
                            // Для значений шагов, больших точности
                            if (param.Step[index] > precision)
                            {
                                // Уменьшить величину шага
                                param.Step[index] /= param.CoefficientReduction;
                            }
                        }

                        newBasis = oldBasis;
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
        private double[] ExploratarySearch(double[] point)
        {
            for (int i = 0; i < param.Dimension; i++)
            {
                if (func(GetPositiveProbe(point, i)) < func(point))
                {
                    // шаг считается удачным
                    point = GetPositiveProbe(point, i);
                }
                else
                {
                    // шаг неудачен, делаем шаг в противоположном направлении
                    if (func(GetNegativeProbe(point, i)) < func(point))
                    {
                        // шаг в противоположном направлении считается удачным
                        point = GetNegativeProbe(point, i);
                    }
                    else
                    {
                        // оба шага неудачны
                        //y[i + 1] = y[i];
                    }
                }
            }

            return point;
        }

        private double[] GetPositiveProbe(double[] y, int i)
        {
            double[] solution = new double[param.Dimension];
            for (int j = 0; j < param.Dimension; j++)
            {
                solution[j] = y[j];
            }

            solution[i] += param.Step[i];
            return solution;
        }

        private double[] GetNegativeProbe(double[] y, int i)
        {
            double[] solution = new double[param.Dimension];
            for (int j = 0; j < param.Dimension; j++)
            {
                solution[j] = y[j];
            }

            solution[i] -= param.Step[i];
            return solution;
        }

        private double[] PatternSearch(double[] basis)
        {
            double[] solution = new double[param.Dimension];
            for (int index = 0; index < param.Dimension; index++)
            {
                solution[index] = basis[index] + param.AccelerateCoefficient * (basis[index] - basis[index]);
            }

            return solution;
        }

        private bool AllStepsLessPrecision(double precision)
        {
            for (int index = 0; index < param.Dimension; index++)
            {
                if (param.Step[index] > precision)
                {
                    return false;
                }
            }

            return true;
        }
        #endregion

        #region Structs
        public struct MethodParams
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