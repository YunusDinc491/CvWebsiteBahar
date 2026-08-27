namespace CvWebsite.Models
{
    public class AboutMe
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string MainMessage { get; set; }

        public string CatchPhrase { get; set; }
        public string NameSurname { get; set; }
        public string JobName { get; set; }
        public string Icons { get; set; }

        public string Biography { get; set; }

        // Profile photo shown on the About Me page. Nullable so adding this
        // column doesn't require a default value for any rows already saved.
        public string? PhotoUrl { get; set; }
    }
}
