//-----------------------------------------------------------------------
// <copyright file="DeformablePolyhedron.cs" company="Home Corporation">
//     Copyright (c) Home Corporation 2009. All rights reserved.
// </copyright>
// <author>Sergii Pechenizkyi</author>
//-----------------------------------------------------------------------

namespace OptimizationMethods.ZerothOrder
{
    /// <summary>
    /// Ссылка на функциональную зависимость f(x1,х2,...,хn).
    /// </summary>
    /// <param name="x">Вектор значений переменных x1,х2,...,хn.</param>
    /// <returns>Значение функции у=f(x1,х2,...,хn).</returns>
    public delegate double ManyVariable(double[] x);

    /// <summary>
    /// Нахождение безусловного минимума функции многих переменных методом деформируемого многогранника
    /// </summary>
    /// -----------------------------------------------------------------------------------------------
    /// В основу метода деформируемого многогранника (метода Нелдера-Мида
    /// [J.A.Nelder, R.Mead]) положено построение последовательности систем n + 1
    /// точек хi(к), i = l,...,n + l , которые являются вершинами выпуклого
    /// многогранника. Точки системы хi(к +1), i = 1,...,n +1 на к +1 итерации совпадают с точками
    /// системых хi(к), i = 1,...,n + 1, кроме i = h, где точка xh(k)- наихудшая в системе
    /// хi(к), i = 1,...,n + 1, т.е. f(xh(k))= max f(xi(k)). Точка xh(k) заменяется на
    /// другую точку по специальным правилам, описанным ниже. В результате
    /// многогранники деформируются в зависимости от структуры линий уровня целевой
    /// функции, вытягиваясь вдоль длинных наклонных плоскостей, изменяя
    /// направление в изогнутых впадинах и сжимаясь в окрестности минимума. Построение
    /// последовательности многогранников заканчивается, когда значения функции
    /// в вершинах текущего многогранника отличаются от значения функции в центре
    /// тяжести системы xi(k), i = 1,... ,n +1; i!=h не более чем на е &gt; 0.
    /// ------------------------------------------------------------------------------------------------
    public static class DeformablePolyhedron
    {
        /// <summary>
        /// Нахождение безусловного минимума функции многих переменных.
        /// </summary>
        /// <param name="searchFunc">Ссылка на минимизируемую функцию.</param>
        /// <param name="funcParam">Параметры поиска.</param>
        /// <returns>Вектор значений х, при котором функция достигает минимума.</returns>
        public static Point GetMinimum(ManyVariable searchFunc, MethodParams funcParam)
        {
            Polyhedron workPolyhedron = new Polyhedron(funcParam);

            bool isStop = false;
            while (!isStop)
            {
                workPolyhedron.VertexSorting(searchFunc);

                workPolyhedron.CalculateCenterOfGravity();

                if (workPolyhedron.GetSigma(searchFunc) <= funcParam.Precision)
                {
                    isStop = true;
                    return workPolyhedron.BestVertex;
                }

                if (searchFunc(workPolyhedron.MirrorVertex.X) <= searchFunc(workPolyhedron.BestVertex.X))
                {
                    if (searchFunc(workPolyhedron.ExtensionVertex.X) < searchFunc(workPolyhedron.BestVertex.X))
                    {
                        workPolyhedron.WorstVertex = workPolyhedron.ExtensionVertex;
                        continue;
                    }
                    else
                    {
                        workPolyhedron.WorstVertex = workPolyhedron.MirrorVertex;
                        continue;
                    }
                }

                if (searchFunc(workPolyhedron.SecondBestVertex.X) < searchFunc(workPolyhedron.MirrorVertex.X) && searchFunc(workPolyhedron.MirrorVertex.X) <= searchFunc(workPolyhedron.WorstVertex.X))
                {
                    workPolyhedron.WorstVertex = workPolyhedron.CompressionVertex;
                    continue;
                }

                if (searchFunc(workPolyhedron.BestVertex.X) < searchFunc(workPolyhedron.MirrorVertex.X) && searchFunc(workPolyhedron.MirrorVertex.X) <= searchFunc(workPolyhedron.SecondBestVertex.X))
                {
                    workPolyhedron.WorstVertex = workPolyhedron.MirrorVertex;
                    continue;
                }

                if (searchFunc(workPolyhedron.MirrorVertex.X) > searchFunc(workPolyhedron.BestVertex.X))
                {
                    workPolyhedron.ReductionOperation();
                    continue;
                }
            }

            return workPolyhedron.BestVertex;
        }

