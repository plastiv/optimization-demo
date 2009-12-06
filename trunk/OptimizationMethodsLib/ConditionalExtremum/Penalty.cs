
namespace OptimizationMethods.ConditionalExtremum
{
    using OptimizationMethods.ZerothOrder;
    using System.Diagnostics;

    public class Penalty
    {
        #region Public Fields

        #endregion

        #region Private Fields
        private MethodParams param;
        #endregion

        #region Constructors
        public Penalty(MethodParams inputParam)
        {
            Debug.Assert(inputParam.Func != null, "Function is unexepectedly not create (null)");
            Debug.Assert(inputParam.incPenalty > 1, "InputParam.incrementPenalty is unexepectedly less or equal 1");
            Debug.Assert(inputParam.Penalty > 0, "Penalty is unexepectedly less or equal 0");
            Debug.Assert(inputParam.QuantityOfEqualities >= 0, "QuantityOfEqualities is unexepectedly less 0");
            Debug.Assert(inputParam.QuantityOfInequalities >= 0, "QuantityOfInequalities is unexepectedly less 0");

            param = inputParam;
        }
        #endregion

        #region Properties

        #endregion

        #region Public Methods
        /// <summary>
        /// Gets the minimum.
        /// </summary>
        /// <param name="startPoint">The start point.</param>
        /// <param name="precision">Малое число е > 0 для остановки алгоритма</param>
        /// <returns></returns>
        public double[] GetMinimum(double[] startPoint, double precision)
        {
            Debug.Assert(precision > 0, "Precision is unexepectedly less or equal zero");

            // Шаг 2. Составить вспомогательную функцию
            ManyVariable AuxiliaryFunction = delegate(double[] inputx)
            {
                return param.Func(inputx) + PenaltyFunction(inputx, param.Penalty);
            };

            Hooke_Jevees.MethodParams hjparam = new Hooke_Jevees.MethodParams();
            hjparam.AccelerateCoefficient = 1.5;
            hjparam.CoefficientReduction = 4;
            hjparam.Dimension = 2;
            hjparam.Step = new double[hjparam.Dimension];
            hjparam.Step[0] = 0.75;
            hjparam.Step[1] = 0.75;
            Hooke_Jevees hjmethod = new Hooke_Jevees(AuxiliaryFunction, hjparam);

            bool notFound = true;

            double[] xopt = startPoint; // искомая точка

            while (notFound)
            {
                // Шаг 3. Найти точку х*{гк} безусловного минимума функции flx9rk\ no x
                xopt = hjmethod.GetMinimum(xopt, precision);
                // Шаг 4. Проверить условие окончания:
                if (PenaltyFunction(xopt, param.Penalty) > precision)
                {
                    //  a) положить: r*+1 = Сгк, хк+х = х*(гк\ к = к + \ и перейти к шагу 2.
                    param.Penalty *= param.incPenalty;
                }
                else
                {
                    //  b) процесс поиска закончить
                    notFound = false;
                }
            }

            return xopt;
        }
        #endregion

        #region Private Methods
        private double QuadraticPenalty(double[] x)
        {
            if (param.QuantityOfEqualities != 0)
            {
                double solution = 0;

                for (int i = 0; i < param.QuantityOfEqualities; i++)
                {
                    solution += param.Equalities[i](x) * param.Equalities[i](x);
                }

                return solution;
            }
            else
            {
                return 0;
            }
        }

        private double SquareShear(double[] x)
        {
            if (param.QuantityOfInequalities != 0)
            {
                double solution = 0;

                for (int i = 0; i < param.QuantityOfInequalities; i++)
                {
                    if (param.Inequalities[i](x) <= 0)
                    {
                        solution += 0;
                    }
                    else
                    {
                        solution += param.Inequalities[i](x) * param.Inequalities[i](x);
                    }
                }

                return solution;
            }
            else
            {
                return 0;
            }
        }

        private double PenaltyFunction(double[] x, double r)
        {
            return (r / 2) * (QuadraticPenalty(x) + SquareShear(x));
        }
        #endregion

        #region Structs
        public struct MethodParams
        {
            /// <summary>
            /// Количество неравенств
            /// </summary>
            public int QuantityOfInequalities;
            
            /// <summary>
            /// Неравенства
            /// </summary>
            public ManyVariable[] Inequalities;

            /// <summary>
            /// Количество равенств
            /// </summary>
            public int QuantityOfEqualities;
            
            /// <summary>
            /// Равенства
            /// </summary>
            public ManyVariable[] Equalities;

            /// <summary>
            /// функция
            /// </summary>
            public ManyVariable Func;

            /// <summary>
            /// Начальное значение параметра штрафа r0 > 0
            /// </summary>
            public double Penalty;
            
            /// <summary>
            /// Число С > 1 для увеличения параметра
            /// </summary>
            public double incPenalty;
        }
        #endregion
    }
}
