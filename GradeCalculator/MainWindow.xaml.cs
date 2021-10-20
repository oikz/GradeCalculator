using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml;
using System.Xml.Linq;

namespace GradeCalculator {
    /// <summary>
    /// Main window of the program, containing all logic
    /// </summary>
    public partial class MainWindow : Window {
        private readonly Dictionary<string, Course> _courses = new Dictionary<string, Course>();
        private Course _currentCourse;
        private readonly List<GradeBoundary> _gradeBoundaries = new List<GradeBoundary>();

        public MainWindow() {
            InitializeComponent();
            LoadCourses();
            LoadGrades();
            if (_currentCourse != null) CalculateValues();
            
            if (_courses.Any()) ((RadioButton) Courses.Children[0]).IsChecked = true;
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

                //Add default grade boundaries
                var load = File.Open("Courses.xml", FileMode.Open);
                var doc = XDocument.Load(load, LoadOptions.None);
                var root = doc.Element("Courses");
                var grades = new XElement("grades");
                DefaultGrades(grades);
                root.Add(grades);


                load.Close();
                doc.Save("Courses.xml");
                return;
            }

            //Load all courses from the file
            var stream = File.Open("Courses.xml", FileMode.Open);
            var document = XDocument.Load(stream, LoadOptions.None);
            var courses = document.Element("Courses");
            if (courses == null) return;
            foreach (var course in courses.Elements("course")) {
                var newCourse = new Course(course.Attribute("courseCode")?.Value);
                foreach (var grade in course.Elements()) {
                    newCourse.Grades.Add(new Grade(grade.Attribute("name")?.Value,
                        Convert.ToDouble(grade.Attribute("mark")?.Value),
                        Convert.ToDouble(grade.Attribute("weight")?.Value)));
                }

                _courses.Add(newCourse.CourseCode, newCourse);
                
                //Add to the sidebar
                var button = new RadioButton {
                    Content = newCourse.CourseCode, Height = 50, Style = Application.Current.Resources["RadioStyle"] as Style
                };
                button.Click += ViewCourse;
                Courses.Children.Add(button);
                
                
                //Show the grades from the first course
                if (_currentCourse != null) continue;
                _currentCourse = newCourse;
                GradeGrid.ItemsSource = newCourse.Grades;
            }

            stream.Close();
        }

        /// <summary>
        /// Load the grade boundaries from the XML file and store them locally
        /// </summary>
        private void LoadGrades() {
            var stream = File.Open("Courses.xml", FileMode.Open);
            var document = XDocument.Load(stream, LoadOptions.None);
            var root = document.Element("Courses");
            if (root == null) return;
            foreach (var boundary in root.Element("grades")?.Elements()) {
                _gradeBoundaries.Add(new GradeBoundary(boundary.Attribute("grade")?.Value,
                    Convert.ToInt32(boundary.Attribute("lowerBound")?.Value)));
            }
        }

        /// <summary>
        /// View a course's information/grades by clicking it in the sidebar
        /// </summary>
        /// <param name="sender">The source of the event</param>
        /// <param name="args">Event arguments</param>
        private void ViewCourse(object sender, RoutedEventArgs args) {
            //Get name from the source
            if (!(args.OriginalSource is RadioButton course)) return;

            _currentCourse = _courses[course.Content as string ?? string.Empty];

            //Create the grid of cells for displaying grades
            GradeGrid.ItemsSource = _currentCourse.Grades;

            //Calculate and display the remaining values at the bottom
            CalculateValues();

            GradeSetting.Visibility = Visibility.Collapsed;
            GradeDisplay.Visibility = Visibility.Visible;
        }


