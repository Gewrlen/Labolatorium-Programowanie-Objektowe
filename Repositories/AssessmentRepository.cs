using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using SystemOcenianiaSimple.Data;
using SystemOcenianiaSimple.Models;

namespace SystemOcenianiaSimple.Repositories
{
    public class AssessmentRepository : IRepository<Assessment>
    {
        public List<Assessment> GetAll()
        {
            var list = new List<Assessment>();
            using var conn = Db.Open();

            var sql = @"
                SELECT AssessmentId, ClassId, CreatedByUserId, Title, Category, GradingMode, MaxPoints, Weight
                FROM dbo.Assessments
                ORDER BY AssessmentId";
            using var cmd = new SqlCommand(sql, conn);
            using var r = cmd.ExecuteReader();

            while (r.Read())
            {
                list.Add(Read(r));
            }

            return list;
        }

        public Assessment? GetById(int id)
        {
            using var conn = Db.Open();
            var sql = @"
                SELECT AssessmentId, ClassId, CreatedByUserId, Title, Category, GradingMode, MaxPoints, Weight
                FROM dbo.Assessments
                WHERE AssessmentId=@id";
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", id);

            using var r = cmd.ExecuteReader();
            if (!r.Read()) return null;

            return Read(r);
        }

        public List<Assessment> GetByClassId(int classId)
        {
            var list = new List<Assessment>();
            using var conn = Db.Open();

            var sql = @"
                SELECT AssessmentId, ClassId, CreatedByUserId, Title, Category, GradingMode, MaxPoints, Weight
                FROM dbo.Assessments
                WHERE ClassId=@cid
                ORDER BY AssessmentId";
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@cid", classId);

            using var r = cmd.ExecuteReader();
            while (r.Read())
                list.Add(Read(r));

            return list;
        }

        public int Add(Assessment item)
        {
            using var conn = Db.Open();
            var sql = @"
                INSERT INTO dbo.Assessments(ClassId, CreatedByUserId, Title, Category, GradingMode, MaxPoints, Weight)
                VALUES(@cid, @uid, @title, @cat, @mode, @max, @w);
                SELECT SCOPE_IDENTITY();";

            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@cid", item.ClassId);
            cmd.Parameters.AddWithValue("@uid", item.CreatedByUserId);
            cmd.Parameters.AddWithValue("@title", item.Title);
            cmd.Parameters.AddWithValue("@cat", item.Category);
            cmd.Parameters.AddWithValue("@mode", item.GradingMode);
            cmd.Parameters.AddWithValue("@max", (object?)item.MaxPoints ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@w", item.Weight);

            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public void Update(Assessment item)
        {
            using var conn = Db.Open();
            var sql = @"
                UPDATE dbo.Assessments
                SET ClassId=@cid, CreatedByUserId=@uid, Title=@title, Category=@cat, GradingMode=@mode, MaxPoints=@max, Weight=@w
                WHERE AssessmentId=@id";
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", item.Id);
            cmd.Parameters.AddWithValue("@cid", item.ClassId);
            cmd.Parameters.AddWithValue("@uid", item.CreatedByUserId);
            cmd.Parameters.AddWithValue("@title", item.Title);
            cmd.Parameters.AddWithValue("@cat", item.Category);
            cmd.Parameters.AddWithValue("@mode", item.GradingMode);
            cmd.Parameters.AddWithValue("@max", (object?)item.MaxPoints ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@w", item.Weight);
            cmd.ExecuteNonQuery();
        }

        public void Delete(int id)
        {
            using var conn = Db.Open();
            var sql = @"DELETE FROM dbo.Assessments WHERE AssessmentId=@id";
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
        }

        private static Assessment Read(SqlDataReader r)
        {
            return new Assessment
            {
                Id = r.GetInt32(0),
                ClassId = r.GetInt32(1),
                CreatedByUserId = r.GetInt32(2),
                Title = r.GetString(3),
                Category = r.GetString(4),
                GradingMode = r.GetString(5),
                MaxPoints = r.IsDBNull(6) ? null : r.GetDecimal(6),
                Weight = r.GetDecimal(7)
            };
        }
    }
}
