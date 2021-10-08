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

        /// <summary>
        /// Update the result of this grade by recalculating with new values
        /// </summary>
        public void UpdateResult() {
            Result = Mark / 100 * Weight;
        }

        /// <summary>
        /// Return the % completed by this course
        /// i.e., If this assignment has been completed, return its full weight
        /// If not completed, return 0 as it has not been completed
        /// </summary>
        /// <returns>The completed weight of this assignment</returns>
        public double GetCompleted() {
            if (Mark == 0) {
                return 0;
            }

            return Weight;
        }
    }
}