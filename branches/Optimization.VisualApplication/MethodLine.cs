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
        #region Public Fields

        #endregion

        #region Private Fields
        LineSource lineSource;
        ViewportPolyline viewpontPolyline;
        int currMaxPointIndex;
        #endregion

        #region Constructors
        public MethodLine(ManyVariableFunctionTask selectedTask, object methodIndex, double[] startingPoint)
        {
            lineSource = new LineSource(selectedTask.function);
            viewpontPolyline = new ViewportPolyline();
            viewpontPolyline.Points = lineSource.GetPointCollection(methodIndex, startingPoint);
            viewpontPolyline.Stroke = ColorHelper.RandomBrush;
            currMaxPointIndex = lineSource.PointsCount;
        }
        #endregion

        #region Properties
        internal int CurrMaxPointIndex
        {
            get { return currMaxPointIndex - 1; }
        }

        internal ViewportPolyline ViewpontPolyline
        {
            get { return viewpontPolyline; }
        }
        #endregion

        #region Public Methods
        internal ObservableCollection<Report> GetReports()
        {
            ObservableCollection<Report> result = new ObservableCollection<Report>();

            for (int i = 0; i < lineSource.PointsCount; i++)
            {
                result.Add(new Report { Iteration = i, PointX = lineSource.Solutions[i][0], PointY = lineSource.Solutions[i][1], FuncValue = lineSource.Function(lineSource.Solutions[i]) });
            }

            return result;
        }

        internal void AddPoint()
        {
            if (currMaxPointIndex < lineSource.PointsCount)
            {
                viewpontPolyline.Points.Add(lineSource.GetPointAt(currMaxPointIndex));
                currMaxPointIndex++;
            }
        }

        internal void RemovePoint()
        {
            if (currMaxPointIndex > 0)
            {
                viewpontPolyline.Points.RemoveAt(currMaxPointIndex - 1);
                currMaxPointIndex--;
            }
        }

        internal void Reset()
        {
            while (currMaxPointIndex > 1)
            {
                viewpontPolyline.Points.RemoveAt(currMaxPointIndex - 1);
                currMaxPointIndex--;
            }
        }
        #endregion

        #region Private Methods

        #endregion

        #region Structs

        #endregion
    }
}
