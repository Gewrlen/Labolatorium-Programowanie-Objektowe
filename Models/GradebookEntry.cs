namespace SystemOcenianiaSimple.Models
{
    public class GradebookEntry
    {
        public int EnrollmentId { get; set; }

        public int StudentId { get; set; }
        public string StudentName { get; set; } = "";
        public string IndexNo { get; set; } = "";

        public int AssessmentId { get; set; }
        public string AssessmentTitle { get; set; } = "";
        public string Mode { get; set; } = "";

        public decimal? GradeValue { get; set; }
        public decimal? Points { get; set; }
        public bool IsAbsent { get; set; }
    }
}
