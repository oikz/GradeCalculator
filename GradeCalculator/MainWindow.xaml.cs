using System.Collections.Generic;
using System.Linq;
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


            //Create the grid of cells for displaying grades
            GradeGrid.ItemsSource = course.Value.Grades;
            GradeGrid.Visibility = Visibility.Visible;

            
            //Add all the columns and make them all have a black background
            foreach (var dataGridColumn in GradeGrid.Columns) {
                dataGridColumn.Visibility = Visibility.Visible;
            }

            //Set the binding for all columns
            ((DataGridTextColumn) GradeGrid.Columns[0]).Binding = new Binding("Name");
            ((DataGridTextColumn) GradeGrid.Columns[1]).Binding = new Binding("Mark");
            ((DataGridTextColumn) GradeGrid.Columns[2]).Binding = new Binding("Weight");
            ((DataGridTextColumn) GradeGrid.Columns[3]).Binding = new Binding("Result");
            
            //Add the display of grade averages etc
            Completed.Visibility = Visibility.Visible;
            var completed = course.Value.Grades.Sum(grade => grade.Mark);
            Completed.Content = "Percentage Completed: " + completed;
        }
    }
}