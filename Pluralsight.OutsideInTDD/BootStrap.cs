using Pluralsight.OutsideInTDD.Properties;
using System;
using System.Data.SqlClient;

namespace Pluralsight.OutsideInTDD
{
    internal class BootStrap
    {
        public static string connString = @"Data Source=.\SQLEXPRESS;Initial Catalog=RunningJournal;Integrated Security=True;Pooling=false";

        internal void InstallDatabase()
        {
            var builder = new SqlConnectionStringBuilder(connString);
            builder.InitialCatalog = "Master";
            using (var conn = new SqlConnection(builder.ConnectionString))
            {
                conn.Open();
                using (var cmd = new SqlCommand())
                {
                    cmd.Connection = conn;

                    var schemaSql = Resources.DbSchema;
                    foreach(var sql in schemaSql.Split(new[] { "GO"}, StringSplitOptions.RemoveEmptyEntries))
                    {
                        cmd.CommandText = sql;
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        internal void UninstallDatabase()
        {
            var builder = new SqlConnectionStringBuilder(connString);
            builder.InitialCatalog = "Master";
            using (var conn = new SqlConnection(builder.ConnectionString))
            {
                conn.Open();
                using (var cmd = new SqlCommand())
                {
                    cmd.Connection = conn;

                    var schemaSql = Resources.DbSchema;
                    foreach (var sql in schemaSql.Split(new[] { "GO" }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        cmd.CommandText = @"
                                IF EXISTS (
                                    SELECT name 
                                    FROM master.dbo.sysdatabases 
                                    WHERE name = N'RunningJournal')
                                DROP DATABASE [RunningJournal];";
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }
    }
}