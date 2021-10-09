using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Xml;
using System.Xml.Linq;

namespace GradeCalculator {
    /// <summary>
    /// Main window of the program, containing all logic
    /// </summary>
    public partial class MainWindow : Window {
        private Dictionary<string, Course> _courses = new Dictionary<string, Course>();
        private Course _currentCourse;

        public MainWindow() {
            InitializeComponent();

            //TODO Load the files from XML/JSON or something?
            LoadCourses();

            //Placeholder
            //var course1 = new Course("Software Design", "SWEN225");
            //course1.Grades.Add(new Grade("Assignment 1", 100, 15));
            //course1.Grades.Add(new Grade("Assignment 2", 100, 15));
            //course1.Grades.Add(new Grade("Group Project", 100, 40));
            //var course2 = new Course("Clouds and Networking", "NWEN243");
            //course2.Grades.Add(new Grade("Project 1", 100, 15));
            //course2.Grades.Add(new Grade("Project 2", 100, 15));
            //course2.Grades.Add(new Grade("Project 3", 100, 15));
            //_courses = new Dictionary<string, Course> {{"SWEN225", course1}, {"NWEN243", course2}};

            Courses.ItemsSource = _courses;
        }

        /// <summary>
        /// Load the available courses from a default file
        /// </summary>
        private void LoadCourses() {
            //No file exists already, create default empty file
            if (!File.Exists("Courses.xml")) {
                var writer = XmlWriter.Create("Courses.xml");
                writer.WriteElementString("Courses", null);
                writer.Close();
                return;
            }

            //Load all courses from the file
            var stream = File.Open("Courses.xml", FileMode.Open);
            var document = XDocument.Load(stream, LoadOptions.None);
            var courses = document.Element("Courses");
            if (courses == null) return;
            foreach (var course in courses.Elements()) {
                var newCourse = new Course(course.Attribute("courseCode")?.Value);
                foreach (var grade in course.Elements()) {
                    newCourse.Grades.Add(new Grade(grade.Attribute("name")?.Value, Convert.ToDouble(grade.Attribute("mark")?.Value),
                        Convert.ToDouble(grade.Attribute("weight")?.Value)));
                }
                _courses.Add(newCourse.CourseCode, newCourse);
            }
            stream.Close();
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

            _currentCourse = course.Value;

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
            ((DataGridTextColumn) GradeGrid.Columns[0]).Binding = new Binding {
                Path = new PropertyPath("Name"),
                Mode = BindingMode.TwoWay,
                UpdateSourceTrigger = UpdateSourceTrigger.LostFocus
            };
            ((DataGridTextColumn) GradeGrid.Columns[1]).Binding = new Binding {
                Path = new PropertyPath("Mark"),
                Mode = BindingMode.TwoWay,
                UpdateSourceTrigger = UpdateSourceTrigger.LostFocus
            };
            ((DataGridTextColumn) GradeGrid.Columns[2]).Binding = new Binding {
                Path = new PropertyPath("Weight"),
                Mode = BindingMode.TwoWay,
                UpdateSourceTrigger = UpdateSourceTrigger.LostFocus
            };
            ((DataGridTextColumn) GradeGrid.Columns[3]).Binding = new Binding {
                Path = new PropertyPath("Result"),
                Mode = BindingMode.TwoWay,
                UpdateSourceTrigger = UpdateSourceTrigger.LostFocus
            };

            AddGrade.Visibility = Visibility.Visible;
            //Calculate and display the remaining values at the bottom
            CalculateValues();
        }


