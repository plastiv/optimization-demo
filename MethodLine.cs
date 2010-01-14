using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Optimization.Tests.Tasks;
using Microsoft.Research.DynamicDataDisplay.Charts.Shapes;
using Microsoft.Research.DynamicDataDisplay;

namespace Optimization.VisualApplication
{
    internal class MethodLine
    {
        LineSource lineSource;
        ViewportPolyline viewpontPolyline;
        int CurrMaxPointIndex;

        public ViewportPolyline ViewpontPolyline
        {
            get { return viewpontPolyline; }
        }

        public MethodLine(ManyVariableFunctionTask selectedTask, object methodIndex, double[] startingPoint)
        {
            lineSource = new LineSource(selectedTask.function);
            viewpontPolyline = new ViewportPolyline();
            viewpontPolyline.Points = lineSource.GetPointCollection(methodIndex, startingPoint);
            viewpontPolyline.Stroke = ColorHelper.RandomBrush;
            CurrMaxPointIndex = lineSource.PointsCount;
        }

        public void AddPoint()
        {
            if (CurrMaxPointIndex < lineSource.PointsCount)
            {
                viewpontPolyline.Points.Add(lineSource.GetPointAt(CurrMaxPointIndex));
                CurrMaxPointIndex++;
            }
        }

        public void RemovePoint()
        {
            if (CurrMaxPointIndex > 0)
            {
                viewpontPolyline.Points.RemoveAt(CurrMaxPointIndex - 1);
                CurrMaxPointIndex--;
            }
        }
    }
}
