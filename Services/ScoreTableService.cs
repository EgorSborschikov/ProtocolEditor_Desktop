using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using System.Linq;
using ProtocolEditor.Models.Sqlite;

namespace ProtocolEditor.Services;

/// <summary>
/// Сервис для работы с локальной базой данных SQLite для хранения соответствия очков
/// </summary>

public class ScoreTableService
{
    private const string DatabaseFile = "scores.db";
    private string ConnectionString => $"Data Source={DatabaseFile}";

    public ScoreTableService()
    {
        InitializeDatabase();
    }

    private void InitializeDatabase()
    {
        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();
        CreateTables(connection);
    }

    private void CreateTables(SqliteConnection connection)
    {
        var command = connection.CreateCommand();
        command.CommandText = @"
                CREATE TABLE IF NOT EXISTS ScoreEntries (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Place INTEGER NOT NULL,
                    Points INTEGER NOT NULL,
                    TableType INTEGER NOT NULL
                );
            ";
        command.ExecuteNonQuery();
    }

    public void SaveEntries(IEnumerable<ScoreEntry> entries)
    {
        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();
            
        // Удаляем старые записи для этих типов
        var types = entries.Select(e => (int)e.TableType).Distinct().ToList();
            
        if (types.Count > 0)
        {
            var deleteCommand = connection.CreateCommand();
            deleteCommand.CommandText = $"DELETE FROM ScoreEntries WHERE TableType IN ({string.Join(",", types)})";
            deleteCommand.ExecuteNonQuery();
        }

        // Вставляем новые записи
        foreach (var entry in entries)
        {
            var insertCommand = connection.CreateCommand();
            insertCommand.CommandText = @"
                    INSERT INTO ScoreEntries (Place, Points, TableType)
                    VALUES ($place, $points, $tableType)
                ";
            insertCommand.Parameters.AddWithValue("$place", entry.Place);
            insertCommand.Parameters.AddWithValue("$points", entry.Points);
            insertCommand.Parameters.AddWithValue("$tableType", (int)entry.TableType);
            insertCommand.ExecuteNonQuery();
        }
    }

    public IEnumerable<ScoreEntry> LoadEntries(ScoreTableType tableType)
    {
        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();
            
        var entries = new List<ScoreEntry>();
        var command = connection.CreateCommand();
        command.CommandText = @"
                SELECT Place, Points 
                FROM ScoreEntries 
                WHERE TableType = $tableType
                ORDER BY Place
            ";
        command.Parameters.AddWithValue("$tableType", (int)tableType);
            
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            // Используем безопасное чтение значений
            entries.Add(new ScoreEntry
            {
                Place = reader.IsDBNull(0) ? 0 : reader.GetInt32(0),
                Points = reader.IsDBNull(1) ? 0 : reader.GetInt32(1),
                TableType = tableType
            });
        }
            
        return entries;
    }
}