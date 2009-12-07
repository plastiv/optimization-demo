//-----------------------------------------------------------------------
// <copyright file="DeformablePolyhedron.cs" company="Home Corporation">
//     Copyright (c) Home Corporation 2009. All rights reserved.
// </copyright>
// <author>Sergii Pechenizkyi</author>
//-----------------------------------------------------------------------

namespace OptimizationMethods.ZerothOrder
{
    using System.Diagnostics;

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
    public class DeformablePolyhedron
    {
        #region Public Fields

        #endregion

        #region Private Fields
        /// <summary>
        /// Параметры метода.
        /// </summary>
        private readonly MethodParams param;

        /// <summary>
        /// Минимизируемая функция.
        /// </summary>
        private readonly ManyVariable func;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="DeformablePolyhedron"/> class.
        /// </summary>
        /// <param name="inputFunc">The input func.</param>
        /// <param name="inputParams">The input params.</param>
        public DeformablePolyhedron(ManyVariable inputFunc, MethodParams inputParams)
        {
            Debug.Assert(inputParams.Alfa > 0, "Alfa coefficient is unexepectedly less or equal zero");
            Debug.Assert(inputParams.Beta > 0, "Alfa coefficient is unexepectedly less or equal zero");
            Debug.Assert(inputParams.Gamma > 0, "Alfa coefficient is unexepectedly less or equal zero");
            Debug.Assert(inputParams.Dimension > 1, "Dimension is unexepectedly less or equal 1");
            this.param = inputParams;

            Debug.Assert(inputFunc != null, "Input function reference is unexepectedly null");
            this.func = inputFunc;
        }
        #endregion

        #region Properties

        #endregion

        #region Public Methods
        /// <summary>
        /// Нахождение безусловного минимума функции многих переменных.
        /// </summary>
        /// <param name="startingPoint">The starting point.</param>
        /// <param name="precision">The precision.</param>
        /// <returns>
        /// Вектор значений х, при котором функция достигает минимума.
        /// </returns>
        public double[] GetMinimum(double[] startingPoint, double precision)
        {
            Debug.Assert(precision > 0, "Precision is unexepectedly less or equal zero");

            Polyhedron polyhedron = new Polyhedron(this.func, this.param, startingPoint);

            while (polyhedron.GetSigma() > precision)
            {
                if (this.func(polyhedron.MirrorVertex.X) <= this.func(polyhedron.BestVertex.X))
                {
                    if (this.func(polyhedron.ExtensionVertex.X) < this.func(polyhedron.BestVertex.X))
                    {
                        // выполним растяжение
                        polyhedron.WorstVertex = polyhedron.ExtensionVertex;
                    }
                    else
                    {
                        // выполним отражение
                        polyhedron.WorstVertex = polyhedron.MirrorVertex;
                    }
                }
                else if (this.func(polyhedron.SecondBestVertex.X) < this.func(polyhedron.MirrorVertex.X) && this.func(polyhedron.MirrorVertex.X) <= this.func(polyhedron.WorstVertex.X))
                {
                    // выполним сжатие
                    polyhedron.WorstVertex = polyhedron.CompressionVertex;
                }
                else if (this.func(polyhedron.BestVertex.X) < this.func(polyhedron.MirrorVertex.X) && this.func(polyhedron.MirrorVertex.X) <= this.func(polyhedron.SecondBestVertex.X))
                {
                    polyhedron.WorstVertex = polyhedron.MirrorVertex;
                }
                else if (this.func(polyhedron.MirrorVertex.X) > this.func(polyhedron.BestVertex.X))
                {
                    // выполним редукцию
                    polyhedron.ReductionOperation();
                }
            }

            return polyhedron.BestVertex.ToDouble();
        }
        #endregion

        #region Private Methods

        #endregion

        #region Structs
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
            /// Количество переменных в минимизируемом уравнении.
            /// </summary>
            public int Dimension;
        }

        /// <summary>
        /// Точка (вершина) многогранника.
        /// </summary>
        internal struct Point
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

