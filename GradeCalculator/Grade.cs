namespace GradeCalculator {
    public class Grade {
        public string Name { get; set; }
        public double Result { get; set; }
        public double Weight { get; set; }

        public Grade(string name, double result, double weight) {
            Name = name;
            Result = result;
            Weight = weight;
        }
    }
}