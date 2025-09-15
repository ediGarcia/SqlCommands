using Microsoft.Data.Sqlite;
using SqlCommands.Core;

namespace SqlCommands.Factories;

public static class SqlClientFactory
{
    #region Public Methods

    #region CreateMySqlClient*

    #region CreateMySqlClient(string, string, string, string)
    /// <summary>
    /// Creates a <see cref="SqlClient"/> instance targeting a MySQL database.
    /// </summary>
    /// <param name="serverAddress"></param>
    /// <param name="database"></param>
    /// <param name="user"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    public static SqlClient CreateMySqlClient(string serverAddress, string database, string user, string password) =>
        new(DbConnectionFactory.CreateMySqlConnection(serverAddress, database, user, password), new MySqlCommandFactory());
    #endregion

    #region CreateMySqlClient(string)
    /// <summary>
    /// Creates a <see cref="SqlClient"/> instance targeting a MySQL database.
    /// </summary>
    /// <param name="connectionString"></param>
    /// <returns></returns>
    public static SqlClient CreateMySqlClient(string connectionString) =>
        new(DbConnectionFactory.CreateMySqlConnection(connectionString), new MySqlCommandFactory());
    #endregion

    #endregion

    #region CreateOracleClient*

    #region CreateOracleClient(string, string, string)
    /// <summary>
    /// Creates a <see cref="SqlClient"/> instance targeting an Oracle database.
    /// </summary>
    /// <param name="serverAddress"></param>
    /// <param name="userId"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    public static SqlClient CreateOracleClient(string serverAddress, string userId, string password) =>
        new(DbConnectionFactory.CreateOracleConnection(serverAddress, userId, password), new OracleCommandFactory());
    #endregion

    #region CreateOracleClient(string)
    /// <summary>
    /// Creates a <see cref="SqlClient"/> instance targeting an Oracle database.
    /// </summary>
    /// <param name="connectionString"></param>
    /// <returns></returns>
    public static SqlClient CreateOracleClient(string connectionString) =>
        new(DbConnectionFactory.CreateOracleConnection(connectionString), new OracleCommandFactory());
    #endregion

    #endregion

    #region CreatePostgreSqlClient*

    #region CreatePostgreSqlClient(string, string, string, string)
    /// <summary>
    /// Creates a <see cref="SqlClient"/> instance targeting a PostgreSQL database.
    /// </summary>
    /// <param name="serverAddress"></param>
    /// <param name="database"></param>
    /// <param name="userName"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    public static SqlClient CreatePostgreSqlClient(string serverAddress, string database, string userName, string password) =>
        new(DbConnectionFactory.CreatePostgreSqlConnection(serverAddress, database, userName, password), new PostgreSqlCommandFactory());
    #endregion

    #region CreatePostgreSqlClient(string)
    /// <summary>
    /// Creates a <see cref="SqlClient"/> instance targeting a PostgreSQL database.
    /// </summary>
    /// <param name="connectionString"></param>
    /// <returns></returns>
    public static SqlClient CreatePostgreSqlClient(string connectionString) =>
        new(DbConnectionFactory.CreatePostgreSqlConnection(connectionString), new PostgreSqlCommandFactory());
    #endregion

    #endregion

    #region CreateSqliteClient*

    #region CreateSqliteClient(SqliteConnectionStringBuilder)
    /// <summary>
    /// Creates a <see cref="SqlClient"/> instance targeting a SQLite database.
    /// </summary>
    /// <param name="connectionStringBuilder"></param>
    /// <returns></returns>
    public static SqlClient CreateSqliteClient(SqliteConnectionStringBuilder connectionStringBuilder) =>
        new(DbConnectionFactory.CreateSqliteConnection(connectionStringBuilder), new SqliteCommandFactory());
    #endregion

    #region CreateSqliteClient(string)
    /// <summary>
    /// Creates a <see cref="SqlClient"/> instance targeting a SQLite database.
    /// </summary>
    /// <param name="connectionString"></param>
    /// <returns></returns>
    public static SqlClient CreateSqliteClient(string connectionString) =>
        new(DbConnectionFactory.CreateSqliteConnection(connectionString), new SqliteCommandFactory());
    #endregion

    #endregion

    #region CreateSqlServerClient*

    #region CreateSqlServerClient(string, string, string, string)
    /// <summary>
    /// Creates a <see cref="SqlClient"/> instance targeting a SQL Server database.
    /// </summary>
    /// <param name="serverAddress"></param>
    /// <param name="database"></param>
    /// <param name="userId"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    public static SqlClient CreateSqlServerClient(string serverAddress, string database, string userId, string password) =>
        new(DbConnectionFactory.CreateSqlServerConnection(serverAddress, database, userId, password), new SqlServerCommandFactory());
    #endregion

    #region CreateSqlServerClient(string)
    /// <summary>
    /// Creates a <see cref="SqlClient"/> instance targeting a SQL Server database.
    /// </summary>
    /// <param name="connectionString"></param>
    /// <returns></returns>
    public static SqlClient CreateSqlServerClient(string connectionString) =>
        new(DbConnectionFactory.CreateSqlServerConnection(connectionString), new SqlServerCommandFactory());
    #endregion

    #endregion

    #endregion
}