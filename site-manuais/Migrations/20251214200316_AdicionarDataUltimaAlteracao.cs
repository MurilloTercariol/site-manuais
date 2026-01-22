using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace site_manuais.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarDataUltimaAlteracao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DataAtualizacao",
                table: "Documentos",
                newName: "DataUltimaAlteracao");

            migrationBuilder.AlterColumn<string>(
                name: "Nome",
                table: "Modulos",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Descricao",
                table: "Modulos",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DataUltimaAlteracao",
                table: "Modulos",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DataUltimaAlteracao",
                table: "Categorias",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DataUltimaAlteracao",
                table: "Modulos");

            migrationBuilder.DropColumn(
                name: "DataUltimaAlteracao",
                table: "Categorias");

            migrationBuilder.RenameColumn(
                name: "DataUltimaAlteracao",
                table: "Documentos",
                newName: "DataAtualizacao");

            migrationBuilder.AlterColumn<string>(
                name: "Nome",
                table: "Modulos",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Descricao",
                table: "Modulos",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);
        }
    }
}
