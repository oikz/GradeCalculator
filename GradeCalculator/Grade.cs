namespace GradeCalculator {
    public class Grade {
        public string Name { get; set; }
        public double Result { get; set; }

        public Grade(string name, double result) {
            Name = name;
            Result = result;
        }
    }
}