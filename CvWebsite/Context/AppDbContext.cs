using CvWebsite.Models;
using Microsoft.EntityFrameworkCore;

namespace CvWebsite.Context
{
    public class AppDbContext : DbContext
    {

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<AboutMe> AboutMe { get; set; }
        public DbSet<Contact> Contact { get; set; }
        public DbSet<Project> Project { get; set; }
        public DbSet<Resume> Resume { get; set; }
        public DbSet<Certificate> Certificate { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seeds 5 placeholder certificates so the About Me page has
            // something to show right after the migration runs. Edit or
            // replace these from Admin > Sertifikalar.
            modelBuilder.Entity<Certificate>().HasData(
                new Certificate { Id = 1, Title = "İletişim Becerileri Sertifikası", Description = "Bu sertifika hakkında kısa bir açıklama. Admin panelinden düzenleyebilirsin." },
                new Certificate { Id = 2, Title = "Uygulamalı Dil Terapisi Eğitimi", Description = "Bu sertifika hakkında kısa bir açıklama. Admin panelinden düzenleyebilirsin." },
                new Certificate { Id = 3, Title = "Katılım Sertifikası", Description = "Bu sertifika hakkında kısa bir açıklama. Admin panelinden düzenleyebilirsin." },
                new Certificate { Id = 4, Title = "Mesleki Gelişim Sertifikası", Description = "Bu sertifika hakkında kısa bir açıklama. Admin panelinden düzenleyebilirsin." },
                new Certificate { Id = 5, Title = "Atölye Katılım Belgesi", Description = "Bu sertifika hakkında kısa bir açıklama. Admin panelinden düzenleyebilirsin." }
            );
        }
    }
}
