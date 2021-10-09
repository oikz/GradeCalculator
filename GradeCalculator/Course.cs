using System.Collections.Generic;

namespace GradeCalculator {
    public class Course {
        public string CourseCode { get; private set; }
        public List<Grade> Grades { get; private set; }

        public Course(string courseCode) {
            CourseCode = courseCode;
            Grades = new List<Grade>();
        }
    }
}