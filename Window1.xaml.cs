//-----------------------------------------------------------------------
// <copyright file="Window1.xaml.cs" company="Home Corporation">
//     Copyright (c) Home Corporation 2010. All rights reserved.
// </copyright>
// <author>Sergii Pechenizkyi</author>
//-----------------------------------------------------------------------

namespace Optimization.VisualApplication
{
    using System.Collections.Generic;
    using System.Windows;
    using Microsoft.Research.DynamicDataDisplay.Charts;
    using Microsoft.Research.DynamicDataDisplay.Charts.Navigation;
    using Microsoft.Research.DynamicDataDisplay.Common.Auxiliary;
    using Microsoft.Research.DynamicDataDisplay.DataSources.MultiDimensional;
    using Optimization.Tests.Tasks;
    using System.Collections.ObjectModel;
    using System.Windows.Controls;
    using System.Windows.Data;

    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        #region Public Fields

        #endregion

        #region Private Fields
        private readonly int minValue = Optimization.VisualApplication.Properties.Settings.Default.MinValue;
        private readonly int maxValue = Optimization.VisualApplication.Properties.Settings.Default.MaxValue;
        private readonly int pointCount = Optimization.VisualApplication.Properties.Settings.Default.PointCount;

        private WarpedDataSource2D<double> warpedDataSource2D;

        private Queue<MethodLine> methodLines;
        private IsolineTrackingGraph trackingGraph;
        private CursorCoordinateGraph cursorCoordinateGraph;
        #endregion

        #region Constructors
        public Window1()
        {
            InitializeComponent();

            methodLines = new Queue<MethodLine>();
            cursorCoordinateGraph = new CursorCoordinateGraph();

            Loaded += new RoutedEventHandler(Window1_Loaded);
        }
        #endregion

        #region Properties

        #endregion

        #region Public Methods

        #endregion

        #region Private Methods
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

            SetDataToListView();
        }

        private void SetDataToListView()
        {
            GridView myGridView = new GridView();

            GridViewColumn gvc1 = new GridViewColumn();
            gvc1.DisplayMemberBinding = new Binding("Iteration");
            gvc1.Header = "Iteration";
            gvc1.Width = 50;
            myGridView.Columns.Add(gvc1);
            GridViewColumn gvc2 = new GridViewColumn();
            gvc2.DisplayMemberBinding = new Binding("PointX");
            gvc2.Header = "Point X";
            gvc2.Width = 150;
            myGridView.Columns.Add(gvc2);
            GridViewColumn gvc3 = new GridViewColumn();
            gvc3.DisplayMemberBinding = new Binding("PointY");
            gvc3.Header = "Point Y";
            gvc3.Width = 150;
            myGridView.Columns.Add(gvc3);
            GridViewColumn gvc4 = new GridViewColumn();
            gvc4.DisplayMemberBinding = new Binding("FuncValue");
            gvc4.Header = "Func Value";
            gvc4.Width = 150;
            myGridView.Columns.Add(gvc4);

            //ItemsSource is ObservableCollection of EmployeeInfo objects
            listView.View = myGridView;
        }

        private void cmbFunctions_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            while (methodLines.Count != 0)
            {
                plotter.Children.Remove(methodLines.Dequeue().ViewpontPolyline);
            }

            ManyVariableFunctionTask selectedTask = (ManyVariableFunctionTask)cmbFunctions.SelectedItem;
            txtFunction.Text = selectedTask.expression;
            txtX1.Text = selectedTask.startPoint[0].ToString();
            txtX2.Text = selectedTask.startPoint[1].ToString();

            warpedDataSource2D = IsolineSource.GetWarpedDataSource2D(selectedTask.function, minValue, maxValue, pointCount);
            isolineGraph.DataSource = warpedDataSource2D;
            plotter.Viewport.Visible = warpedDataSource2D.GetGridBounds();
        }

        private void btnAddLine_Click(object sender, RoutedEventArgs e)
        {
            MethodLine tempMethodLine = new MethodLine((ManyVariableFunctionTask)cmbFunctions.SelectedItem, cmbMethods.SelectedItem, new double[2] { double.Parse(txtX1.Text), double.Parse(txtX2.Text) });
            methodLines.Enqueue(tempMethodLine);
            plotter.AddChild(tempMethodLine.ViewpontPolyline);

            listView.ItemsSource = tempMethodLine.GetReports();
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
                trackingGraph = new IsolineTrackingGraph(); // TODO: Lazy initialization.
                trackingGraph.DataSource = warpedDataSource2D;
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

        #region MenuClicks
        private void InDevelope_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("В разработке.");
        }
        #endregion

        
        #endregion

        #region Structs

        #endregion
    }
}
