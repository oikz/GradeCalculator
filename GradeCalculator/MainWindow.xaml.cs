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

namespace GradeCalculator {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        private List<Course> courses;

        public MainWindow() {
            InitializeComponent();

            //TODO Load the files from XML or something?


            //Placeholder
            var course1 = new Course("Software Design", "SWEN225");
            course1.Grades.Add(new Grade("Assignment 1", 100, 15));
            course1.Grades.Add(new Grade("Assignment 2", 100, 15));
            course1.Grades.Add(new Grade("Assignment 3", 100, 25));
            var course2 = new Course("Clouds and Networking", "NWEN243");
            courses = new List<Course> {course1, course2};

            Courses.ItemsSource = courses;
            Courses.SelectionChanged += ViewCourse;
        }

        private void ViewCourse(object sender, SelectionChangedEventArgs args) {
            //Show the course information
            MenuTitle.Visibility = Visibility.Hidden;

            
            var grid = new DataGrid {
                ItemsSource = courses[0].Grades,
                AutoGenerateColumns = false,
                HeadersVisibility = DataGridHeadersVisibility.Column,
                BorderBrush = null,
                FontSize = 20,
                Background = Brushes.Black,
                Foreground = Brushes.White,
                HorizontalGridLinesBrush = Brushes.White,
                VerticalGridLinesBrush = Brushes.White,
                CanUserResizeColumns = false,
                CanUserResizeRows = false,
                CanUserAddRows = false,
                CanUserDeleteRows = false
            };


            grid.Columns.Add(new DataGridTextColumn {
                Header = "Name", Binding = new Binding("Name"), Width = 275
            });
            grid.Columns.Add(new DataGridTextColumn {
                Header = "Result", Binding = new Binding("Result"), Width = 75
            });
            grid.Columns.Add(new DataGridTextColumn {
                Header = "Weight", Binding = new Binding("Weight"), Width = 75
            });


            grid.Columns[0].CellStyle = Resources["BlackCell"] as Style;
            grid.Columns[0].HeaderStyle = Resources["BlackHeader"] as Style;
            grid.Columns[1].CellStyle = Resources["BlackCell"] as Style;
            grid.Columns[1].HeaderStyle = Resources["BlackHeader"] as Style;
            grid.Columns[2].CellStyle = Resources["BlackCell"] as Style;
            grid.Columns[2].HeaderStyle = Resources["BlackHeader"] as Style;
            Grid.Children.Add(grid);
            Grid.SetColumn(grid, 1);
        }
    }
}