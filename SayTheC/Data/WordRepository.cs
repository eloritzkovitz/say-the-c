using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using Models;

namespace Data
{
    public class WordRepository
    {
        private readonly string _dbPath;

        // Constructor to initialize the database path
        public WordRepository(string dbPath)
        {
            _dbPath = dbPath;
        }

        // Load words from the database
        public List<WordEntry> GetAllWords()
        {
            var result = new List<WordEntry>();
            using (var connection = new SqliteConnection($"Data Source={_dbPath}"))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT Word, ImageUrl FROM Words";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(new WordEntry
                        {
                            Word = reader.GetString(0),
                            ImageUrl = reader.GetString(1)
                        });
                    }
                }
            }
            return result;
        }
    }
}