using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using SystemOcenianiaSimple.Data;
using SystemOcenianiaSimple.Models;

namespace SystemOcenianiaSimple.Repositories
{
    public class ClassRepository : IRepository<ClassGroup>
    {
        public List<ClassGroup> GetAll()
        {
            var list = new List<ClassGroup>();
            using var conn = Db.Open();

            var sql = @"SELECT ClassId, GroupName, TermName FROM dbo.Classes ORDER BY ClassId";
            using var cmd = new SqlCommand(sql, conn);
            using var r = cmd.ExecuteReader();

            while (r.Read())
            {
                list.Add(new ClassGroup
                {
                    Id = r.GetInt32(0),
                    GroupName = r.GetString(1),
                    TermName = r.GetString(2)
                });
            }

            return list;
        }

        public ClassGroup? GetById(int id)
        {
            using var conn = Db.Open();
            var sql = @"SELECT ClassId, GroupName, TermName FROM dbo.Classes WHERE ClassId=@id";
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", id);

            using var r = cmd.ExecuteReader();
            if (!r.Read()) return null;

            return new ClassGroup
            {
                Id = r.GetInt32(0),
                GroupName = r.GetString(1),
                TermName = r.GetString(2)
            };
        }

        public int Add(ClassGroup item)
        {
            using var conn = Db.Open();
            var sql = @"
                INSERT INTO dbo.Classes(GroupName, TermName)
                VALUES(@g, @t);
                SELECT SCOPE_IDENTITY();";
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@g", item.GroupName);
            cmd.Parameters.AddWithValue("@t", item.TermName);
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public void Update(ClassGroup item)
        {
            using var conn = Db.Open();
            var sql = @"UPDATE dbo.Classes SET GroupName=@g, TermName=@t WHERE ClassId=@id";
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", item.Id);
            cmd.Parameters.AddWithValue("@g", item.GroupName);
            cmd.Parameters.AddWithValue("@t", item.TermName);
            cmd.ExecuteNonQuery();
        }

        public void Delete(int id)
        {
            using var conn = Db.Open();
            var sql = @"DELETE FROM dbo.Classes WHERE ClassId=@id";
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
        }
    }
}
