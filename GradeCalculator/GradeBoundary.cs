namespace GradeCalculator {
    public class GradeBoundary {
        public string Grade { get; set; }
        public int LowerBound { get; set; }
        
        public GradeBoundary(string grade, int lowerBound) {
            Grade = grade;
            LowerBound = lowerBound;
        }
    }
}