        /// <summary>
        /// Calculate and display all of the averages, percentages etc based on the results
        /// </summary>
        private void CalculateValues() {
            //Add the display of grade averages etc
            Completed.Visibility = Visibility.Visible;
            var completed = _currentCourse.Grades.Sum(grade => grade.Result);
            Completed.Content = "Current Percentage:\t\t" + Math.Round(completed, 2) + "%";

            CompletedPercentage.Visibility = Visibility.Visible;
            var completedPercentage = _currentCourse.Grades.Sum(grade => grade.GetCompleted());
            CompletedPercentage.Content = "Percentage Completed:\t\t" + Math.Round(completedPercentage, 2) + "%";

            AverageGrade.Visibility = Visibility.Visible;
            var average = _currentCourse.Grades.Sum(grade => grade.Mark);
            average /= _currentCourse.Grades.Count;
            AverageGrade.Content = "Average Mark:\t\t\t" + Math.Round(average, 2);

            try {
                GradeGrid.Items.Refresh();
            } catch (InvalidOperationException) {
            }
            //Invalid during edit or add
        }


        /// <summary>
        /// Method called when a cell is updated
        /// Want to recalculate all of the values based on it's changes
        /// </summary>
        /// <param name="sender">The origin of this event</param>
        /// <param name="args">The arguments supplied with the event</param>
        private void CellEdited(object sender, EventArgs args) {
            foreach (var grade in _currentCourse.Grades) {
                grade.UpdateResult();
            }

            GradeGrid.CommitEdit();
            CalculateValues();
        }

        /// <summary>
        /// Method called when a key is pressed in the grid
        /// Used to refresh when the user presses enter
        /// </summary>
        /// <param name="sender">The origin of this event</param>
        /// <param name="args">The arguments supplied with the event</param>
        private void CellKeyDown(object sender, KeyEventArgs args) {
            if (args.Key != Key.Enter || !(args.OriginalSource is UIElement cell)) return;
            args.Handled = true;
            cell.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            CellEdited(sender, args);
        }

        /// <summary>
        /// Add a new grade to the current course
        /// </summary>
        /// <param name="sender">The origin of this event (unused)</param>
        /// <param name="args">The arguments supplied with the event</param>
        private void AddNewGrade(object sender, RoutedEventArgs args) {
            _currentCourse.Grades.Add(new Grade("", 0, 0));
            GradeGrid.Items.Refresh();
        }

        /// <summary>
        /// Validate the text input of the DataGrid to only allow numbers and .'s
        /// </summary>
        /// <param name="sender">The origin of this event (unused)</param>
        /// <param name="args">The arguments supplied with the event - text input</param>
        private void GridTextValidation(object sender, TextCompositionEventArgs args) {
            if (args.OriginalSource is DataGridCell) return;
            if (((DataGridCell) ((TextBox) args.OriginalSource).Parent).Column.Header.Equals("Name"))
                return;
            args.Handled = new Regex("[^0-9.]+").IsMatch(args.Text);
        }

        /// <summary>
        /// Save the courses currently loaded into the program to the default XML file
        /// </summary>
        /// <param name="sender">The origin of this event (unused)</param>
        /// <param name="args">The arguments supplied with the event - text input (unused)</param>
        private void SaveCourses(object sender, RoutedEventArgs args) {
            //Reset the file to only contain the root node - <Courses/>
            var writer = XmlWriter.Create("Courses.xml");
            writer.WriteElementString("Courses", null);
            writer.Close();
            
            //Load in the file
            var stream = File.Open("Courses.xml", FileMode.Open);
            var document = XDocument.Load(stream, LoadOptions.None);
            var courses = document.Element("Courses");

            //Save each course and all the grades within it
            foreach (var (_, value) in _courses) {
                var element = new XElement("course");
                element.SetAttributeValue("courseCode", value.CourseCode);
                foreach (var grade in value.Grades) {
                    var gradeElement = new XElement("grade");
                    gradeElement.SetAttributeValue("name", grade.Name);
                    gradeElement.SetAttributeValue("mark", grade.Mark);
                    gradeElement.SetAttributeValue("weight", grade.Weight);
                    element.Add(gradeElement);
                }
                courses?.Add(element);
            }
            
            stream.Close();
            document.Save("Courses.xml");
        }
    }
}