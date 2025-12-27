using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Dalmarkit.Sample.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ApiLogs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ActionName = table.Column<string>(type: "text", nullable: true),
                    EventType = table.Column<string>(type: "text", nullable: false),
                    ResponseStatusCode = table.Column<int>(type: "integer", nullable: false),
                    Url = table.Column<string>(type: "text", nullable: false),
                    UserIp = table.Column<string>(type: "text", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now() at time zone 'utc'"),
                    DurationMsec = table.Column<int>(type: "integer", nullable: false),
                    LogDetail = table.Column<string>(type: "jsonb", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now() at time zone 'utc'"),
                    Status = table.Column<bool>(type: "boolean", nullable: false),
                    TraceId = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AuditScopeId = table.Column<Guid>(type: "uuid", nullable: false),
                    Action = table.Column<string>(type: "text", nullable: false),
                    ChangedValues = table.Column<string>(type: "jsonb", nullable: true),
                    Error = table.Column<string>(type: "text", nullable: true),
                    PrimaryKey = table.Column<string>(type: "text", nullable: false),
                    Table = table.Column<string>(type: "text", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now() at time zone 'utc'"),
                    DurationMsec = table.Column<int>(type: "integer", nullable: false),
                    LogDetail = table.Column<string>(type: "jsonb", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now() at time zone 'utc'"),
                    Status = table.Column<bool>(type: "boolean", nullable: false),
                    TraceId = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Entities",
                columns: table => new
                {
                    EntityId = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    EntityName = table.Column<string>(type: "text", nullable: false),
                    AppClientId = table.Column<string>(type: "text", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now() at time zone 'utc'"),
                    CreateRequestId = table.Column<string>(type: "text", nullable: false),
                    CreatorId = table.Column<string>(type: "text", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now() at time zone 'utc'"),
                    ModifierId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Entities", x => x.EntityId);
                });

            migrationBuilder.CreateTable(
                name: "EvmEvents",
                columns: table => new
                {
                    EvmEventId = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    EventName = table.Column<string>(type: "text", nullable: false),
                    ContractAddress = table.Column<string>(type: "text", nullable: false),
                    TransactionHash = table.Column<string>(type: "text", nullable: false),
                    BlockchainNetwork = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    EventDetail = table.Column<string>(type: "jsonb", nullable: false),
                    AppClientId = table.Column<string>(type: "text", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now() at time zone 'utc'"),
                    CreateRequestId = table.Column<string>(type: "text", nullable: false),
                    CreatorId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EvmEvents", x => x.EvmEventId);
                });

            migrationBuilder.CreateTable(
                name: "DependentEntities",
                columns: table => new
                {
                    DependentEntityId = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    DependentEntityName = table.Column<string>(type: "text", nullable: false),
                    EntityId = table.Column<Guid>(type: "uuid", nullable: false),
                    AppClientId = table.Column<string>(type: "text", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now() at time zone 'utc'"),
                    CreateRequestId = table.Column<string>(type: "text", nullable: false),
                    CreatorId = table.Column<string>(type: "text", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now() at time zone 'utc'"),
                    ModifierId = table.Column<string>(type: "text", nullable: false),
                    EntityHash = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DependentEntities", x => x.DependentEntityId);
                    table.ForeignKey(
                        name: "FK_DependentEntities_Entities_EntityId",
                        column: x => x.EntityId,
                        principalTable: "Entities",
                        principalColumn: "EntityId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EntityImages",
                columns: table => new
                {
                    EntityImageId = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    EntityId = table.Column<Guid>(type: "uuid", nullable: false),
                    AppClientId = table.Column<string>(type: "text", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now() at time zone 'utc'"),
                    CreateRequestId = table.Column<string>(type: "text", nullable: false),
                    CreatorId = table.Column<string>(type: "text", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now() at time zone 'utc'"),
                    ModifierId = table.Column<string>(type: "text", nullable: false),
                    ObjectExtension = table.Column<string>(type: "text", nullable: false),
                    ObjectName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntityImages", x => x.EntityImageId);
                    table.ForeignKey(
                        name: "FK_EntityImages_Entities_EntityId",
                        column: x => x.EntityId,
                        principalTable: "Entities",
                        principalColumn: "EntityId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApiLogs_ActionName",
                table: "ApiLogs",
                column: "ActionName");

            migrationBuilder.CreateIndex(
                name: "IX_ApiLogs_CreatedOn",
                table: "ApiLogs",
                column: "CreatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_ApiLogs_EventType",
                table: "ApiLogs",
                column: "EventType");

            migrationBuilder.CreateIndex(
                name: "IX_ApiLogs_ModifiedOn",
                table: "ApiLogs",
                column: "ModifiedOn");

            migrationBuilder.CreateIndex(
                name: "IX_ApiLogs_ResponseStatusCode",
                table: "ApiLogs",
                column: "ResponseStatusCode");

            migrationBuilder.CreateIndex(
                name: "IX_ApiLogs_TraceId",
                table: "ApiLogs",
                column: "TraceId");

            migrationBuilder.CreateIndex(
                name: "IX_ApiLogs_Url",
                table: "ApiLogs",
                column: "Url");

            migrationBuilder.CreateIndex(
                name: "IX_ApiLogs_UserId",
                table: "ApiLogs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ApiLogs_UserIp",
                table: "ApiLogs",
                column: "UserIp");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_ChangedValues",
                table: "AuditLogs",
                column: "ChangedValues")
                .Annotation("Npgsql:IndexMethod", "gin");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_CreatedOn",
                table: "AuditLogs",
                column: "CreatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_ModifiedOn",
                table: "AuditLogs",
                column: "ModifiedOn");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_PrimaryKey",
                table: "AuditLogs",
                column: "PrimaryKey");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_Table_PrimaryKey",
                table: "AuditLogs",
                columns: new[] { "Table", "PrimaryKey" });

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_TraceId",
                table: "AuditLogs",
                column: "TraceId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_UserId",
                table: "AuditLogs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_DependentEntities_AppClientId",
                table: "DependentEntities",
                column: "AppClientId");

            migrationBuilder.CreateIndex(
                name: "IX_DependentEntities_CreateRequestId_AppClientId_EntityHash",
                table: "DependentEntities",
                columns: new[] { "CreateRequestId", "AppClientId", "EntityHash" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DependentEntities_CreatedOn",
                table: "DependentEntities",
                column: "CreatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_DependentEntities_CreatorId",
                table: "DependentEntities",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_DependentEntities_DependentEntityName_EntityId",
                table: "DependentEntities",
                columns: new[] { "DependentEntityName", "EntityId" },
                unique: true,
                filter: "\"IsDeleted\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_DependentEntities_EntityId",
                table: "DependentEntities",
                column: "EntityId");

            migrationBuilder.CreateIndex(
                name: "IX_DependentEntities_ModifiedOn",
                table: "DependentEntities",
                column: "ModifiedOn");

            migrationBuilder.CreateIndex(
                name: "IX_DependentEntities_ModifierId",
                table: "DependentEntities",
                column: "ModifierId");

            migrationBuilder.CreateIndex(
                name: "IX_Entities_AppClientId",
                table: "Entities",
                column: "AppClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Entities_CreateRequestId_AppClientId",
                table: "Entities",
                columns: new[] { "CreateRequestId", "AppClientId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Entities_CreatedOn",
                table: "Entities",
                column: "CreatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Entities_CreatorId",
                table: "Entities",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_Entities_EntityName",
                table: "Entities",
                column: "EntityName",
                unique: true,
                filter: "\"IsDeleted\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_Entities_ModifiedOn",
                table: "Entities",
                column: "ModifiedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Entities_ModifierId",
                table: "Entities",
                column: "ModifierId");

            migrationBuilder.CreateIndex(
                name: "IX_EntityImages_AppClientId",
                table: "EntityImages",
                column: "AppClientId");

            migrationBuilder.CreateIndex(
                name: "IX_EntityImages_CreateRequestId_AppClientId",
                table: "EntityImages",
                columns: new[] { "CreateRequestId", "AppClientId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EntityImages_CreatedOn",
                table: "EntityImages",
                column: "CreatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_EntityImages_CreatorId",
                table: "EntityImages",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_EntityImages_EntityId",
                table: "EntityImages",
                column: "EntityId");

            migrationBuilder.CreateIndex(
                name: "IX_EntityImages_ModifiedOn",
                table: "EntityImages",
                column: "ModifiedOn");

            migrationBuilder.CreateIndex(
                name: "IX_EntityImages_ModifierId",
                table: "EntityImages",
                column: "ModifierId");

            migrationBuilder.CreateIndex(
                name: "IX_EntityImages_ObjectName_EntityId",
                table: "EntityImages",
                columns: new[] { "ObjectName", "EntityId" },
                unique: true,
                filter: "\"IsDeleted\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_EvmEvents_AppClientId",
                table: "EvmEvents",
                column: "AppClientId");

            migrationBuilder.CreateIndex(
                name: "IX_EvmEvents_CreateRequestId_AppClientId",
                table: "EvmEvents",
                columns: new[] { "CreateRequestId", "AppClientId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EvmEvents_CreatedOn",
                table: "EvmEvents",
                column: "CreatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_EvmEvents_CreatorId",
                table: "EvmEvents",
                column: "CreatorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApiLogs");

            migrationBuilder.DropTable(
                name: "AuditLogs");

            migrationBuilder.DropTable(
                name: "DependentEntities");

            migrationBuilder.DropTable(
                name: "EntityImages");

            migrationBuilder.DropTable(
                name: "EvmEvents");

            migrationBuilder.DropTable(
                name: "Entities");
        }
    }
}
