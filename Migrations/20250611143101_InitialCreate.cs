using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProfessorApp.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Alunos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nome = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    DataNascimento = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Ativo = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Alunos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Topicos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nome = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Topicos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Aprendizados",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AlunoId = table.Column<int>(type: "INTEGER", nullable: false),
                    DataRegistro = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "datetime('now')"),
                    Descricao = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Aprendizados", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Aprendizados_Alunos_AlunoId",
                        column: x => x.AlunoId,
                        principalTable: "Alunos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AprendizadoTopicos",
                columns: table => new
                {
                    AprendizadosId = table.Column<int>(type: "INTEGER", nullable: false),
                    TopicosId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AprendizadoTopicos", x => new { x.AprendizadosId, x.TopicosId });
                    table.ForeignKey(
                        name: "FK_AprendizadoTopicos_Aprendizados_AprendizadosId",
                        column: x => x.AprendizadosId,
                        principalTable: "Aprendizados",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AprendizadoTopicos_Topicos_TopicosId",
                        column: x => x.TopicosId,
                        principalTable: "Topicos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Aprendizados_AlunoId",
                table: "Aprendizados",
                column: "AlunoId");

            migrationBuilder.CreateIndex(
                name: "IX_AprendizadoTopicos_TopicosId",
                table: "AprendizadoTopicos",
                column: "TopicosId");

            migrationBuilder.CreateIndex(
                name: "IX_Topicos_Nome",
                table: "Topicos",
                column: "Nome",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AprendizadoTopicos");

            migrationBuilder.DropTable(
                name: "Aprendizados");

            migrationBuilder.DropTable(
                name: "Topicos");

            migrationBuilder.DropTable(
                name: "Alunos");
        }
    }
}
