using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DataCollect.EntityFramework.Migrations.Migrations
{
    public partial class v100 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CollectPlcData",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    OpcValue = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    Message = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    IpAddress = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    DeviceCode = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: true),
                    IType = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true),
                    Value = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    OldValue = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeviceType = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true),
                    DeviceNumber = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true),
                    ComponentPropertyType = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true),
                    ComponentProperty = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CollectPlcData", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MqttConnect",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    port = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    password = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Ispassword = table.Column<bool>(type: "bit", nullable: false),
                    IsHeartCheck = table.Column<bool>(type: "bit", nullable: false),
                    HeartContent = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SubscribeName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    creator = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    creatTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    createMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    updater = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    updateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updateMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    finisher = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    finishTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    finishMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    objectStatus = table.Column<int>(type: "int", nullable: false),
                    workStatus = table.Column<int>(type: "int", nullable: false),
                    deleteFlag = table.Column<bool>(type: "bit", nullable: false),
                    dataVersion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    standby1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    standby2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    standby3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    standby4 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    standby5 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    comments = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MqttConnect", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Role",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    Remark = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Enabled = table.Column<bool>(type: "bit", nullable: false),
                    CreatedTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Role", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Security",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UniqueName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Remark = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Enabled = table.Column<bool>(type: "bit", nullable: false),
                    CreatedTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Security", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SystemConfiguration",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    creator = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    creatTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    createMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    updater = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    updateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updateMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    finisher = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    finishTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    finishMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    objectStatus = table.Column<int>(type: "int", nullable: false),
                    workStatus = table.Column<int>(type: "int", nullable: false),
                    deleteFlag = table.Column<bool>(type: "bit", nullable: false),
                    dataVersion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    standby1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    standby2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    standby3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    standby4 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    standby5 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    comments = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemConfiguration", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SystemDataCategory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    Remark = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Sequence = table.Column<int>(type: "int", nullable: false),
                    HigherId = table.Column<int>(type: "int", nullable: true),
                    Enabled = table.Column<bool>(type: "bit", nullable: false),
                    CreatedTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemDataCategory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SystemDataCategory_SystemDataCategory_HigherId",
                        column: x => x.HigherId,
                        principalTable: "SystemDataCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Account = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    Nickname = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    Photo = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Synopsis = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Gender = table.Column<int>(type: "int", nullable: false),
                    SigninedTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Enabled = table.Column<bool>(type: "bit", nullable: false),
                    CreatedTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RoleSecurity",
                columns: table => new
                {
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    SecurityId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleSecurity", x => new { x.RoleId, x.SecurityId });
                    table.ForeignKey(
                        name: "FK_RoleSecurity_Role_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoleSecurity_Security_SecurityId",
                        column: x => x.SecurityId,
                        principalTable: "Security",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SystemData",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    Remark = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Sequence = table.Column<int>(type: "int", nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    Enabled = table.Column<bool>(type: "bit", nullable: false),
                    CreatedTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemData", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SystemData_SystemDataCategory_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "SystemDataCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserRole",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRole", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_UserRole_Role_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRole_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "MqttConnect",
                columns: new[] { "Id", "HeartContent", "IsHeartCheck", "Ispassword", "SubscribeName", "UserName", "comments", "creatTime", "createMessage", "creator", "dataVersion", "deleteFlag", "finishMessage", "finishTime", "finisher", "objectStatus", "password", "port", "standby1", "standby2", "standby3", "standby4", "standby5", "updateMessage", "updateTime", "updater", "workStatus" },
                values: new object[] { "9a1e30880eaf4ceabf22eed19cabe754", null, false, false, "Kengic", "Kengic", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 0, "Kengic@123", "1003", null, null, null, null, null, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 0 });

            migrationBuilder.InsertData(
                table: "Role",
                columns: new[] { "Id", "CreatedTime", "Enabled", "IsDeleted", "Name", "Remark", "UpdatedTime" },
                values: new object[,]
                {
                    { 1, new DateTimeOffset(new DateTime(2020, 12, 17, 10, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 8, 0, 0, 0)), true, false, "超级管理员", "拥有所有权限", null },
                    { 2, new DateTimeOffset(new DateTime(2020, 12, 17, 10, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 8, 0, 0, 0)), true, false, "测试用户", "只有测试权限", null }
                });

            migrationBuilder.InsertData(
                table: "Security",
                columns: new[] { "Id", "CreatedTime", "Enabled", "IsDeleted", "Remark", "UniqueName", "UpdatedTime" },
                values: new object[,]
                {
                    { 21, new DateTimeOffset(new DateTime(2020, 12, 22, 10, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 8, 0, 0, 0)), true, false, "security.service.all", "security.service.all", null },
                    { 20, new DateTimeOffset(new DateTime(2020, 12, 22, 10, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 8, 0, 0, 0)), true, false, "security.service.refresh", "security.service.refresh", null },
                    { 19, new DateTimeOffset(new DateTime(2020, 12, 22, 10, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 8, 0, 0, 0)), true, false, "security.service.give", "security.service.give", null },
                    { 18, new DateTimeOffset(new DateTime(2020, 12, 22, 10, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 8, 0, 0, 0)), true, false, "security.service.list", "security.service.list", null },
                    { 17, new DateTimeOffset(new DateTime(2020, 12, 22, 10, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 8, 0, 0, 0)), true, false, "role.service.give", "role.service.give", null },
                    { 16, new DateTimeOffset(new DateTime(2020, 12, 22, 10, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 8, 0, 0, 0)), true, false, "role.service.delete", "role.service.delete", null },
                    { 15, new DateTimeOffset(new DateTime(2020, 12, 22, 10, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 8, 0, 0, 0)), true, false, "role.service.modify", "role.service.modify", null },
                    { 14, new DateTimeOffset(new DateTime(2020, 12, 22, 10, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 8, 0, 0, 0)), true, false, "role.service.add", "role.service.add", null },
                    { 13, new DateTimeOffset(new DateTime(2020, 12, 22, 10, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 8, 0, 0, 0)), true, false, "role.service.list", "role.service.list", null },
                    { 12, new DateTimeOffset(new DateTime(2020, 12, 22, 10, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 8, 0, 0, 0)), true, false, "user.service.securities", "user.service.securities", null },
                    { 10, new DateTimeOffset(new DateTime(2020, 12, 22, 10, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 8, 0, 0, 0)), true, false, "user.service.roles", "user.service.roles", null },
                    { 9, new DateTimeOffset(new DateTime(2020, 12, 22, 10, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 8, 0, 0, 0)), true, false, "user.service.roles.self", "user.service.roles.self", null },
                    { 8, new DateTimeOffset(new DateTime(2020, 12, 22, 10, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 8, 0, 0, 0)), true, false, "user.service.change.password", "user.service.change.password", null },
                    { 7, new DateTimeOffset(new DateTime(2020, 12, 22, 10, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 8, 0, 0, 0)), true, false, "user.service.add", "user.service.add", null },
                    { 6, new DateTimeOffset(new DateTime(2020, 12, 22, 10, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 8, 0, 0, 0)), true, false, "user.service.delete", "user.service.delete", null },
                    { 5, new DateTimeOffset(new DateTime(2020, 12, 22, 10, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 8, 0, 0, 0)), true, false, "user.service.modify", "user.service.modify", null },
                    { 4, new DateTimeOffset(new DateTime(2020, 12, 22, 10, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 8, 0, 0, 0)), true, false, "user.service.modify.self", "user.service.modify.self", null },
                    { 3, new DateTimeOffset(new DateTime(2020, 12, 22, 10, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 8, 0, 0, 0)), true, false, "user.service.list", "user.service.list", null },
                    { 2, new DateTimeOffset(new DateTime(2020, 12, 22, 10, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 8, 0, 0, 0)), true, false, "user.service.profile", "user.service.profile", null },
                    { 1, new DateTimeOffset(new DateTime(2020, 12, 22, 10, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 8, 0, 0, 0)), true, false, "user.service.profile.self", "user.service.profile.self", null },
                    { 11, new DateTimeOffset(new DateTime(2020, 12, 22, 10, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 8, 0, 0, 0)), true, false, "user.service.securities.self", "user.service.securities.self", null }
                });

            migrationBuilder.InsertData(
                table: "SystemDataCategory",
                columns: new[] { "Id", "CreatedTime", "Enabled", "HigherId", "IsDeleted", "Name", "Remark", "Sequence", "UpdatedTime" },
                values: new object[] { 1, new DateTimeOffset(new DateTime(2020, 12, 22, 15, 30, 20, 0, DateTimeKind.Unspecified), new TimeSpan(0, 8, 0, 0, 0)), true, null, false, "性别", "性别", 0, null });

            migrationBuilder.InsertData(
                table: "User",
                columns: new[] { "Id", "Account", "CreatedTime", "Enabled", "Gender", "IsDeleted", "Nickname", "Password", "Photo", "SigninedTime", "Synopsis", "UpdatedTime" },
                values: new object[] { 1, "admin", new DateTimeOffset(new DateTime(2020, 12, 17, 10, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 8, 0, 0, 0)), true, 0, false, null, "e10adc3949ba59abbe56e057f20f883e", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null });

            migrationBuilder.InsertData(
                table: "RoleSecurity",
                columns: new[] { "RoleId", "SecurityId" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 1, 21 },
                    { 1, 20 },
                    { 1, 19 },
                    { 1, 18 },
                    { 1, 17 },
                    { 1, 16 },
                    { 1, 15 },
                    { 1, 14 },
                    { 1, 13 },
                    { 1, 12 },
                    { 1, 10 },
                    { 1, 9 },
                    { 1, 8 },
                    { 1, 7 },
                    { 1, 6 },
                    { 1, 5 },
                    { 1, 4 },
                    { 1, 3 },
                    { 1, 2 },
                    { 1, 11 }
                });

            migrationBuilder.InsertData(
                table: "SystemData",
                columns: new[] { "Id", "CategoryId", "CreatedTime", "Enabled", "IsDeleted", "Name", "Remark", "Sequence", "UpdatedTime" },
                values: new object[,]
                {
                    { 2, 1, new DateTimeOffset(new DateTime(2020, 12, 22, 15, 30, 20, 0, DateTimeKind.Unspecified), new TimeSpan(0, 8, 0, 0, 0)), true, false, "女", "女", 1, null },
                    { 1, 1, new DateTimeOffset(new DateTime(2020, 12, 22, 15, 30, 20, 0, DateTimeKind.Unspecified), new TimeSpan(0, 8, 0, 0, 0)), true, false, "男", "男", 0, null }
                });

            migrationBuilder.InsertData(
                table: "UserRole",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { 1, 1 });

            migrationBuilder.CreateIndex(
                name: "IX_RoleSecurity_SecurityId",
                table: "RoleSecurity",
                column: "SecurityId");

            migrationBuilder.CreateIndex(
                name: "IX_SystemData_CategoryId",
                table: "SystemData",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_SystemDataCategory_HigherId",
                table: "SystemDataCategory",
                column: "HigherId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRole_RoleId",
                table: "UserRole",
                column: "RoleId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CollectPlcData");

            migrationBuilder.DropTable(
                name: "MqttConnect");

            migrationBuilder.DropTable(
                name: "RoleSecurity");

            migrationBuilder.DropTable(
                name: "SystemConfiguration");

            migrationBuilder.DropTable(
                name: "SystemData");

            migrationBuilder.DropTable(
                name: "UserRole");

            migrationBuilder.DropTable(
                name: "Security");

            migrationBuilder.DropTable(
                name: "SystemDataCategory");

            migrationBuilder.DropTable(
                name: "Role");

            migrationBuilder.DropTable(
                name: "User");
        }
    }
}
