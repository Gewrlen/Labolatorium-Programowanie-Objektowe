namespace SystemOcenianiaSimple.Models
{
    public class Assessment : EntityBase
    {
        public int ClassId { get; set; }
        public int CreatedByUserId { get; set; }
        public string Title { get; set; } = "";
        public string Category { get; set; } = "";     
        public string GradingMode { get; set; } = "";  
        public decimal? MaxPoints { get; set; }
        public decimal Weight { get; set; }
    }
}
