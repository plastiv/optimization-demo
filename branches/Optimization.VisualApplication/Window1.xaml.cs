using System.Windows;
using System.Windows.Media;
using Microsoft.Research.DynamicDataDisplay.Charts.Shapes;
using Microsoft.Research.DynamicDataDisplay.Common.Auxiliary;
using Microsoft.Research.DynamicDataDisplay.DataSources.MultiDimensional;
using Microsoft.Research.DynamicDataDisplay.PointMarkers;
using Optimization.Tests.Tasks;

namespace Optimization.VisualApplication
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        readonly private int minValue;
        readonly private int maxValue;
        readonly private int pointCount;

        LineSource lineSource;
        ViewportPolyline vwpolyline;
        int solPointIndex;
        int solPointCount;

        public Window1()
        {
            InitializeComponent();

            minValue = Optimization.VisualApplication.Properties.Settings.Default.MinValue;
            maxValue = Optimization.VisualApplication.Properties.Settings.Default.MaxValue;
            pointCount = Optimization.VisualApplication.Properties.Settings.Default.PointCount;

            solPointCount = 0;
            solPointIndex = 0;
            vwpolyline = new ViewportPolyline();
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
            plotter.Children.Remove(vwpolyline);

            ManyVariableFunctionTask selectedTask = (ManyVariableFunctionTask)cmbFunctions.SelectedItem;
            txtFunction.Text = selectedTask.expression;
            lineSource = new LineSource(selectedTask.function);
            WarpedDataSource2D<double> dataSource = DataSource.GetDataSource(selectedTask.function, minValue, maxValue, pointCount);
            isolineGraph.DataSource = dataSource;
            trackingGraph.DataSource = dataSource;

            Rect visible = dataSource.GetGridBounds();
            plotter.Viewport.Visible = visible;
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            plotter.Children.Remove(vwpolyline);
            vwpolyline.Points = lineSource.GetPointCollection(cmbMethods.SelectedItem, new double[2] { double.Parse(txtX1.Text), double.Parse(txtX2.Text) });
            solPointCount = lineSource.PointsCount;
            solPointIndex = solPointCount;
            var marker = new CircleElementPointMarker
            {
                Size = 10,
                Brush = Brushes.Red,
                Fill = Brushes.Orange
            };
            marker.ToolTipText = "Value is";
            vwpolyline.ToolTip = marker;
            plotter.AddChild(vwpolyline);
        }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            plotter.Children.Remove(vwpolyline);
        }

        private void btnForward_Click(object sender, RoutedEventArgs e)
        {
            if (solPointIndex < solPointCount)
            {
                vwpolyline.Points.Add(lineSource.GetCurrPoint(solPointIndex));
                solPointIndex++;
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            if (solPointIndex > 0)
            {
                vwpolyline.Points.RemoveAt(solPointIndex - 1);
                solPointIndex--;
            }

        }

        private void MenuItemIsoline_Click(object sender, RoutedEventArgs e)
        {

            MessageBox.Show("Menu item clicked");

        }
    }
}
