using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.IO;

using Dapper;
using Mono.Data;
using Mono.Data.Sqlite;
using Android.Content.Res;

namespace ComicsViewer.Models
{
    class Database
    {
        private string source;

        public Database()
        {
            string path = Path.Combine(Path.Combine(Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.Personal)).FullName, "files"), "profile.db");
            source = new SqliteConnectionStringBuilder() { DataSource = path }.ToString();
            if (!File.Exists(path))
            {
                SqliteConnection.CreateFile(path);
                using (var conn = GetConnection())
                {
                    using (var reader = new StreamReader(Application.Context.Assets.Open("profile.sql")))
                    {
                        string query = reader.ReadToEnd();
                        Console.WriteLine(query);
                        conn.Execute(query);
                    }
                }
            }
        }

        private SqliteConnection GetConnection()
        {
            var conn = new SqliteConnection(source);
            conn.Open();
            return conn;
        }

        public ComicsSettings GetSettingsForPath(string path)
        {
            using (var conn = GetConnection())
            {
                return conn.Query<ComicsSettings>("SELECT page as Page, pos_x as TranslationX, pos_y as TranslationY, zoom as ZoomStep FROM comics WHERE path LIKE @path;", new { path = path }).FirstOrDefault();
            }
        }

        public void SaveSettingsForPath(string path, ComicsSettings sett)
        {
            using (var conn = GetConnection())
            {
                conn.Execute("INSERT OR REPLACE INTO comics (path, page, pos_x, pos_y, zoom) VALUES (@path, @page, @pos_x, @pos_y, @zoom);", new { path = path, page = sett.Page, pos_x = sett.TranslationX, pos_y = sett.TranslationY, zoom = sett.ZoomStep });
            }
        }
    }
}