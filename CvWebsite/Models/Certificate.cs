namespace CvWebsite.Models
{
    public class Certificate
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        // Nullable so seed data / new records without a photo yet don't
        // require a default value in the migration.
        public string? PhotoUrl { get; set; }
    }
}