        /// <summary>
        /// Calculate and display all of the averages, percentages etc based on the results
        /// </summary>
        private void CalculateValues() {
            //Add the display of grade averages etc
            var completed = _currentCourse.Grades.Sum(grade => grade.Result);
            Completed.Content = "Current Percentage:\t\t" + Math.Round(completed, 2) + "%";

            var completedPercentage = _currentCourse.Grades.Sum(grade => grade.GetCompleted());
            CompletedPercentage.Content = "Percentage Completed:\t\t" + Math.Round(completedPercentage, 2) + "%";

            var average = completed / completedPercentage * 100;
            AverageGrade.Content = "Average Mark:\t\t\t" + Math.Round(average, 2);

            if (_gradeBoundaries.Count == 0) return;
            var averageLetterGrade = _gradeBoundaries.ElementAt(0).Grade;
            for (var i = _gradeBoundaries.Count - 1; i >= 0; i--) {
                if (average >= _gradeBoundaries[i].LowerBound) {
                    averageLetterGrade = _gradeBoundaries[i].Grade;
                }
            }
            
            LetterGrade.Content = "Average Letter Grade:\t\t" + averageLetterGrade;

            var desiredGrade = _gradeBoundaries.Where(boundary => boundary.Grade.Equals(DesiredLetterGradeText.Text));

            var gradeBoundaries = desiredGrade as GradeBoundary[] ?? desiredGrade.ToArray();
            var desired = 0;
            if (gradeBoundaries.Any()) {
                desired = gradeBoundaries.ElementAt(0).LowerBound;
            }

            var required = (desired - completed) / (100 - completedPercentage);
            PercentageRequired.Content = "Percentage Required:\t\t" + Math.Max(Math.Round(required * 100, 2), 0.0);
            
            try {
                GradeGrid.Items.Refresh();
            } catch (InvalidOperationException) {
            }
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
            if (_currentCourse == null) return;
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
            if (((DataGridCell) ((TextBox) args.OriginalSource).Parent).Column.Header.Equals("Name") ||
                ((DataGridCell) ((TextBox) args.OriginalSource).Parent).Column.Header.Equals("Grade"))
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

            //Save the grade boundaries to XML file
            var grades = new XElement("grades");
            foreach (var boundary in _gradeBoundaries) {
                var element = new XElement("boundary");
                element.SetAttributeValue("grade", boundary.Grade);
                element.SetAttributeValue("lowerBound", boundary.LowerBound);
                grades.Add(element);
            }

            courses?.Add(grades);

            stream.Close();
            document.Save("Courses.xml");
        }

        /// <summary>
        /// Add a new course to the program
        /// </summary>
        /// <param name="sender">The origin of this event (unused)</param>
        /// <param name="args">The arguments supplied with the event - text input (unused)</param>
        private void AddCourse(object sender, RoutedEventArgs args) {
            if (CourseName.Text.Equals(string.Empty)) return;
            try {
                _courses.Add(CourseName.Text, new Course(CourseName.Text));
            } catch (ArgumentException) {
                //Don't let the user add the same named course twice
                return;
            }

            var button = new RadioButton {
                Content = CourseName.Text, Height = 50, Style = Application.Current.Resources["RadioStyle"] as Style
            };
            button.Click += ViewCourse;
            Courses.Children.Add(button);
        }

        /// <summary>
        /// Event called when the user clicks to edit grade boundaries
        /// </summary>
        /// <param name="sender">The origin of this event (unused)</param>
        /// <param name="args">The arguments supplied with the event - text input (unused)</param>z
        private void EditGradeBoundaries(object sender, RoutedEventArgs args) {
            GradeSetting.Visibility = Visibility.Visible;
            GradeDisplay.Visibility = Visibility.Collapsed;

            GradeBoundaryGrid.ItemsSource = _gradeBoundaries;
        }


        /// <summary>
        /// Load in a default set of grades to be modified if an XML is not present
        /// </summary>
        /// <param name="grades">The Grades element of the XML file being created</param>
        private void DefaultGrades(XElement grades) {
            string[] letters = {"A+", "A", "A-", "B+", "B", "B-", "C+", "C", "C-", "D", "E"};
            int[] lowerBounds = {90, 85, 80, 75, 70, 65, 60, 55, 50, 40, 0};
            for (var i = 0; i < letters.Length; i++) {
                var elem = new XElement("boundary");
                elem.SetAttributeValue("grade", letters[i]);
                elem.SetAttributeValue("lowerBound", lowerBounds[i]);
                grades.Add(elem);
            }
        }
    }
}