using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Exercises.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMuscleGroup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                CREATE TABLE [exercises].[Exercises_New] (
                    [Id]                   uniqueidentifier NOT NULL,
                    [Name]                 nvarchar(150)    NOT NULL,
                    [PrimaryMuscleGroup]   nvarchar(50)     NOT NULL DEFAULT '',
                    [SecondaryMuscleGroup]  nvarchar(50)     NULL,
                    [Description]          nvarchar(500)    NULL,
                    [IsActive]             bit              NOT NULL,
                    [IsDeleted]            bit              NOT NULL,
                    [CreatedDate]          datetime2        NOT NULL,
                    [CreatedBy]            nvarchar(100)    NULL,
                    [UpdatedDate]          datetime2        NULL,
                    [UpdatedBy]            nvarchar(100)    NULL,
                    CONSTRAINT [PK_Exercises_New] PRIMARY KEY ([Id])
                );

                INSERT INTO [exercises].[Exercises_New]
                       ([Id], [Name], [PrimaryMuscleGroup], [Description],
                        [IsActive], [IsDeleted], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy])
                SELECT [Id], [Name], [MuscleGroup], [Description],
                       [IsActive], [IsDeleted], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]
                FROM [exercises].[Exercises];

                DROP TABLE [exercises].[Exercises];

                EXEC sp_rename '[exercises].[Exercises_New]',    'Exercises';
                EXEC sp_rename '[exercises].[PK_Exercises_New]', 'PK_Exercises', 'OBJECT';
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                CREATE TABLE [exercises].[Exercises_Old] (
                    [Id]           uniqueidentifier NOT NULL,
                    [Name]         nvarchar(150)    NOT NULL,
                    [MuscleGroup]  nvarchar(100)    NOT NULL DEFAULT '',
                    [Description]  nvarchar(500)    NULL,
                    [IsActive]     bit              NOT NULL,
                    [IsDeleted]    bit              NOT NULL,
                    [CreatedDate]  datetime2        NOT NULL,
                    [CreatedBy]    nvarchar(100)    NULL,
                    [UpdatedDate]  datetime2        NULL,
                    [UpdatedBy]    nvarchar(100)    NULL,
                    CONSTRAINT [PK_Exercises_Old] PRIMARY KEY ([Id])
                );

                INSERT INTO [exercises].[Exercises_Old]
                       ([Id], [Name], [MuscleGroup], [Description],
                        [IsActive], [IsDeleted], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy])
                SELECT [Id], [Name], [PrimaryMuscleGroup], [Description],
                       [IsActive], [IsDeleted], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]
                FROM [exercises].[Exercises];

                DROP TABLE [exercises].[Exercises];

                EXEC sp_rename '[exercises].[Exercises_Old]',    'Exercises';
                EXEC sp_rename '[exercises].[PK_Exercises_Old]', 'PK_Exercises', 'OBJECT';
                """);
        }
    }
}
