﻿using System.Windows;
using System.Windows.Media;
using Microsoft.Research.DynamicDataDisplay.Charts.Shapes;
using Microsoft.Research.DynamicDataDisplay.Common.Auxiliary;
using Microsoft.Research.DynamicDataDisplay.DataSources.MultiDimensional;
using Microsoft.Research.DynamicDataDisplay.PointMarkers;
using Optimization.Tests.Tasks;
using Microsoft.Research.DynamicDataDisplay.Charts;
using Microsoft.Research.DynamicDataDisplay.Charts.Navigation;
using System.Windows.Documents;
using System.Collections.Generic;
using Microsoft.Research.DynamicDataDisplay;

namespace Optimization.VisualApplication
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        readonly private int minValue = Optimization.VisualApplication.Properties.Settings.Default.MinValue;
        readonly private int maxValue = Optimization.VisualApplication.Properties.Settings.Default.MaxValue;
        readonly private int pointCount = Optimization.VisualApplication.Properties.Settings.Default.PointCount;

        //int solPointIndex;

        //LineSource lineSource;
        WarpedDataSource2D<double> warpedDataSource2D;

        Queue<MethodLine> methodLines;
        IsolineTrackingGraph trackingGraph;
        CursorCoordinateGraph cursorCoordinateGraph;

        public Window1()
        {
            InitializeComponent();

            //solPointIndex = 0;

            methodLines = new Queue<MethodLine>();
            cursorCoordinateGraph = new CursorCoordinateGraph();

            Loaded += new RoutedEventHandler(Window1_Loaded);
        }

        private void Window1_Loaded(object sender, RoutedEventArgs e)
        {
            cmbMethods.Items.Add(LineSource.Methods.Gradient);
            cmbMethods.Items.Add(LineSource.Methods.Hooke_Jeves);

            cmbFunctions.Items.Add(new ManyVariableFunctionTask0());
            cmbFunctions.Items.Add(new ManyVariableFunctionTask1());
            cmbFunctions.Items.Add(new ManyVariableFunctionTask2());
            cmbFunctions.Items.Add(new ManyVariableFunctionTask3());
            cmbFunctions.Items.Add(new ManyVariableFunctionTask4());
            cmbFunctions.Items.Add(new ManyVariableFunctionTask5());
            cmbFunctions.Items.Add(new ManyVariableFunctionTask6());
            cmbFunctions.Items.Add(new ManyVariableFunctionTask7());
            cmbFunctions.Items.Add(new ManyVariableFunctionTask8());
            cmbFunctions.Items.Add(new ManyVariableFunctionTask9());
            cmbFunctions.Items.Add(new ManyVariableFunctionTask10());
        }

        private void cmbFunctions_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            while (methodLines.Count != 0)
            {
                plotter.Children.Remove(methodLines.Dequeue().ViewpontPolyline);
            }

            ManyVariableFunctionTask selectedTask = (ManyVariableFunctionTask)cmbFunctions.SelectedItem;
            txtFunction.Text = selectedTask.expression;
            //lineSource = new LineSource(selectedTask.function);
            txtX1.Text = selectedTask.startPoint[0].ToString();
            txtX2.Text = selectedTask.startPoint[1].ToString();
            warpedDataSource2D = IsolineSource.GetWarpedDataSource2D(selectedTask.function, minValue, maxValue, pointCount);
            isolineGraph.DataSource = warpedDataSource2D;
            trackingGraph = new IsolineTrackingGraph(); // TODO: Lazy initialization.
            trackingGraph.DataSource = warpedDataSource2D;
            Rect visible = warpedDataSource2D.GetGridBounds();
            plotter.Viewport.Visible = visible;
        }

        private void btnAddLine_Click(object sender, RoutedEventArgs e)
        {
            MethodLine tempMethodLine = new MethodLine((ManyVariableFunctionTask)cmbFunctions.SelectedItem,cmbMethods.SelectedItem,new double[2] { double.Parse(txtX1.Text), double.Parse(txtX2.Text) });
            methodLines.Enqueue(tempMethodLine);

            plotter.AddChild(tempMethodLine.ViewpontPolyline);
        }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            if (methodLines.Count != 0)
            {
                plotter.Children.Remove(methodLines.Dequeue().ViewpontPolyline);
            }
        }

        private void btnForward_Click(object sender, RoutedEventArgs e)
        {
            methodLines.Peek().AddPoint();
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            methodLines.Peek().RemovePoint();
        }

        private void chkTrackingGraph_Click(object sender, RoutedEventArgs e)
        {
            if (chkTrackingGraph.IsChecked == true)
            {
                plotter.AddChild(trackingGraph);
            }
            else
            {
                plotter.Children.Remove(trackingGraph);
            }
        }

        private void chkCursorCoordinateGraph_Click(object sender, RoutedEventArgs e)
        {
            if (chkCursorCoordinateGraph.IsChecked == true)
            {
                plotter.AddChild(cursorCoordinateGraph);
            }
            else
            {
                plotter.Children.Remove(cursorCoordinateGraph);
            }
        }

        private void MenuItemIsoline_Click(object sender, RoutedEventArgs e)
        {

            MessageBox.Show("Menu item clicked");

        }
    }
}
