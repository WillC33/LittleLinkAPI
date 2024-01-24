namespace LittleLink;

using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.Sqlite;

/// <summary>
/// DbContext for SQLite implementation
/// </summary>
public class DbContext
{
    private readonly string _connectionString = "Data Source=Links.sqlite;";
    private readonly SqliteConnection _connection;

    //ctor
    public DbContext()
    {
        _connection = OpenConnection();
        InitialiseDatabase();
    }
    
    /// <summary>
    /// Initialises the Db on load if absent
    /// </summary>
    private void InitialiseDatabase()
    {
        using var connection = OpenConnection();
        var command = connection.CreateCommand();
        command.CommandText = "CREATE TABLE IF NOT EXISTS Links (Id TEXT PRIMARY KEY, Url TEXT);";
        command.ExecuteNonQuery();
    }


    /// <summary>
    /// Opens the connexion
    /// </summary>
    /// <returns></returns>
    private SqliteConnection OpenConnection()
    {
        var connection = new SqliteConnection(_connectionString);
        connection.Open();
        return connection;
    }

    /// <summary>
    /// Creates a SQL command against the _connection
    /// </summary>
    /// <param name="sql">the SQL command</param>
    /// <param name="parameters">the params</param>
    /// <returns></returns>
    private SqliteCommand CreateCommand(string sql, params SqliteParameter[] parameters)
    {
        var command = _connection.CreateCommand();
        command.CommandText = sql;
        command.Parameters.AddRange(parameters);
        return command;
    }

    /// <summary>
    /// Fetches all entities in the table
    /// </summary>
    /// <returns></returns>
    public IEnumerable<Link> GetAllEntities()
    {
        try
        {
            using var command = CreateCommand("SELECT * FROM Links", null);
            using var reader = command.ExecuteReader(CommandBehavior.CloseConnection);
            
            return ParseEntities(reader);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetAllEntities: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Fetches an entity based on its Id
    /// </summary>
    /// <param name="id">the hashed url Id</param>
    /// <returns></returns>
    public Link? GetEntityById(string id)
    {
        try
        {
            using var command = CreateCommand("SELECT * FROM Links WHERE Id = @Id", new SqliteParameter("@Id", id));
            using var reader = command.ExecuteReader(CommandBehavior.CloseConnection);
            
            return ParseEntities(reader).FirstOrDefault();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetEntityById: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Posts an entry to the Db
    /// </summary>
    /// <param name="entity">the hash/url pair to post</param>
    /// <returns></returns>
    public bool InsertEntity(Link entity)
    {
        try
        {
            using var command = CreateCommand("INSERT INTO Links (Id, Url) VALUES (@Id, @Url)",
                MapEntityToParameters(entity));
            command.ExecuteNonQuery();
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in InsertEntity: {ex.Message}");
            return false;
        }
    }

    // TODO: other CRUD operations

    /// <summary>
    /// Parses SQLite response to Link objects
    /// </summary>
    /// <param name="reader">the reader</param>
    /// <returns></returns>
    private IEnumerable<Link> ParseEntities(SqliteDataReader reader)
    {
        var entities = new List<Link>();
        while (reader.Read())
        {
            Link link = new()
            {
                Id = Convert.ToString(reader["Id"]),
                Url = Convert.ToString(reader["Url"]),
            };
            entities.Add(link);
        }
        return entities;
    }

    /// <summary>
    /// Maps an entity to the relevant params
    /// </summary>
    /// <param name="entity">the entity</param>
    /// <returns></returns>
    private SqliteParameter[] MapEntityToParameters(Link entity)
    {
        var parameters = new List<SqliteParameter>
        {
            new SqliteParameter("@Id", entity.Id),
            new SqliteParameter("@Url", entity.Url),
        };
        return parameters.ToArray();
    }
}
