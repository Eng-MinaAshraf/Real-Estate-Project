namespace RealEstateProject.Models.ViewModels
{
    /// <summary>
    /// Simple model for the shared _PageHeader partial.
    /// </summary>
    public class PageHeaderModel
    {
        public string Title { get; set; } = string.Empty;
        public string? Subtitle { get; set; }
        public string? ActionText { get; set; }
        public string? ActionUrl { get; set; }
    }
}
