using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using SystemOcenianiaSimple.Data;
using SystemOcenianiaSimple.Models;

namespace SystemOcenianiaSimple.Repositories
{
    public class GradebookRepository
    {
        public List<GradebookEntry> GetByClassId(int classId)
        {
            var list = new List<GradebookEntry>();
            using var conn = Db.Open();

            var sql = @"
SELECT
    e.EnrollmentID,
    s.StudentID,
    (s.FirstName + ' ' + s.LastName) AS StudentName,
    s.IndexNo,
    a.AssessmentID,
    a.Title,
    a.GradingMode,
    g.GradeValue,
    g.Points,
    g.IsAbsent
FROM Enrollments e
JOIN Students s ON e.StudentID = s.StudentID
JOIN Assessments a ON a.ClassID = e.ClassID
LEFT JOIN Grades g ON g.EnrollmentID = e.EnrollmentID AND g.AssessmentID = a.AssessmentID
WHERE e.ClassID = @classId
ORDER BY s.LastName, s.FirstName, a.Title;
";

            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@classId", classId);

            using var r = cmd.ExecuteReader();
            while (r.Read())
            {
                var entry = new GradebookEntry();

                entry.EnrollmentId = r.GetInt32(0);
                entry.StudentId = r.GetInt32(1);
                entry.StudentName = r.GetString(2);
                entry.IndexNo = r.GetString(3);
                entry.AssessmentId = r.GetInt32(4);
                entry.AssessmentTitle = r.GetString(5);
                entry.Mode = r.GetString(6);

                if (!r.IsDBNull(7)) entry.GradeValue = r.GetDecimal(7);
                if (!r.IsDBNull(8)) entry.Points = r.GetDecimal(8);
                entry.IsAbsent = !r.IsDBNull(9) && r.GetBoolean(9);

                list.Add(entry);
            }

            return list;
        }

        

        public int GetEnrollmentId(int classId, int studentId)
        {
            using var conn = Db.Open();

            var sql = @"
SELECT EnrollmentID
FROM Enrollments
WHERE ClassID = @classId AND StudentID = @studentId;
";
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@classId", classId);
            cmd.Parameters.AddWithValue("@studentId", studentId);

            var result = cmd.ExecuteScalar();
            if (result == null)
                throw new Exception("Nie znaleziono zapisu studenta do tej klasy (Enrollments).");

            return Convert.ToInt32(result);
        }

        public void UpsertGrade(int enrollmentId, int assessmentId, decimal? grade, decimal? points, bool isAbsent)
        {
            using var conn = Db.Open();

            if (isAbsent)
            {
                grade = null;
                points = null;
            }

            var checkSql = @"
SELECT GradeID
FROM dbo.Grades
WHERE EnrollmentID = @enrollmentId AND AssessmentID = @assessmentId;
";
            using var checkCmd = new SqlCommand(checkSql, conn);
            checkCmd.Parameters.AddWithValue("@enrollmentId", enrollmentId);
            checkCmd.Parameters.AddWithValue("@assessmentId", assessmentId);

            var existingId = checkCmd.ExecuteScalar();

            if (existingId == null)
            {
                var insertSql = @"
INSERT INTO dbo.Grades (EnrollmentID, AssessmentID, GradeValue, Points, IsAbsent)
VALUES (@enrollmentId, @assessmentId, @grade, @points, @isAbsent);
";
                using var cmd = new SqlCommand(insertSql, conn);
                cmd.Parameters.AddWithValue("@enrollmentId", enrollmentId);
                cmd.Parameters.AddWithValue("@assessmentId", assessmentId);
                cmd.Parameters.AddWithValue("@grade", (object?)grade ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@points", (object?)points ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@isAbsent", isAbsent);
                cmd.ExecuteNonQuery();
            }
            else
            {
                var updateSql = @"
UPDATE dbo.Grades
SET GradeValue = @grade,
    Points = @points,
    IsAbsent = @isAbsent
WHERE EnrollmentID = @enrollmentId AND AssessmentID = @assessmentId;
";
                using var cmd = new SqlCommand(updateSql, conn);
                cmd.Parameters.AddWithValue("@enrollmentId", enrollmentId);
                cmd.Parameters.AddWithValue("@assessmentId", assessmentId);
                cmd.Parameters.AddWithValue("@grade", (object?)grade ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@points", (object?)points ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@isAbsent", isAbsent);
                cmd.ExecuteNonQuery();
            }
        }
    }
}
