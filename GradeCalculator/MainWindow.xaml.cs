using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace GradeCalculator {
    /// <summary>
    /// Main window of the program, containing all logic
    /// </summary>
    public partial class MainWindow : Window {
        private Dictionary<string, Course> courses;

        public MainWindow() {
            InitializeComponent();

            //TODO Load the files from XML or something?


            //Placeholder
            var course1 = new Course("Software Design", "SWEN225");
            course1.Grades.Add(new Grade("Assignment 1", 100, 15));
            course1.Grades.Add(new Grade("Assignment 2", 100, 15));
            course1.Grades.Add(new Grade("Group Project", 100, 40));
            var course2 = new Course("Clouds and Networking", "NWEN243");
            course2.Grades.Add(new Grade("Project 1", 100, 15));
            course2.Grades.Add(new Grade("Project 2", 100, 15));
            course2.Grades.Add(new Grade("Project 3", 100, 15));
            courses = new Dictionary<string, Course> {{"SWEN225", course1}, {"NWEN243", course2}};

            Courses.ItemsSource = courses;
        }

        /// <summary>
        /// View a course's information/grades by clicking it in the sidebar
        /// </summary>
        /// <param name="sender">The source of the event</param>
        /// <param name="args">Event arguments</param>
        private void ViewCourse(object sender, SelectionChangedEventArgs args) {
            //Get name from the source
            if (!(((ListBox) sender).SelectedItem is KeyValuePair<string, Course> course)) {
                return;
            }

            //Hide the menu title
            MenuTitle.Visibility = Visibility.Hidden;


            //TODO MOVE TO XAML?
            //Create the grid of cells for displaying grades
            var grid = new DataGrid {
                ItemsSource = course.Value.Grades,
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

            
            //Add all the columns and make them all have a black background
            var cells = Resources["BlackCell"] as Style;
            var headers = Resources["BlackHeader"] as Style;
            grid.Columns.Add(new DataGridTextColumn {
                Header = "Name", Binding = new Binding("Name"), Width = 275, CellStyle = cells, HeaderStyle = headers
            });
            grid.Columns.Add(new DataGridTextColumn {
                Header = "Result", Binding = new Binding("Result"), Width = 75, CellStyle = cells, HeaderStyle = headers
            });
            grid.Columns.Add(new DataGridTextColumn {
                Header = "Weight", Binding = new Binding("Weight"), Width = 75, CellStyle = cells, HeaderStyle = headers
            });


            //Add the finished grid to the Displayed Grid
            Grid.Children.Add(grid);
            Grid.SetColumn(grid, 1);
        }
    }
}