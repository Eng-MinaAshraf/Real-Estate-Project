namespace RealEstateProject.Models
{
    public class Media
    {
        public int MediaId { get; set; }

        public int? PropId { get; set; }
        public Property? Property { get; set; }

        public string MediaType { get; set; } = string.Empty;
        public string MediaUrl { get; set; } = string.Empty;
    }
}
