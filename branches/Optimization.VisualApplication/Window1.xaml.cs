using System.Windows;
using System.Windows.Media;
using Microsoft.Research.DynamicDataDisplay.Charts.Shapes;
using Microsoft.Research.DynamicDataDisplay.Common.Auxiliary;
using Microsoft.Research.DynamicDataDisplay.DataSources.MultiDimensional;
using Microsoft.Research.DynamicDataDisplay.PointMarkers;
using Optimization.Tests.Tasks;
using Microsoft.Research.DynamicDataDisplay.Charts;
using Microsoft.Research.DynamicDataDisplay.Charts.Navigation;

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
        ViewportPolyline viewportPolyline;
        WarpedDataSource2D<double> dataSource;
        int solPointIndex;
        int solPointCount;
        IsolineTrackingGraph trackingGraph;
        CursorCoordinateGraph cursorCoordinateGraph;

        public Window1()
        {
            InitializeComponent();

            minValue = Optimization.VisualApplication.Properties.Settings.Default.MinValue;
            maxValue = Optimization.VisualApplication.Properties.Settings.Default.MaxValue;
            pointCount = Optimization.VisualApplication.Properties.Settings.Default.PointCount;

            solPointCount = 0;
            solPointIndex = 0;
            viewportPolyline = new ViewportPolyline();
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
            plotter.Children.Remove(viewportPolyline);

            ManyVariableFunctionTask selectedTask = (ManyVariableFunctionTask)cmbFunctions.SelectedItem;
            txtFunction.Text = selectedTask.expression;
            lineSource = new LineSource(selectedTask.function);
            txtX1.Text = selectedTask.startPoint[0].ToString();
            txtX2.Text = selectedTask.startPoint[1].ToString();
            dataSource = DataSource.GetDataSource(selectedTask.function, minValue, maxValue, pointCount);
            isolineGraph.DataSource = dataSource;
            trackingGraph = new IsolineTrackingGraph(); // TODO: Lazy initialization.
            trackingGraph.DataSource = dataSource;
            Rect visible = dataSource.GetGridBounds();
            plotter.Viewport.Visible = visible;
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            plotter.Children.Remove(viewportPolyline);
            viewportPolyline.Points = lineSource.GetPointCollection(cmbMethods.SelectedItem, new double[2] { double.Parse(txtX1.Text), double.Parse(txtX2.Text) });
            solPointCount = lineSource.PointsCount;
            solPointIndex = solPointCount;

            plotter.AddChild(viewportPolyline);
        }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            plotter.Children.Remove(viewportPolyline);
        }

        private void btnForward_Click(object sender, RoutedEventArgs e)
        {
            if (solPointIndex < solPointCount)
            {
                viewportPolyline.Points.Add(lineSource.GetCurrPoint(solPointIndex));
                solPointIndex++;
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            if (solPointIndex > 0)
            {
                viewportPolyline.Points.RemoveAt(solPointIndex - 1);
                solPointIndex--;
            }
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
