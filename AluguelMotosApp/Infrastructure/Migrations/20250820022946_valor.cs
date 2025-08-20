using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class valor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImagemCnhPath",
                table: "Entregador");

            migrationBuilder.AddColumn<DateTime>(
                name: "DataPrevisaoTermino",
                table: "Locacao",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<decimal>(
                name: "valorTotalLocacao",
                table: "Locacao",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateIndex(
                name: "IX_Entregador_Cnpj",
                table: "Entregador",
                column: "Cnpj",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Entregador_NumeroCnh",
                table: "Entregador",
                column: "NumeroCnh",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Entregador_Cnpj",
                table: "Entregador");

            migrationBuilder.DropIndex(
                name: "IX_Entregador_NumeroCnh",
                table: "Entregador");

            migrationBuilder.DropColumn(
                name: "DataPrevisaoTermino",
                table: "Locacao");

            migrationBuilder.DropColumn(
                name: "valorTotalLocacao",
                table: "Locacao");

            migrationBuilder.AddColumn<string>(
                name: "ImagemCnhPath",
                table: "Entregador",
                type: "text",
                nullable: true);
        }
    }
}
