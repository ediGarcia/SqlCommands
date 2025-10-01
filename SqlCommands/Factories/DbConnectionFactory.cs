using System.Data.Common;
using HelperExtensions;
using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;
using MySql.Data.MySqlClient;
using Npgsql;
using Oracle.ManagedDataAccess.Client;

namespace SqlCommands.Factories;

public static class DbConnectionFactory
{
    #region Public Methods

    #region CreateConnection
    /// <summary>
    /// Creates a <see cref="DbConnection"/> instance with the data of the connection string.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="connectionString"></param>
    /// <returns></returns>
    public static T CreateConnection<T>(string connectionString) where T : DbConnection, new()
    {
        T conn = new() { ConnectionString = connectionString };
        return conn;
    }
    #endregion

    #region CreateMySqlConnection*

    #region CreateMySqlConnection(string, string, string, string)
    /// <summary>
    /// Creates a <see cref="MySqlConnection"/> instance with the server and user data.
    /// </summary>
    /// <param name="serverAddress"></param>
    /// <param name="database"></param>
    /// <param name="user"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    public static MySqlConnection CreateMySqlConnection(string serverAddress, string database, string user, string password) =>
        CreateConnection<MySqlConnection>($"Server={serverAddress};Database={database};User={user};Password={password};");
    #endregion

    #region CreateMySqlConnection(string)
    /// <summary>
    /// Creates a <see cref="MySqlConnection"/> instance with the data of the connection string.
    /// </summary>
    /// <param name="connectionString"></param>
    /// <returns></returns>
    public static MySqlConnection CreateMySqlConnection(string connectionString) =>
        CreateConnection<MySqlConnection>(connectionString);
    #endregion

    #endregion

    #region CreateOracleConnection*

    #region CreateOracleConnection(string, string, string)
    /// <summary>
    /// Creates a <see cref="MySqlConnection"/> instance with the server and user data.
    /// </summary>
    /// <param name="serverAddress"></param>
    /// <param name="userId"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    public static OracleConnection CreateOracleConnection(string serverAddress, string userId, string password) =>
        CreateConnection<OracleConnection>($"User Id={userId};Password={password};Data Source={serverAddress};");
    #endregion

    #region CreateOracleConnection(string)
    /// <summary>
    /// Creates a <see cref="OracleConnection"/> instance with the data of the connection string.
    /// </summary>
    /// <param name="connectionString"></param>
    /// <returns></returns>
    public static OracleConnection CreateOracleConnection(string connectionString) =>
        CreateConnection<OracleConnection>(connectionString);
    #endregion

    #endregion

    #region CreatePostgreSqlConnection*

    #region CreatePostgreSqlConnection(string, string, string, string)
    /// <summary>
    /// Creates a <see cref="MySqlConnection"/> instance with the server and user data.
    /// </summary>
    /// <param name="serverAddress"></param>
    /// <param name="database"></param>
    /// <param name="userName"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    public static NpgsqlConnection CreatePostgreSqlConnection(string serverAddress, string database, string userName, string password) =>
        CreateConnection<NpgsqlConnection>($"Host={serverAddress};Database={database};Username={userName};Password={password};");
    #endregion

    #region CreatePostgreSqlConnection(string)
    /// <summary>
    /// Creates a <see cref="NpgsqlConnection"/> instance with the data of the connection string.
    /// </summary>
    /// <param name="connectionString"></param>
    /// <returns></returns>
    public static NpgsqlConnection CreatePostgreSqlConnection(string connectionString) =>
        CreateConnection<NpgsqlConnection>(connectionString);
    #endregion

    #endregion

    #region CreateSqliteConnection*

    #region CreateSqliteConnection(SqliteConnectionStringBuilder)
    /// <summary>
    /// Creates a <see cref="SqliteConnection"/> instance with the data of the <see cref="SqliteConnectionStringBuilder"/>.
    /// </summary>
    /// <param name="connectionStringBuilder"></param>
    /// <returns></returns>
    public static SqliteConnection CreateSqliteConnection(SqliteConnectionStringBuilder connectionStringBuilder) =>
        CreateConnection<SqliteConnection>(connectionStringBuilder.ConnectionString);
    #endregion

    #region CreateSqliteConnection(string, bool, [string], [string])
    /// <summary>
    /// Creates a <see cref="SqliteConnection"/> instance with the data of the connection string.
    /// </summary>
    /// <param name="path"></param>
    /// <param name="enforceExisting">Indicates whether to enforce the existence of the database file.</param>
    /// <param name="password"></param>
    /// <returns></returns>
    public static SqliteConnection CreateSqliteConnection(string path, bool enforceExisting, string password = null)
    {
        string passwordPart = password.IsNullOrWhiteSpace() ? String.Empty : $"Password={password};";
        return CreateConnection<SqliteConnection>($"Data Source={path};{passwordPart}Mode={(enforceExisting ? "ReadWrite" : "ReadWriteCreate")}");
    }
    #endregion

    #region CreateSqliteConnection(string)
    /// <summary>
    /// Creates a <see cref="SqliteConnection"/> instance with the data of the connection string.
    /// </summary>
    /// <param name="connectionString"></param>
    /// <returns></returns>
    public static SqliteConnection CreateSqliteConnection(string connectionString) =>
        CreateConnection<SqliteConnection>(connectionString);
    #endregion

    #endregion

    #region CreateSqlServerConnection*

    #region CreateSqlServerConnection(string, string, string, string)
    /// <summary>
    /// Creates a <see cref="MySqlConnection"/> instance with the server and user data.
    /// </summary>
    /// <param name="serverAddress"></param>
    /// <param name="database"></param>
    /// <param name="userId"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    public static SqlConnection CreateSqlServerConnection(string serverAddress, string database, string userId, string password) =>
        CreateConnection<SqlConnection>($"Server={serverAddress};Database={database};User Id={userId};Password={password};");
    #endregion

    #region CreateSqlServerConnection(string)
    /// <summary>
    /// Creates a <see cref="SqlConnection"/> instance with the data of the connection string.
    /// </summary>
    /// <param name="connectionString"></param>
    /// <returns></returns>
    public static SqlConnection CreateSqlServerConnection(string connectionString) =>
        CreateConnection<SqlConnection>(connectionString);
    #endregion

    #endregion

    #endregion
}