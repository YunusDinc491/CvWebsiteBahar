namespace CvWebsite.Models
{
    public class Project
    {
        public int Id { get; set; }
        public string MainExplanation { get; set; }
        public string ProjectName { get; set; }
        public string RoleTitle { get; set; }
        public string Explanation { get; set; }

        // Nullable so a project without a photo yet doesn't block saving —
        // the public Projects page already falls back to a placeholder image.
        public string? PhotoUrl { get; set; }
    }
}
