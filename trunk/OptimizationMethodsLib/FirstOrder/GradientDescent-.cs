/*

namespace OptimizationMethods.FirstOrder
{
    public delegate double GetManyVariableFunctionValue(double[] x);
    public delegate double[] GetGradientOfFunction(double[] x);

    /// <summary>
    /// Метод градиентного спуска с постоянным шагом
    /// </summary>
    class GradientDescent
    {
        private readonly int maxIteration;

        double[] de_dxi;

        double err;
        double h;
        int j;

        GetManyVariableFunctionValue searchFunc;
        GetGradientOfFunction searchGradient;
        
        private int dimension;

        public GradientDescent()
        {
            maxIteration = 60;
        }

        public double[] GetMinimum(double[] point, double sigma, double epsilon)
        {
            h = 1; // rename
            err = 1; // rename
            int count = 0;

            while (count < maxIteration && (h > sigma || err > epsilon))
            {
                de_dxi = searchGradient(point);
                point = QMin(de_dxi, point, epsilon, sigma);
                count = count + j + 1;
            }

            return point;
        }

        /*
        private double[] QMin(double[] de_dxi, double[] p, double epsilon, double sigma)
        {
            int cond = 0;
            int jmax = 60;
            double z0 = searchFunc(p);
            double[] minPoint = new double[dimension];
            double[] point1 = new double[dimension];
            double[] point2 = new double[dimension];
            double yMin;

            for (int i = 0; i < dimension; i++)
                point1[i] = p[i] + h * de_dxi[i];

            double y1 = searchFunc(point1);

            for (int i = 0; i < dimension; i++)
                point2[i] = p[i] + 2 * h * de_dxi[i];

            double y2 = searchFunc(point2);

            j = 0;

            while (j < jmax && cond == 0)
            {
                if (z0 <= y1)
                {
                    for (int i = 0; i < dimension; i++)
                        point2[i] = point1[i];

                    y2 = y1;
                    h = h / 2;

                    for (int i = 0; i < dimension; i++)
                        point1[i] = p[i] + h * de_dxi[i];

                    y1 = searchFunc(point1);
                }
                else if (y2 < y1)
                {
                    for (int i = 0; i < dimension; i++)
                        point1[i] = point2[i];

                    y1 = y2;

                    h = h * 2;

                    for (int i = 0; i < dimension; i++)
                        point2[i] = p[i] + 2 * h * de_dxi[i];

                    y2 = searchFunc(point2);
                }
                else
                {
                    cond = -1;
                }

                j = j + 1;
                if (h < sigma)
                    cond = 1;
            }

            double hMin = (h / 2) * (4 * y1 - 3 * z0 - y2) / (2 * y1 - z0 - y2);

            for (int i = 0; i < dimension; i++)
            {
                minPoint[i] = p[i] + hMin * de_dxi[i];
            }

            yMin = searchFunc(minPoint);

            double h0 = System.Math.Abs(hMin);
            double h1 = System.Math.Abs(hMin - h);
            double h2 = System.Math.Abs(hMin - 2 * h);

            if (h0 < h)
                h = h0;
            if (h1 < h)
                h = h1;
            if (h2 < h)
                h = h2;
            if (h == 0)
                h = hMin;
            if (h < sigma)
                cond = 1;
            double e0 = System.Math.Abs(z0 - yMin);
            double e1 = System.Math.Abs(y1 - yMin);
            double e2 = System.Math.Abs(y2 - yMin);

            if (e0 != 0 && e0 < err)
                err = e0;
            if (e1 != 0 && e1 < err)
                err = e1;
            if (e2 != 0 && e2 < err)
                err = e2;
            if (e0 == 0 && e1 == 0 && e2 == 0)
                err = 0;
            if (err < epsilon)
                cond = 2;

            return minPoint;
        }
         
    }
}
*/