using System;
using System.Collections.Generic;
using System.Text;
using OptimizationMethods.ZerothOrder.OneVariable;


namespace OptimizationMethods.ConditionalExtremum
{
    public class PenaltyOneDimension
    {
        private readonly OneVariableFunction[] quadraticPens;
        private readonly int m;
        OneVariableFunction Myovf;
        private readonly OneVariableFunction[] squareShearss;
        private readonly int p;
        private readonly double startPoint;
        private double paramPenalty;
        private readonly double incrementParamPenalty;
        private readonly double precision;

        double QuadraticPenalty(double x)
        {
            if (m!=0)
            {
                double solution = quadraticPens[0](x) * quadraticPens[0](x);

                for (int i = 1; i < m; i++)
                {
                    solution += quadraticPens[i](x) * quadraticPens[i](x);
                }
                return solution;
            }
            return 0;
        }

        double SquareShear(double x)
        {
            if (p != 0)
            {
                if (x <= 0)
                {
                    return 0;
                }
                else
                {
                    double solution = squareShearss[0](x) * squareShearss[0](x);

                    for (int i = 1; i < p; i++)
                    {
                        solution += squareShearss[i](x) * squareShearss[i](x);
                    }
                    return solution;
                }
            }
            return 0;
        }

        private double PenaltyFunction(double x, double r)
        {
            return (r / 2) * (QuadraticPenalty(x) + SquareShear(x));
        }

        public double GetMinimum()
        {
            // искомая точка
            double xopt = startPoint;

            // Шаг 2. Составить вспомогательную функцию
            OneVariableFunction BigFunction = delegate(double inputx)
            {
                return Myovf(inputx) + PenaltyFunction(inputx, paramPenalty);
            };

            bool isBigger = true;

            do
            {
                // Шаг 3. Найти точку х*{гк} безусловного минимума функции flx9rk\ no x
                xopt = OptimizationMethods.ZerothOrder.OneVariable.Fibonacci.GetMinimum(BigFunction, 0, 2, 0.1);
                // Шаг 4. Проверить условие окончания:
                if (PenaltyFunction(xopt,paramPenalty)<=precision)
                {
                    //  а) процесс поиска закончить
                    isBigger = false;
                }
                else
                {
                    //  б) положить: r*+1 = Сгк, хк+х = х*(гк\ к = к + \ и перейти к шагу 2.
                    paramPenalty *= incrementParamPenalty;
                }
                
            } while (isBigger);

            return xopt;
        }

        public PenaltyOneDimension(MethodParams mp)
        {
            p = mp.QuantityOfInequalities;
            m = mp.QuantityOfEqualities;
            Myovf = mp.OneVariableFunc;
            quadraticPens = mp.Equalities;
            squareShearss = mp.Inequalities;
            startPoint = mp.startPoint;
            paramPenalty = mp.paramPenalty;
            incrementParamPenalty = mp.incrementParamPenalty;
            precision = mp.precision;
        }

        public struct MethodParams
        {
            // Количество неравенств
            public int QuantityOfInequalities;
            // Неравенства
            public OneVariableFunction[] Inequalities;

            // Количество равенств
            public int QuantityOfEqualities;
            // равенства
            public OneVariableFunction[] Equalities;

            // функция
            public OneVariableFunction OneVariableFunc;

            // Задать начальную точку х°
            public double startPoint;
            // начальное значение параметра штрафа г0 > 0
            public double paramPenalty;
            // число С > 1 для увеличения параметра
            public double incrementParamPenalty;
            // малое число е > 0 для остановки алгоритма.
            public double precision;
        }
    }
}