            /// <summary>
            /// Toes the double.
            /// </summary>
            /// <returns> A <see cref="System.Double"/> that represents this instance.</returns>
            public double[] ToDouble()
            {
                return this.x;
            }
            #endregion
        }
        #endregion

        /// <summary>
        /// Абстракция многогранника.
        /// </summary>
        private class Polyhedron
        {
            #region Private Member Variables
            /// <summary>
            /// Параметры метода.
            /// </summary>
            private readonly MethodParams param;

            /// <summary>
            /// Значение ребра, на ктр будут смещены вершины начального многогранника относительно начальной точки.
            /// </summary>
            private const int InitEdgeValue = 1; // magic number

            /// <summary>
            /// Минимизируемая функция.
            /// </summary>
            private readonly ManyVariable func;

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
            /// <param name="inputFunc">The input func.</param>
            /// <param name="inputParams">The input params.</param>
            /// <param name="startingPoint">The starting point.</param>
            public Polyhedron(ManyVariable inputFunc, MethodParams inputParams, double[] startingPoint)
            {
                Debug.Assert(inputParams.Alfa > 0, "Alfa coefficient is unexepectedly less or equal zero");
                Debug.Assert(inputParams.Beta > 0, "Alfa coefficient is unexepectedly less or equal zero");
                Debug.Assert(inputParams.Gamma > 0, "Alfa coefficient is unexepectedly less or equal zero");
                Debug.Assert(inputParams.Dimension > 1, "Dimension is unexepectedly less or equal 1");
                this.param = inputParams;

                Debug.Assert(inputFunc != null, "Input function reference is unexepectedly null");
                this.func = inputFunc;

                this.vertex = this.GetStartingVertex(startingPoint);
            }
            #endregion

            #region Public Properties
            /// <summary>
            /// Gets or sets the worst vertex (наихудшая вершина).
            /// </summary>
            /// <value>The worst vertex.</value>
            internal Point WorstVertex
            {
                get { return this.vertex[this.param.Dimension]; }
                set { this.vertex[this.param.Dimension] = value; }
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
                get { return this.centerOfGravity + (this.param.Alfa * (this.centerOfGravity - this.WorstVertex)); }
            }

            /// <summary>
            /// Gets the extension vertex (растяжимая вершина).
            /// </summary>
            /// <value>The extension vertex.</value>
            internal Point ExtensionVertex
            {
                get { return this.centerOfGravity + (this.param.Gamma * (this.MirrorVertex - this.centerOfGravity)); }
            }

            /// <summary>
            /// Gets the compression vertex (сжатая вершина).
            /// </summary>
            /// <value>The compression vertex.</value>
            internal Point CompressionVertex
            {
                get { return this.centerOfGravity + (this.param.Beta * (this.WorstVertex - this.centerOfGravity)); }
            }
            #endregion

            #region Public Methods
            /// <summary>
            /// Получить отличие значения функции в вершинах текущего многогранника от значений функции в центре тяжести.
            /// </summary>
            /// <returns>Значение сигмы.</returns>
            internal double GetSigma()
            {
                this.VertexSorting();
                this.CalculateCenterOfGravity();

                double valueFuncInCentr = this.func(this.centerOfGravity.X);
                double sumAllVertex = 0;
                for (int i = 0; i < this.param.Dimension + 1; i++)
                {
                    sumAllVertex += (this.func(this.vertex[i].X) - valueFuncInCentr) * (this.func(this.vertex[i].X) - valueFuncInCentr);
                }

                sumAllVertex /= this.param.Dimension + 1;

                return System.Math.Sqrt(sumAllVertex);
            }

            /// <summary>
            /// Выполнить операцию редукции.
            /// </summary>
            internal void ReductionOperation()
            {
                for (int i = 0; i < this.param.Dimension + 1; i++)
                {
                    this.vertex[i] = this.BestVertex + (0.5 * (this.vertex[i] - this.BestVertex));
                }
            }

            #endregion

            #region Private Methods
            /// <summary>
            /// Сортировка вершин по близости к минимуму.
            /// </summary>
            private void VertexSorting()
            {
                double[] functionValue = new double[this.param.Dimension + 1];
                int[] vertexNumber = new int[this.param.Dimension + 1];

                for (int i = 0; i < this.param.Dimension + 1; i++)
                {
                    vertexNumber[i] = i;
                    functionValue[i] = this.func(this.vertex[i].X);
                }

                System.Array.Sort(functionValue, vertexNumber);

                Point[] sortPoints = new Point[this.param.Dimension + 1];
                for (int i = 0; i < this.param.Dimension + 1; i++)
                {
                    sortPoints[i] = this.vertex[vertexNumber[i]];
                }

                this.vertex = sortPoints;
            }

            /// <summary>
            /// Вычислить центр тяжести.
            /// </summary>
            private void CalculateCenterOfGravity()
            {
                Point tmpval = new Point(this.param.Dimension);

                for (int i = 0; i < this.param.Dimension; i++)
                {
                    tmpval += this.vertex[i];
                }

                tmpval /= this.param.Dimension;
                this.centerOfGravity = tmpval;
            }

            /// <summary>
            /// Gets the starting vertex.
            /// </summary>
            /// <param name="startingPoint">The starting point.</param>
            /// <returns>Значения вершин начального многогранника.</returns>
            private Point[] GetStartingVertex(double[] startingPoint)
            {
                Point[] startingVertex = new Point[this.param.Dimension + 1];
                startingVertex[0] = new Point(this.param.Dimension);
                for (int i = 0; i < this.param.Dimension; i++)
                {
                    startingVertex[0].X[i] = startingPoint[i];
                }

                for (int i = 1; i < this.param.Dimension + 1; i++)
                {
                    startingVertex[i] = new Point(this.param.Dimension);
                    for (int j = 0; j < this.param.Dimension; j++)
                    {
                        startingVertex[0].X[j] = startingPoint[j];
                    }

                    startingVertex[i].X[i - 1] += InitEdgeValue;
                }

                return startingVertex;
            }
            #endregion
        }
    }
}
