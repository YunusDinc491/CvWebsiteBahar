namespace CvWebsite.Models
{
    public class Contact
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string Email { get; set; }

        // Optional fields in the contact form — nullable so a visitor
        // isn't forced to fill them in.
        public string? Subject { get; set; }
        public string? Message { get; set; }
    }
}
