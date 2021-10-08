using System.Collections.Generic;

namespace GradeCalculator {
    public class Course {
        private string Name { get; set; }
        public string CourseCode { get; set; }
        private List<Grade> Grades { get; set; }

        public Course(string name, string courseCode) {
            Name = name;
            CourseCode = courseCode;
            Grades = new List<Grade>();
        }


        public override string ToString() {
            return "Hoge";
        }
    }
}