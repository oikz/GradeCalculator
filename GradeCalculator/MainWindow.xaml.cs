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
        public MainWindow() {
            InitializeComponent();

            //TODO Load the files from XML or something?


            //Placeholder
            var course1 = new Course("Software Design", "SWEN225");
            var course2 = new Course("Clouds and Networking", "NWEN243");
            var courses = new List<Course> {course1, course2};

            Courses.ItemsSource = courses;
        }
    }
}