using Pluralsight.OutsideInTDD.WebAPI.Domain;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Pluralsight.OutsideInTDD.WebAPI.Data
{
    public class Repository
    {
        private static string connString = @"Data Source=.\SQLEXPRESS;Initial Catalog=RunningJournal;Integrated Security=True;Pooling=false";

        public JournalEntry[] GetForUser(string username)
        {
            var entries = new List<JournalEntry>();
            var builder = new SqlConnectionStringBuilder(connString);
            builder.InitialCatalog = "RunningJournal";
            using (var conn = new SqlConnection(builder.ConnectionString))
            {
                conn.Open();
                using (var cmd = new SqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "SELECT TOP 1 * FROM dbo.[JournalEntry]";

                    SqlDataReader reader = cmd.ExecuteReader();
                    
                    while (reader.Read())
                    {
                        entries.Add(
                            new JournalEntry()
                            {
                                UserId = int.Parse(reader["UserId"].ToString()),
                                Distance = int.Parse(reader["Distance"].ToString()),
                                Duration = TimeSpan.Parse(reader["Duration"].ToString()),
                                Time = DateTimeOffset.Parse(reader["Time"].ToString())
                            });
                    }

                    reader.Close();
                };
            }
            return entries.ToArray();
        }
    }
}
