using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Optimization.Tests.Tasks;
using Microsoft.Research.DynamicDataDisplay.Charts.Shapes;
using Microsoft.Research.DynamicDataDisplay;
using System.Collections.ObjectModel;

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

        internal ObservableCollection<Report> GetReports()
        {
            ObservableCollection<Report> result = new ObservableCollection<Report>();

            for (int i = 0; i < lineSource.PointsCount; i++)
            {
                result.Add(new Report { Iteration = i, PointX = lineSource.Solutions[i][0], PointY = lineSource.Solutions[i][1], FuncValue = lineSource.Function(lineSource.Solutions[i])});
            }

            return result;
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

        public void Reset()
        {
            while (CurrMaxPointIndex != 1)
            {
                viewpontPolyline.Points.RemoveAt(CurrMaxPointIndex - 1);
                CurrMaxPointIndex--;
            }
        }
    }
}
