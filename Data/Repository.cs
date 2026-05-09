using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace FinancialAnalyzer.Data
{
    public static class Repository
    {
        /// <summary>
        /// Выполняет SQL-запрос и возвращает список словарей
        /// </summary>
        public static List<Dictionary<string, object>> ExecuteQuery(string sql, params (string, object)[] parameters)
        {
            var results = new List<Dictionary<string, object>>();

            using (var connection = DatabaseHelper.GetConnection())
            using (var cmd = new SQLiteCommand(sql, connection))
            {
                foreach (var (name, value) in parameters)
                {
                    cmd.Parameters.AddWithValue(name, value ?? DBNull.Value);
                }

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var row = new Dictionary<string, object>();
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            row[reader.GetName(i)] = reader.GetValue(i);
                        }
                        results.Add(row);
                    }
                }
            }

            return results;
        }

        /// <summary>
        /// Выполняет SQL-команду (INSERT, UPDATE, DELETE)
        /// </summary>
        public static int ExecuteNonQuery(string sql, params (string, object)[] parameters)
        {
            using (var connection = DatabaseHelper.GetConnection())
            using (var cmd = new SQLiteCommand(sql, connection))
            {
                foreach (var (name, value) in parameters)
                {
                    cmd.Parameters.AddWithValue(name, value ?? DBNull.Value);
                }
                return cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Вставляет запись и возвращает её Id
        /// </summary>
        public static int InsertAndGetId(string sql, params (string, object)[] parameters)
        {
            sql += "; SELECT last_insert_rowid();";
            using (var connection = DatabaseHelper.GetConnection())
            using (var cmd = new SQLiteCommand(sql, connection))
            {
                foreach (var (name, value) in parameters)
                {
                    cmd.Parameters.AddWithValue(name, value ?? DBNull.Value);
                }
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }
    }
}