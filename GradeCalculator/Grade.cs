namespace GradeCalculator {
    public class Grade {
        public string Name { get; set; }
        public double Weight { get; set; }
        public double Mark { get; set; }
        public double Result { get; set; }

        public Grade(string name, double mark, double weight) {
            Name = name;
            Mark = mark;
            Weight = weight;
            Result = Mark / 100 * weight;
        }
    }
}