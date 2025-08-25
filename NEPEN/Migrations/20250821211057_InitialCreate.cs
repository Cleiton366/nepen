using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Desafio_NEPEN.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Medidores",
                columns: table => new
                {
                    MedidorId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Medidores", x => x.MedidorId);
                });

            migrationBuilder.CreateTable(
                name: "Leituras",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MedidorId = table.Column<string>(type: "text", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Tensao = table.Column<decimal>(type: "numeric", nullable: false),
                    Corrente = table.Column<decimal>(type: "numeric", nullable: false),
                    PotenciaAtiva = table.Column<decimal>(type: "numeric", nullable: false),
                    PotenciaReativa = table.Column<decimal>(type: "numeric", nullable: false),
                    EnergiaAtivaDireta = table.Column<decimal>(type: "numeric", nullable: false),
                    EnergiaAtivaReversa = table.Column<decimal>(type: "numeric", nullable: false),
                    FatorPotencia = table.Column<decimal>(type: "numeric", nullable: false),
                    Frequencia = table.Column<decimal>(type: "numeric", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Leituras", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Leituras_Medidores_MedidorId",
                        column: x => x.MedidorId,
                        principalTable: "Medidores",
                        principalColumn: "MedidorId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Leituras_MedidorId",
                table: "Leituras",
                column: "MedidorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Leituras");

            migrationBuilder.DropTable(
                name: "Medidores");
        }
    }
}
