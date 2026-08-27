using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CvWebsite.Migrations
{
    /// <inheritdoc />
    public partial class AddCertificates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Certificate",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhotoUrl = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Certificate", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Certificate",
                columns: new[] { "Id", "Description", "PhotoUrl", "Title" },
                values: new object[,]
                {
                    { 1, "Bu sertifika hakkında kısa bir açıklama. Admin panelinden düzenleyebilirsin.", null, "İletişim Becerileri Sertifikası" },
                    { 2, "Bu sertifika hakkında kısa bir açıklama. Admin panelinden düzenleyebilirsin.", null, "Uygulamalı Dil Terapisi Eğitimi" },
                    { 3, "Bu sertifika hakkında kısa bir açıklama. Admin panelinden düzenleyebilirsin.", null, "Katılım Sertifikası" },
                    { 4, "Bu sertifika hakkında kısa bir açıklama. Admin panelinden düzenleyebilirsin.", null, "Mesleki Gelişim Sertifikası" },
                    { 5, "Bu sertifika hakkında kısa bir açıklama. Admin panelinden düzenleyebilirsin.", null, "Atölye Katılım Belgesi" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Certificate");
        }
    }
}
