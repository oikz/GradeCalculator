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
            course1.Grades.Add(new Grade("Assignment 1", 100));
            var course2 = new Course("Clouds and Networking", "NWEN243");
            courses = new List<Course> {course1, course2};

            Courses.ItemsSource = courses;
            Courses.SelectionChanged += ViewCourse;
        }
        private void ViewCourse(object sender, SelectionChangedEventArgs args) {
            //Show the course information
            MenuTitle.Visibility = Visibility.Hidden;
            var items = new ListBox {
                ItemsSource = courses[0].Grades, 
                DisplayMemberPath = "Name",
                Background = Brushes.Black,
                Foreground = Brushes.White,
                BorderBrush = null,
                FontSize = 25,
                Width = 450,
                HorizontalAlignment = HorizontalAlignment.Left
            };
            
            
            Grid.Children.Add(items);
            Grid.SetColumn(items, 1);

        }
    }
}