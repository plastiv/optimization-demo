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
        private int minValue = Optimization.VisualApplication.Properties.Settings.Default.MinValue;
        private int maxValue = Optimization.VisualApplication.Properties.Settings.Default.MaxValue;
        private int pointCount = Optimization.VisualApplication.Properties.Settings.Default.PointCount;

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

            listViewReport.View = myGridView;
        }

        private void cmbFunctions_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            while (methodLines.Count != 0)
            {
                plotter.Children.Remove(methodLines.Dequeue().ViewpontPolyline);
            }

            lblStepCount.Content = "Step count: 0";
            lblStepIndex.Content = "Step index: 0";

            ManyVariableFunctionTask selectedTask = (ManyVariableFunctionTask)cmbFunctions.SelectedItem;
            txtFunction.Text = selectedTask.expression;
            txtX1.Text = selectedTask.startPoint[0].ToString();
            txtX2.Text = selectedTask.startPoint[1].ToString();

            DrawIsolineGraph();
        }

        private void DrawIsolineGraph()
        {
            warpedDataSource2D = IsolineSource.GetWarpedDataSource2D(((ManyVariableFunctionTask)cmbFunctions.SelectedItem).function, minValue, maxValue, pointCount);
            isolineGraph.DataSource = warpedDataSource2D;
            plotter.Viewport.Visible = warpedDataSource2D.GetGridBounds();
        }

        #region Button Clicks
        private void btnAddLine_Click(object sender, RoutedEventArgs e)
        {
            MethodLine tempMethodLine = new MethodLine((ManyVariableFunctionTask)cmbFunctions.SelectedItem, cmbMethods.SelectedItem, new double[2] { double.Parse(txtX1.Text), double.Parse(txtX2.Text) });
            methodLines.Enqueue(tempMethodLine);
            plotter.AddChild(tempMethodLine.ViewpontPolyline);

            listViewReport.ItemsSource = tempMethodLine.GetReports();
            lblStepCount.Content = "Step count: " + methodLines.Peek().CurrMaxPointIndex;
            lblStepIndex.Content = "Step index: " + methodLines.Peek().CurrMaxPointIndex;
        }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            if (methodLines.Count != 0)
            {
                plotter.Children.Remove(methodLines.Dequeue().ViewpontPolyline);
                lblStepCount.Content = "Step count: 0";
                lblStepIndex.Content = "Step index: 0";
            }
            else
            {
                MessageBox.Show("Добавьте линию сначала.");
            }
        }

        private void btnForward_Click(object sender, RoutedEventArgs e)
        {
            if (methodLines.Count != 0)
            {
                methodLines.Peek().AddPoint();
                lblStepIndex.Content = "Step index: " + methodLines.Peek().CurrMaxPointIndex;
            }
            else
            {
                MessageBox.Show("Добавьте линию сначала.");
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            if (methodLines.Count != 0)
            {
                methodLines.Peek().RemovePoint();
                lblStepIndex.Content = "Step index: " + methodLines.Peek().CurrMaxPointIndex;
            }
            else
            {
                MessageBox.Show("Добавьте линию сначала.");
            }
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            if (methodLines.Count != 0)
            {
                methodLines.Peek().Reset();
                lblStepIndex.Content = "Step index: " + methodLines.Peek().CurrMaxPointIndex;
            }
            else
            {
                MessageBox.Show("Добавьте линию сначала.");
            }
        }
        #endregion

        #region Menu Clicks
        private void menuItemTrackingGraph_Click(object sender, RoutedEventArgs e)
        {
            if (menuItemTrackingGraph.IsChecked == true)
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

        private void menuItemCursorCoordinateGraph_Click(object sender, RoutedEventArgs e)
        {
            if (menuItemCursorCoordinateGraph.IsChecked == true)
            {
                plotter.AddChild(cursorCoordinateGraph);
            }
            else
            {
                plotter.Children.Remove(cursorCoordinateGraph);
            }
        }

        private void InDevelope_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("В разработке.");
        }

        private void menuItemIsolineSettings_Click(object sender, RoutedEventArgs e)
        {
            IsolineSettingsWindow isolineSettings = new IsolineSettingsWindow();
            isolineSettings.Owner = this;
            isolineSettings.IsolineSetting = new IsolineSetting(this.minValue, this.maxValue, this.pointCount);
            isolineSettings.ShowDialog();

            if (isolineSettings.DialogResult == true)
            {
                this.minValue = isolineSettings.IsolineSetting.Minimum;
                this.maxValue = isolineSettings.IsolineSetting.Maximum;
                this.pointCount = isolineSettings.IsolineSetting.Count;
                DrawIsolineGraph();
            }
        }
        #endregion

        #endregion

        #region Structs

        #endregion
    }

    internal struct IsolineSetting
    {
        private int minimum;
        private int maximum;
        private int count;

        public IsolineSetting(int minimum, int maximum, int count)
        {
            this.count = count;
            this.minimum = minimum;
            this.maximum = maximum;
        }

        public int Minimum
        {
            get { return minimum; }
            set { minimum = value; }
        }

        public int Maximum
        {
            get { return maximum; }
            set { maximum = value; }
        }

        public int Count
        {
            get { return count; }
            set { count = value; }
        }
    }
}
