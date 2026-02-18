using System;
using System.IO;
using System.Text.Json;

namespace SystemOcenianiaSimple.Data
{
    public class DbConfigModel
    {
        public string ConnectionString { get; set; } = "";
    }

    public static class DbConfig
    {
        public static string GetConnectionString()
        {
            var path = Path.Combine(AppContext.BaseDirectory, "dbconfig.json");

            if (!File.Exists(path))
                throw new Exception($"Brak pliku dbconfig.json w: {path}");

            var json = File.ReadAllText(path);
            var cfg = JsonSerializer.Deserialize<DbConfigModel>(json);

            if (cfg == null || string.IsNullOrWhiteSpace(cfg.ConnectionString))
                throw new Exception("dbconfig.json ma pusty ConnectionString");

            return cfg.ConnectionString;
        }
    }
}