        /// <summary>
        /// Передаваемые в метод параметры.
        /// </summary>
        public struct MethodParams
        {
            /// <summary>
            /// Коэфициент отражения.
            /// </summary>
            public double Alfa;

            /// <summary>
            /// Коэфициент сжатия.
            /// </summary>
            public double Beta;

            /// <summary>
            /// Коэфициент растяжения.
            /// </summary>
            public double Gamma;

            /// <summary>
            /// Число для остановки алгоритма.
            /// </summary>
            public double Precision;

            /// <summary>
            /// Количество переменных в минимизируемом уравнении.
            /// </summary>
            public int FuncDimension;

            /// <summary>
            /// Вершины начального многогранника.
            /// </summary>
            public Point[] StartPoint;
        }

        /// <summary>
        /// Точка (вершина) многогранника.
        /// </summary>
        public struct Point
        {
            #region Private Member Variables
            /// <summary>
            /// Набор переменных, описывающих точку (вершину).
            /// </summary>
            private double[] x;

            /// <summary>
            /// Количество переменных
            /// </summary>
            private int dimension;
            #endregion

            #region Constructors
            /// <summary>
            /// Initializes a new instance of the <see cref="Point"/> struct.
            /// </summary>
            /// <param name="dimension">The dimension.</param>
            public Point(int dimension)
            {
                this.x = new double[dimension];
                for (int i = 0; i < dimension; i++)
                {
                    this.x[i] = 0;
                }

                this.dimension = dimension;
            }
            #endregion

            #region Public Properties
            /// <summary>
            /// Gets or sets the X.
            /// </summary>
            /// <value>Набор переменных x, описывающих точку (вершину).</value>
            public double[] X
            {
                get { return this.x; }
                set { this.x = value; }
            }
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
                Point sum = new Point(leftOperand.dimension);
                for (int i = 0; i < leftOperand.dimension; i++)
                {
                    sum.x[i] = leftOperand.x[i] + rigthOperand.x[i];
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
                Point sub = new Point(leftOperand.dimension);
                for (int i = 0; i < leftOperand.dimension; i++)
                {
                    sub.x[i] = leftOperand.x[i] - rigthOperand.x[i];
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
                Point div = new Point(leftOperand.dimension);
                for (int i = 0; i < leftOperand.dimension; i++)
                {
                    div.x[i] = leftOperand.x[i] / number;
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
                Point mul = new Point(rigthOperand.dimension);
                for (int i = 0; i < rigthOperand.dimension; i++)
                {
                    mul.x[i] = rigthOperand.x[i] * number;
                }

                return mul;
            }

            /// <summary>
            /// Returns a <see cref="System.String"/> that represents this instance.
            /// </summary>
            /// <returns>
            /// A <see cref="System.String"/> that represents this instance.
            /// </returns>
            public override string ToString()
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder(this.x[0].ToString());

                for (int i = 1; i < this.dimension; i++)
                {
                    sb.Append(" ");
                    sb.Append(this.x[i].ToString());
                }

                return sb.ToString();
            }

            public double[] ToDouble()
            {
                return this.x;
            }
            #endregion
        }

        /// <summary>
        /// Абстракция многогранника.
        /// </summary>
        private class Polyhedron
        {
            #region Private Member Variables
            /// <summary>
            /// Коэфициент отражения.
            /// </summary>
            private readonly double alfa;

            /// <summary>
            /// Коэфициент сжатия.
            /// </summary>
            private readonly double beta;

            /// <summary>
            /// Коэфициент сжатия.
            /// </summary>
            private readonly double gama;

            /// <summary>
            /// Количество вершин многогранника.
            /// </summary>
            private readonly int dimension;

            /// <summary>
            /// Множество вершин многогранника.
            /// </summary>
            private Point[] vertex;

            /// <summary>
            /// Вершина, соответвующая центру тяжести многогранника.
            /// </summary>
            private Point centerOfGravity;
            #endregion

