using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using SystemOcenianiaSimple.Data;
using SystemOcenianiaSimple.Models;

namespace SystemOcenianiaSimple.Repositories
{
    public class UserRepository : IRepository<User>
    {
        public List<User> GetAll()
        {
            var list = new List<User>();
            using var conn = Db.Open();

            var sql = @"SELECT UserId, Email, PasswordHash, Role FROM dbo.Users ORDER BY UserId";
            using var cmd = new SqlCommand(sql, conn);
            using var r = cmd.ExecuteReader();

            while (r.Read())
            {
                list.Add(new User
                {
                    Id = r.GetInt32(0),
                    Email = r.GetString(1),
                    PasswordHash = r.GetString(2),
                    Role = r.GetString(3)
                });
            }

            return list;
        }

        public User? GetById(int id)
        {
            using var conn = Db.Open();
            var sql = @"SELECT UserId, Email, PasswordHash, Role FROM dbo.Users WHERE UserId=@id";
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", id);

            using var r = cmd.ExecuteReader();
            if (!r.Read()) return null;

            return new User
            {
                Id = r.GetInt32(0),
                Email = r.GetString(1),
                PasswordHash = r.GetString(2),
                Role = r.GetString(3)
            };
        }

        public int Add(User item)
        {
            using var conn = Db.Open();
            var sql = @"
                INSERT INTO dbo.Users(Email, PasswordHash, Role)
                VALUES(@email, @pass, @role);
                SELECT SCOPE_IDENTITY();";
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@email", item.Email);
            cmd.Parameters.AddWithValue("@pass", item.PasswordHash);
            cmd.Parameters.AddWithValue("@role", item.Role);
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public void Update(User item)
        {
            using var conn = Db.Open();
            var sql = @"
                UPDATE dbo.Users
                SET Email=@email, PasswordHash=@pass, Role=@role
                WHERE UserId=@id";
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", item.Id);
            cmd.Parameters.AddWithValue("@email", item.Email);
            cmd.Parameters.AddWithValue("@pass", item.PasswordHash);
            cmd.Parameters.AddWithValue("@role", item.Role);
            cmd.ExecuteNonQuery();
        }

        public void Delete(int id)
        {
            using var conn = Db.Open();
            var sql = @"DELETE FROM dbo.Users WHERE UserId=@id";
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
        }

        public User? Login(string email, string password)
        {
            using var conn = Db.Open();
            var sql = @"SELECT UserId, Email, PasswordHash, Role FROM dbo.Users WHERE Email=@email";
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@email", email);

            using var r = cmd.ExecuteReader();
            if (!r.Read()) return null;

            var pass = r.GetString(2);
            if (pass != password) return null;

            return new User
            {
                Id = r.GetInt32(0),
                Email = r.GetString(1),
                PasswordHash = pass,
                Role = r.GetString(3)
            };
        }
    }
}
