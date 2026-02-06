using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkoutPrograms.Infrastructure.Migrations
{
    public partial class MinMaxRepAdd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1) Eski kolon adını rename et (veri kaybetmeden)
            migrationBuilder.RenameColumn(
                name: "TargetReps",
                schema: "workoutprograms",
                table: "WorkoutSplitExercises",
                newName: "MinimumReps");

            // 2) MaximumReps'i 4. kolona almak için tabloyu yeniden oluştur (SQL Server)
            migrationBuilder.Sql("""
                SET XACT_ABORT ON;

                IF OBJECT_ID(N'[workoutprograms].[WorkoutSplitExercises]', N'U') IS NULL
                    THROW 50000, 'Table [workoutprograms].[WorkoutSplitExercises] not found.', 1;

                -- Varsa temp tabloyu temizle
                IF OBJECT_ID(N'[workoutprograms].[WorkoutSplitExercises__temp]', N'U') IS NOT NULL
                    DROP TABLE [workoutprograms].[WorkoutSplitExercises__temp];

                /*
                  Mevcut tablo kolonları (designer/config'e göre):
                  Id, ExerciseId, WorkoutProgramSplitId, Sets, CreatedBy, CreatedDate,MinimumReps (rename sonrası),İstenen: MaximumReps, IsActive, IsDeleted, UpdatedBy, UpdatedDate                  
                */
                CREATE TABLE [workoutprograms].[WorkoutSplitExercises__temp]
                (
                    [Id] uniqueidentifier NOT NULL,
                    [ExerciseId] uniqueidentifier NOT NULL,
                    [WorkoutProgramSplitId] uniqueidentifier NOT NULL,
                    [Sets] int NOT NULL,
                    [MinimumReps] int NOT NULL,
                    [MaximumReps] int NOT NULL CONSTRAINT [DF_WorkoutSplitExercises_MaximumReps] DEFAULT (0),
                    [IsActive] bit NOT NULL,
                    [IsDeleted] bit NOT NULL,
                    [CreatedBy] nvarchar(max) NULL,
                    [CreatedDate] datetime2 NULL,
                    [UpdatedBy] nvarchar(max) NULL,
                    [UpdatedDate] datetime2 NULL,                   
                    CONSTRAINT [PK_WorkoutSplitExercises__temp] PRIMARY KEY ([Id])
                );

                -- Veriyi kopyala (MaximumReps yeni kolon: default 0)
                INSERT INTO [workoutprograms].[WorkoutSplitExercises__temp]
                (
                    [Id],
                    [ExerciseId],
                    [WorkoutProgramSplitId],
                    [Sets],
                    [MinimumReps],
                    [MaximumReps],
                    [IsActive],
                    [IsDeleted],
                    [CreatedBy],
                    [CreatedDate],
                    [UpdatedBy],
                    [UpdatedDate]
                )
                SELECT
                    [Id],
                    [ExerciseId],
                    [WorkoutProgramSplitId],
                    [Sets],
                    [MinimumReps],
                    0 AS [MaximumReps],                    
                    [IsActive],
                    [IsDeleted],
                    [CreatedBy],
                    [CreatedDate],                                     
                    [UpdatedBy],
                    [UpdatedDate]
                    
                FROM [workoutprograms].[WorkoutSplitExercises];

                DROP TABLE [workoutprograms].[WorkoutSplitExercises];

                EXEC sp_rename N'[workoutprograms].[WorkoutSplitExercises__temp]', N'WorkoutSplitExercises';

                -- Index'i geri oluştur (designer'da HasIndex(WorkoutProgramSplitId) var)
                CREATE INDEX [IX_WorkoutSplitExercises_WorkoutProgramSplitId]
                    ON [workoutprograms].[WorkoutSplitExercises]([WorkoutProgramSplitId]);

                -- PK adını da standartlaştır
                EXEC sp_rename N'[workoutprograms].[PK_WorkoutSplitExercises__temp]', N'PK_WorkoutSplitExercises', N'OBJECT';
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Down için de aynı şekilde tabloyu eski haline (MaximumReps yok) döndürmek gerekir
            migrationBuilder.Sql("""
                SET XACT_ABORT ON;

                IF OBJECT_ID(N'[workoutprograms].[WorkoutSplitExercises]', N'U') IS NULL
                    THROW 50000, 'Table [workoutprograms].[WorkoutSplitExercises] not found.', 1;

                IF OBJECT_ID(N'[workoutprograms].[WorkoutSplitExercises__temp]', N'U') IS NOT NULL
                    DROP TABLE [workoutprograms].[WorkoutSplitExercises__temp];

                CREATE TABLE [workoutprograms].[WorkoutSplitExercises__temp]
                (
                    [Id] uniqueidentifier NOT NULL,
                    [CreatedBy] nvarchar(max) NULL,
                    [CreatedDate] datetime2 NULL,
                    [ExerciseId] uniqueidentifier NOT NULL,
                    [IsActive] bit NOT NULL,
                    [IsDeleted] bit NOT NULL,
                    [TargetReps] int NOT NULL,
                    [Sets] int NOT NULL,
                    [UpdatedBy] nvarchar(max) NULL,
                    [UpdatedDate] datetime2 NULL,
                    [WorkoutProgramSplitId] uniqueidentifier NOT NULL,

                    CONSTRAINT [PK_WorkoutSplitExercises__temp] PRIMARY KEY ([Id])
                );

                INSERT INTO [workoutprograms].[WorkoutSplitExercises__temp]
                (
                    [Id],
                    [CreatedBy],
                    [CreatedDate],
                    [ExerciseId],
                    [IsActive],
                    [IsDeleted],
                    [TargetReps],
                    [Sets],
                    [UpdatedBy],
                    [UpdatedDate],
                    [WorkoutProgramSplitId]
                )
                SELECT
                    [Id],
                    [CreatedBy],
                    [CreatedDate],
                    [ExerciseId],
                    [IsActive],
                    [IsDeleted],
                    [MinimumReps] AS [TargetReps],
                    [Sets],
                    [UpdatedBy],
                    [UpdatedDate],
                    [WorkoutProgramSplitId]
                FROM [workoutprograms].[WorkoutSplitExercises];

                DROP TABLE [workoutprograms].[WorkoutSplitExercises];

                EXEC sp_rename N'[workoutprograms].[WorkoutSplitExercises__temp]', N'WorkoutSplitExercises';

                CREATE INDEX [IX_WorkoutSplitExercises_WorkoutProgramSplitId]
                    ON [workoutprograms].[WorkoutSplitExercises]([WorkoutProgramSplitId]);

                EXEC sp_rename N'[workoutprograms].[PK_WorkoutSplitExercises__temp]', N'PK_WorkoutSplitExercises', N'OBJECT';
                """);
        }
    }
}
