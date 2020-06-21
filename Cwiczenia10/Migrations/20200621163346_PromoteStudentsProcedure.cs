using Microsoft.EntityFrameworkCore.Migrations;

namespace Cwiczenia10.Migrations
{
    public partial class PromoteStudentsProcedure : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
			string procedure = @"CREATE PROCEDURE PromoteStudents
						 			 @studies NVARCHAR(100),
									 @semester INT
								 AS
								 BEGIN
									 DECLARE @id_study INT;
									 DECLARE @id_enrollment_old INT;
									 DECLARE @id_enrollment_new INT;

									 SET @id_study = (SELECT IdStudy FROM Studies WHERE Name = @studies);
									 SET @id_enrollment_old = (SELECT IdEnrollment FROM Enrollment WHERE Semester = @semester AND IdStudy = @id_study);

									 IF NOT EXISTS (SELECT * FROM Enrollment WHERE Semester = @semester + 1 AND IdStudy = @id_study)
										 BEGIN
											 SET @id_enrollment_new = (SELECT MAX(IdEnrollment) + 1 FROM Enrollment);	
											 INSERT INTO Enrollment VALUES(@id_enrollment_new, @semester + 1, @id_study, GETDATE()); 
										 END
									 ELSE
										 BEGIN
											 SET @id_enrollment_new = (SELECT IdEnrollment FROM Enrollment WHERE Semester = @semester + 1 AND IdStudy = @id_study);
										 END

									 UPDATE Student SET IdEnrollment = @id_enrollment_new WHERE IdEnrollment = @id_enrollment_old;
									 SELECT * FROM Enrollment WHERE IdEnrollment = @id_enrollment_new;
								 END;";

			migrationBuilder.Sql(procedure);
		}

        protected override void Down(MigrationBuilder migrationBuilder)
        {
			string procedure = @"DROP PROCEDURE PromoteStudents;";

			migrationBuilder.Sql(procedure);
        }
    }
}
