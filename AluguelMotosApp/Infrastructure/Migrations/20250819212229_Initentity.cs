using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initentity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Locacoes",
                table: "Moto");

            migrationBuilder.CreateTable(
                name: "Entregador",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Nome = table.Column<string>(type: "text", nullable: false),
                    Cnpj = table.Column<string>(type: "text", nullable: false),
                    DataNascimento = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    NumeroCnh = table.Column<string>(type: "text", nullable: false),
                    TipoCnh = table.Column<string>(type: "text", nullable: false),
                    ImagemCnhPath = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Entregador", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Notificacoes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Placa = table.Column<string>(type: "text", nullable: false),
                    Ano = table.Column<int>(type: "integer", nullable: false),
                    Mensagem = table.Column<string>(type: "text", nullable: false),
                    DataEvento = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notificacoes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Locacao",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MotoId = table.Column<string>(type: "text", nullable: false),
                    EntregadorId = table.Column<Guid>(type: "uuid", nullable: false),
                    DataInicio = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DataFim = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Locacao", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Locacao_Entregador_EntregadorId",
                        column: x => x.EntregadorId,
                        principalTable: "Entregador",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Locacao_Moto_MotoId",
                        column: x => x.MotoId,
                        principalTable: "Moto",
                        principalColumn: "Identificador",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Locacao_EntregadorId",
                table: "Locacao",
                column: "EntregadorId");

            migrationBuilder.CreateIndex(
                name: "IX_Locacao_MotoId",
                table: "Locacao",
                column: "MotoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Locacao");

            migrationBuilder.DropTable(
                name: "Notificacoes");

            migrationBuilder.DropTable(
                name: "Entregador");

            migrationBuilder.AddColumn<int>(
                name: "Locacoes",
                table: "Moto",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
