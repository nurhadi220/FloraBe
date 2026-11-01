using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace floral.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMemberNumberAndUsername : Migration
    {
        /// <inheritdoc />
protected override void Up(MigrationBuilder migrationBuilder)
{
    // Tambahkan kolom sementara sebagai nullable
    migrationBuilder.AddColumn<int>(
        name: "member_number",
        table: "users",
        type: "int",
        nullable: true);

    migrationBuilder.AddColumn<string>(
        name: "username",
        table: "users",
        type: "varchar(30)",
        nullable: true);

    // Isi data lama menggunakan ID yang sudah ada.
    // ID user lama tidak berubah.
    migrationBuilder.Sql(
        """
        UPDATE users
        SET
            member_number = id,
            username = CONCAT('user', LPAD(id, 4, '0'))
        """);

    // Setelah data lama terisi, ubah kolom menjadi wajib.
    migrationBuilder.AlterColumn<int>(
        name: "member_number",
        table: "users",
        type: "int",
        nullable: false,
        oldClrType: typeof(int),
        oldType: "int",
        oldNullable: true);

    migrationBuilder.AlterColumn<string>(
        name: "username",
        table: "users",
        type: "varchar(30)",
        nullable: false,
        oldClrType: typeof(string),
        oldType: "varchar(30)",
        oldNullable: true);

    // Unique index
    migrationBuilder.CreateIndex(
        name: "ux_users_member_number",
        table: "users",
        column: "member_number",
        unique: true);

    migrationBuilder.CreateIndex(
        name: "ux_users_username",
        table: "users",
        column: "username",
        unique: true);
}
        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
{
    migrationBuilder.DropIndex(
        name: "ux_users_member_number",
        table: "users");

    migrationBuilder.DropIndex(
        name: "ux_users_username",
        table: "users");

    migrationBuilder.DropColumn(
        name: "member_number",
        table: "users");

    migrationBuilder.DropColumn(
        name: "username",
        table: "users");
}
    }
}
