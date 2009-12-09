using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace OptimizationMethods.ZerothOrder
{
    public class VisualDeformablePolyhedron : DeformablePolyhedron
    {
        #region Public Fields

        #endregion

        #region Private Fields

        private Polyhedron polyhedron;

        double precision;
        public bool found;
        public double[][] startPoints;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="DeformablePolyhedron"/> class.
        /// </summary>
        /// <param name="inputFunc">The input func.</param>
        /// <param name="inputParams">The input params.</param>
        public VisualDeformablePolyhedron(ManyVariable inputFunc, MethodParams inputParams, double[] startingPoint, double precision) : base (inputFunc,inputParams)
        {
            Debug.Assert(precision > 0, "Precision is unexepectedly less or equal zero");
            this.precision = precision;

            this.polyhedron = new DeformablePolyhedron.Polyhedron(inputFunc, inputParams, startingPoint);
            found = false;

            startPoints = new double[param.Dimension + 1][];
            for (int i = 0; i < param.Dimension + 1; i++)
            {
                startPoints[i] = polyhedron.vertex[i].X;
            }
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
        public double[][] GetIterResult()
        {
            double[][] solutions = new double[param.Dimension + 1][];
            for (int i = 0; i < param.Dimension + 1; i++)
            {
                solutions[i] = new double[param.Dimension];
            }

            if (this.polyhedron.GetSigma() > precision)
            {
                if (this.func(this.polyhedron.MirrorVertex.X) <= this.func(polyhedron.BestVertex.X))
                {
                    if (this.func(this.polyhedron.ExtensionVertex.X) < this.func(this.polyhedron.BestVertex.X))
                    {
                        // выполним растяжение
                        this.polyhedron.WorstVertex = this.polyhedron.ExtensionVertex;
                    }
                    else
                    {
                        // выполним отражение
                        this.polyhedron.WorstVertex = this.polyhedron.MirrorVertex;
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
            else
            {
                found = true;
            }

            for (int i = 0; i < param.Dimension + 1; i++)
            {
                solutions[i] = polyhedron.vertex[i].X;
            }
            return solutions;
        }
        #endregion

        #region Private Methods

        #endregion
    }
}
