using System.Collections.Generic;

namespace GradeCalculator {
    public class Course {
        public string Name { get; private set; }
        public string CourseCode { get; private set; }
        public List<Grade> Grades { get; private set; }

        public Course(string name, string courseCode) {
            Name = name;
            CourseCode = courseCode;
            Grades = new List<Grade>();
        }
    }
}