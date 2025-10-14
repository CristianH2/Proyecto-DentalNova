using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Proyecto_DentalNova.Migrations
{
    /// <inheritdoc />
    public partial class ActualizacionTablaOdontologo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Especialidad",
                table: "Odontologos");

            migrationBuilder.CreateTable(
                name: "Especialidades",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Especialidades", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EspecialidadOdontologo",
                columns: table => new
                {
                    EspecialidadesId = table.Column<int>(type: "int", nullable: false),
                    OdontologosId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EspecialidadOdontologo", x => new { x.EspecialidadesId, x.OdontologosId });
                    table.ForeignKey(
                        name: "FK_EspecialidadOdontologo_Especialidades_EspecialidadesId",
                        column: x => x.EspecialidadesId,
                        principalTable: "Especialidades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EspecialidadOdontologo_Odontologos_OdontologosId",
                        column: x => x.OdontologosId,
                        principalTable: "Odontologos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EspecialidadOdontologo_OdontologosId",
                table: "EspecialidadOdontologo",
                column: "OdontologosId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EspecialidadOdontologo");

            migrationBuilder.DropTable(
                name: "Especialidades");

            migrationBuilder.AddColumn<string>(
                name: "Especialidad",
                table: "Odontologos",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }
    }
}
