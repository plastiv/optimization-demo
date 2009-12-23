using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Documents;
using Microsoft.Research.DynamicDataDisplay.DataSources.MultiDimensional;
using Microsoft.Research.DynamicDataDisplay.DataSources;
using Microsoft.Research.DynamicDataDisplay.Common.Auxiliary;
using Microsoft.Research.DynamicDataDisplay.Charts;
using Microsoft.Research.DynamicDataDisplay;
using System.Windows.Media;
using System.Windows.Shapes;
using Microsoft.Research.DynamicDataDisplay.Charts.Shapes;

namespace MoptDemo
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
       readonly private int minValue;
       readonly private int maxValue;
       readonly private int pointCount;

        TestFunctions mytest;
        DataLayer myData;
        ViewportPolyline vwpolyline;
        int solPointIndex;
        int solPointCount;

        public Window1()
        {
            InitializeComponent();
            minValue = MoptDemo.Properties.Settings.Default.MinValue;
            maxValue = MoptDemo.Properties.Settings.Default.MaxValue;
            pointCount = MoptDemo.Properties.Settings.Default.PointCount;

            solPointCount = 0;
            solPointIndex = 0;
            mytest = new TestFunctions();
            myData = new DataLayer();
            vwpolyline = new ViewportPolyline();
            Loaded += new RoutedEventHandler(Window1_Loaded);
        }

        private void Window1_Loaded(object sender, RoutedEventArgs e)
        {
            cmbMethods.Items.Add(DataLayer.Methods.Gradient);
            cmbMethods.Items.Add(DataLayer.Methods.Hooke_Jeves);

            for (int i = 0; i < mytest.funcCount; i++)
            {
                cmbFunctions.Items.Add("Function " + i.ToString());
            }

        }

        private void cmbFunctions_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            plotter.Children.Remove(vwpolyline);
            txtFunction.Text = mytest.funcsStr[cmbFunctions.SelectedIndex];

            myData.Function = mytest.funcs[cmbFunctions.SelectedIndex];
            WarpedDataSource2D<double> dataSource = DataSource.GetDataSource(myData.Function, minValue, maxValue, pointCount);
            isolineGraph.DataSource = dataSource;
            trackingGraph.DataSource = dataSource;

            Rect visible = dataSource.GetGridBounds();
            plotter.Viewport.Visible = visible;
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            plotter.Children.Remove(vwpolyline);
            vwpolyline.Points = myData.GetSolutionPoints(cmbMethods.SelectedItem, new double[2] { double.Parse(txtX1.Text), double.Parse(txtX2.Text) });
            solPointCount = myData.SolutionCount;
            solPointIndex = solPointCount;
            plotter.AddChild(vwpolyline);
        }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            plotter.Children.Remove(vwpolyline);
        }

        private void btnForward_Click(object sender, RoutedEventArgs e)
        {
            if(solPointIndex<solPointCount)
            {
            vwpolyline.Points.Add(myData.GetCurrPoint(solPointIndex));
            solPointIndex++;
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            if (solPointIndex>0)
            {
                vwpolyline.Points.RemoveAt(solPointIndex-1);
                solPointIndex--;
            }
            
        }
    }
}