            #region Constructors
            /// <summary>
            /// Initializes a new instance of the <see cref="Polyhedron"/> class.
            /// </summary>
            /// <param name="initParam">The initial param.</param>
            public Polyhedron(MethodParams initParam)
            {
                this.alfa = initParam.Alfa;
                this.beta = initParam.Beta;
                this.gama = initParam.Gamma;
                this.dimension = initParam.FuncDimension + 1;
                this.vertex = initParam.StartPoint;
            }
            #endregion

            #region Public Properties
            /// <summary>
            /// Gets or sets the worst vertex (наихудшая вершина).
            /// </summary>
            /// <value>The worst vertex.</value>
            internal Point WorstVertex
            {
                get { return this.vertex[this.dimension - 1]; }
                set { this.vertex[this.dimension - 1] = value; }
            }

            /// <summary>
            /// Gets the best vertex (наилучшая вершина).
            /// </summary>
            /// <value>The best vertex.</value>
            internal Point BestVertex
            {
                get { return this.vertex[0]; }
            }

            /// <summary>
            /// Gets the second best vertex (вторая наилучшая вершина).
            /// </summary>
            /// <value>The second best vertex.</value>
            internal Point SecondBestVertex
            {
                get { return this.vertex[1]; }
            }

            /// <summary>
            /// Gets the mirror vertex (отраженная вершина).
            /// </summary>
            /// <value>The mirror vertex.</value>
            internal Point MirrorVertex
            {
                get { return this.centerOfGravity + (this.alfa * (this.centerOfGravity - this.WorstVertex)); }
            }

            /// <summary>
            /// Gets the extension vertex (растяжимая вершина).
            /// </summary>
            /// <value>The extension vertex.</value>
            internal Point ExtensionVertex
            {
                get { return this.centerOfGravity + (this.gama * (this.MirrorVertex - this.centerOfGravity)); }
            }

            /// <summary>
            /// Gets the compression vertex (сжатая вершина).
            /// </summary>
            /// <value>The compression vertex.</value>
            internal Point CompressionVertex
            {
                get { return this.centerOfGravity + (this.beta * (this.WorstVertex - this.centerOfGravity)); }
            }
            #endregion

            #region Public Methods
            /// <summary>
            /// Вычислить центр тяжести.
            /// </summary>
            internal void CalculateCenterOfGravity()
            {
                Point tmpval = new Point(this.dimension - 1);

                for (int i = 0; i < this.dimension - 1; i++)
                {
                    tmpval += this.vertex[i];
                }

                tmpval /= this.dimension - 1;
                this.centerOfGravity = tmpval;
            }

            /// <summary>
            /// Получить отличие значения функции в вершинах текущего многогранника от значений функции в центре тяжести.
            /// </summary>
            /// <param name="funcSgm">Ссылка на функцию многих переменных.</param>
            /// <returns>Значение сигмы.</returns>
            internal double GetSigma(ManyVariable funcSgm)
            {
                double tmpSigma = 0;
                double valueFuncInCentr = funcSgm(this.centerOfGravity.X);

                for (int i = 0; i < this.dimension; i++)
                {
                    tmpSigma += (funcSgm(this.vertex[i].X) - valueFuncInCentr) * (funcSgm(this.vertex[i].X) - valueFuncInCentr);
                }

                tmpSigma /= this.dimension;

                return System.Math.Sqrt(tmpSigma);
            }

            /// <summary>
            /// Выполнить операцию редукции.
            /// </summary>
            internal void ReductionOperation()
            {
                for (int i = 0; i < this.dimension; i++)
                {
                    this.vertex[i] = this.BestVertex + (0.5 * (this.vertex[i] - this.BestVertex));
                }
            }

            /// <summary>
            /// Сортировка вершин по близости к минимуму.
            /// </summary>
            /// <param name="mvf">Ссылка на функцию многих переменных.</param>
            internal void VertexSorting(ManyVariable mvf)
            {
                double[] functionValue = new double[this.dimension];
                int[] vertexNumber = new int[this.dimension];

                for (int i = 0; i < this.dimension; i++)
                {
                    vertexNumber[i] = i;
                    functionValue[i] = mvf(this.vertex[i].X);
                }

                System.Array.Sort(functionValue, vertexNumber);

                Point[] sortPoints = new Point[this.dimension];
                for (int i = 0; i < this.dimension; i++)
                {
                    sortPoints[i] = this.vertex[vertexNumber[i]];
                }

                this.vertex = sortPoints;
            }
            #endregion
        }
    }
}
