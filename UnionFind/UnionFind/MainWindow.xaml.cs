using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace UnionFind
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<Connection> connections = new List<Connection>();
        private List<Connection> outConnections = new List<Connection>();
        private List<Path> paths = new List<Path>();

        public MainWindow()
        {
            InitializeComponent();
            InitGrid();
        }

        private void InitGrid()
        {
            Path myPath = new Path();
            myPath.Name = "MyCanvas_Grid";
            myPath.Stroke = Brushes.Blue;
            myPath.StrokeThickness = 0.1;

            StreamGeometry geometry = new StreamGeometry();
            using (StreamGeometryContext ctx = geometry.Open())
            {
                for (int i = 0; i <= 500; i += 50)
                {
                    ctx.BeginFigure(new Point(0, i), false, false);
                    ctx.LineTo(new Point(500, i), true, false);
                    ctx.BeginFigure(new Point(i, 0), false, false);
                    ctx.LineTo(new Point(i, 500), true, false);
                }
            }
            geometry.Freeze();
            myPath.Data = geometry;
            MyCanvas.Children.Add(myPath);
        }

        private void Button_LoadData_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Filter = "Xml documents(.xml)|*.xml";
            if (dlg.ShowDialog() == true)
            {
                UFRoot xml = XmlUtils.XmlDeserializeFromFile<UFRoot>(dlg.FileName, Encoding.UTF8);
                if (xml.Data.Count != 0)
                {
                    connections = xml.Data;
                    DrawLine(connections);
                    MyDataGrid.ItemsSource = connections;
                }
            }
        }

        private void Button_SaveData_Click(object sender, RoutedEventArgs e)
        {
            if (connections.Count == 0) return;
            Microsoft.Win32.SaveFileDialog slg = new Microsoft.Win32.SaveFileDialog();
            slg.Filter = "Xml documents(.xml)|*.xml";
            if (slg.ShowDialog() == true)
            {
                UFRoot ufRoot = new UFRoot { Header = new Header(), Data = connections };
                XmlUtils.XmlSerializeToFile(ufRoot, slg.FileName, Encoding.UTF8);
            }
        }

        private List<Connection> AutoCreateData(int count, int minValue, int maxValue)
        {
            List<Connection> list = new List<Connection>();
            Random random = new Random();
            MyPoint[] myPoint =
            {
                new MyPoint {X = 0, Y = 50},
                new MyPoint {X = 0, Y = -50},
                new MyPoint {X = 50, Y = 0},
                new MyPoint {X = -50, Y = 0}
            };
            for (int i = 0; i < count; ++i)
            {
                Connection connection = new Connection();
                connection.Point1 = new MyPoint { X = random.Next(minValue, maxValue) * 50, Y = random.Next(minValue, maxValue) * 50 };
                int r = random.Next(0, 4);
                MyPoint p = connection.Point1 + myPoint[r];
                if (p.X < 0 || p.X > 500 || p.Y < 0 || p.Y > 500) p = connection.Point1 - myPoint[r];
                connection.Point2 = p;
                list.Add(connection);
            }
            return list;
        }

        private void DrawLine(List<Connection> connections)
        {
            Path linePath = new Path();
            linePath.Name = "MyCanvas_LinePath";
            linePath.Stroke = Brushes.Black;
            linePath.StrokeThickness = 3;

            StreamGeometry streamGeometry = new StreamGeometry();
            using (StreamGeometryContext sgc = streamGeometry.Open())
            {
                foreach (Connection c in connections)
                {
                    sgc.BeginFigure(new Point(c.Point1.X, 500 - c.Point1.Y), false, false);
                    sgc.LineTo(new Point(c.Point2.X, 500 - c.Point2.Y), true, false);
                }
            }

            streamGeometry.Freeze();
            linePath.Data = streamGeometry;
            MyCanvas.Children.Add(linePath);
            paths.Add(linePath);
        }

        private void DrawLine(Connection c)
        {
            Path linePath = new Path();
            linePath.Stroke = Brushes.DeepPink;
            linePath.StrokeThickness = 3;

            StreamGeometry streamGeometry = new StreamGeometry();
            using (StreamGeometryContext sgc = streamGeometry.Open())
            {
                sgc.BeginFigure(new Point(c.Point1.X, 500 - c.Point1.Y), false, false);
                sgc.LineTo(new Point(c.Point2.X, 500 - c.Point2.Y), true, false);
            }

            streamGeometry.Freeze();
            linePath.Data = streamGeometry;
            MyCanvas.Children.Add(linePath);
            paths.Add(linePath);
        }

        private void Button_RemoveData_Click(object sender, RoutedEventArgs e)
        {
            foreach (Path p in paths)
            {
                MyCanvas.Children.Remove(p);
            }
            MyDataGrid.ItemsSource = null;
            connections.Clear();
        }

        private void Button_CreateData_Click(object sender, RoutedEventArgs e)
        {
            List<Connection> list = AutoCreateData(10, 0, 11);

            DrawLine(list);
            connections = connections.Concat<Connection>(list).ToList<Connection>();
            MyDataGrid.ItemsSource = connections;
        }

        private void Button_Print_Click(object sender, RoutedEventArgs e)
        {
            if (connections.Count == 0) return;
            int n = connections.Count;
            QuickFind qf = new QuickFind(10);
            foreach (Connection con in connections)
            {
                if (qf.Connected(con.Point1, con.Point2))
                {
                    outConnections.Add(con);
                    DrawLine(con);
                    continue;
                }
                qf.Union(con.Point1, con.Point2);
                //System.Diagnostics.Debug.WriteLine($"X:{con.Point1.X},Y:{con.Point1.Y} Union X:{con.Point2.X},Y:{con.Point2.Y}\n");
                //for (int i = 0; i <= 4; i++)
                //{
                //    System.Diagnostics.Debug.Write($"[{i},1]{qf.id[i, 1]}   ");
                //}
                //for (int i = 0; i <= 4; i++)
                //{
                //    System.Diagnostics.Debug.Write($"[{i},0]{qf.id[i, 0]}   ");
                //}
                //System.Diagnostics.Debug.WriteLine("");
            }

            MyOutDataGrid.ItemsSource = outConnections;
            System.Diagnostics.Debug.WriteLine(qf.Count + "Components");

            //foreach(Connection c in connections)
            //{
            //    System.Diagnostics.Debug.WriteLine($"{c.Point1},{c.Point2}");
            //}

        }
    }
}
