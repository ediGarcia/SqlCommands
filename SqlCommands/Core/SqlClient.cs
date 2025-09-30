using System.Data;
using SqlCommands.Commands;
using SqlCommands.Factories;
using SqlCommands.Metadata;
using System.Data.Common;

#pragma warning disable CS8604

namespace SqlCommands.Core;

public class SqlClient(DbConnection connection, SqlCommandFactoryBase commandFactory)
{
    private readonly SqlCommandFactoryBase _commandFactory = commandFactory;
    private readonly DbConnection _connection = connection;

    #region Public Methods

    #region CreateTable
    /// <summary>
    /// Creates a new database table for the specified type.
    /// </summary>
    /// <remarks>This method generates a SQL command to create a table based on the properties of the
    /// specified type <typeparamref name="T"/>. The table schema is derived from the type's structure, including
    /// property names and data types. Ensure that the type <typeparamref name="T"/> is properly annotated or structured
    /// to represent a valid table schema.</remarks>
    /// <typeparam name="T">The type representing the schema of the table to be created. Each property of the type corresponds to a column
    /// in the table.</typeparam>
    public void CreateTable<T>() =>
        ExecuteNonQueryCommand(_commandFactory.CreateNewTableCommand(typeof(T)));
    #endregion

    #region DropTable
    /// <summary>
    /// Drops (deletes) the specified table from the database.
    /// </summary>
    /// <remarks>This method executes a command to remove the specified table from the database.  Use caution
    /// when dropping tables, as this operation is irreversible and may result in data loss.</remarks>
    /// <param name="tableName">The name of the table to drop. Cannot be null or empty.</param>
    /// <param name="ignoreIfNotExists">A value indicating whether the operation should ignore the absence of the table.  If <see langword="true"/>, no
    /// exception is thrown if the table does not exist; otherwise, an exception is thrown.</param>
    /// <param name="mode">Specifies the mode in which the table is dropped. Use <see cref="DropTableMode.Unsafe"/> for a standard drop
    /// operation, or other modes as defined in <see cref="DropTableMode"/> for specific behaviors.</param>
    public void DropTable(string tableName, bool ignoreIfNotExists = true, DropTableMode mode = DropTableMode.Unsafe) =>
        ExecuteNonQueryCommand(_commandFactory.CreateDropTableCommand(tableName, ignoreIfNotExists, mode));
    #endregion

    #region Insert
    /// <summary>
    /// Inserts the specified data into the database.
    /// </summary>
    /// <remarks>This method uses the provided data to generate an insert command and executes it against the
    /// database. </remarks>
    /// <typeparam name="T">The type of the data to insert. The type must match the structure of the target database table.</typeparam>
    /// <param name="data">The data object to insert. Cannot be <see langword="null"/>.</param>
    /// <returns><see langword="true"/> if the data was successfully inserted; otherwise, <see langword="false"/>.</returns>
    public bool Insert<T>(T data) =>
        ExecuteNonQueryCommand(_commandFactory.CreateInsertCommand(data)) == 1;
    #endregion

    #region RunCommands*

    #region RunCommands(IEnumerable<SqlCommand>)
    /// <summary>
    /// Executes a series of SQL commands and returns the total number of rows affected.
    /// </summary>
    /// <param name="commands"></param>
    /// <returns>The number of rows affected by each command.</returns>
    public int RunCommands(IEnumerable<SqlCommand> commands)
    {
        int affectedRows = 0;

        using (_connection)
        {
            _connection.Open();

            foreach (SqlCommand commandData in commands)
            {
                using DbCommand command = SetupCommand(commandData);
                affectedRows += Math.Max(0, command.ExecuteNonQuery());
            }
        }

        return affectedRows;
    }
    #endregion

    #region RunCommands(params SqlCommand[])
    /// <summary>
    /// Executes a series of SQL commands and returns the total number of rows affected.
    /// </summary>
    /// <param name="commands"></param>
    /// <returns>The number of rows affected by each command.</returns>
    public int RunCommands(params SqlCommand[] commands) =>
        RunCommands((IEnumerable<SqlCommand>)commands);
    #endregion

    #endregion

    #region RunFetchCommand
    /// <summary>
    /// Executes a fetch user-defined command and returns the results as a <see cref="DataTable"/>.
    /// </summary>
    /// <remarks>If the command returns no results, this method will return an empty <see cref="DataTable"/>.</remarks>
    /// <param name="commandData"></param>
    /// <returns>The results fetched by the command.</returns>
    public DataTable RunFetchCommand(SqlCommand commandData)
    {
        using DbCommand command = SetupCommand(commandData);
        using DbDataReader reader = command.ExecuteReader();
        DataTable dataTable = new();

        while (reader.Read())
        {
            DataRow row = dataTable.NewRow();

            for (int i = 0; i < reader.FieldCount; i++)
                row[reader.GetName(i)] = reader.GetValue(i);

            dataTable.Rows.Add(row);
        }

        return dataTable;
    }
    #endregion

    #region RunTransaction*

    #region RunTransaction(IEnumerable<SqlCommand>)
    /// <summary>
    /// Executes a series of SQL commands within a single transaction and returns the total number of rows affected.
    /// </summary>
    /// <param name="commands"></param>
    /// <returns>The number of rows affected by each command.</returns>
    public int RunTransaction(IEnumerable<SqlCommand> commands)
    {
        using (_connection)
        {
            _connection.Open();

            using DbTransaction transaction = _connection.BeginTransaction();
            int totalAffectedRows = 0;

            try
            {
                foreach (SqlCommand commandData in commands)
                {
                    using DbCommand command = SetupCommand(commandData);
                    command.Transaction = transaction;
                    totalAffectedRows += Math.Max(0, command.ExecuteNonQuery());
                }

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }

            return totalAffectedRows;
        }
    }
    #endregion

    #region RunTransaction(params SqlCommand[])
    /// <summary>
    /// Executes one or more of SQL commands within a single transaction and returns the total number of rows affected.
    /// </summary>
    /// <param name="commands"></param>
    /// <returns>The number of rows affected by each command.</returns>
    public int RunTransaction(params SqlCommand[] commands) =>
        RunTransaction((IEnumerable<SqlCommand>)commands);
    #endregion

    #endregion

    #region Select
    /// <summary>
    /// Retrieves a collection of entities of the specified type from the database, with optional filtering, distinct
    /// selection, and result limits.
    /// </summary>
    /// <remarks>The method uses the provided <paramref name="data"/> instance, if specified, to infer query
    /// parameters based on its properties. If <paramref name="distinct"/> is set to <see langword="true"/>, duplicate
    /// rows will be excluded from the results. The <paramref name="filter"/> parameter allows for additional filtering
    /// criteria to be applied to the query.</remarks>
    /// <typeparam name="T">The type of the entities to retrieve. The type must have a corresponding database mapping.</typeparam>
    /// <param name="data">An optional instance of the entity type <typeparamref name="T"/> to use as a template for the query. Default is
    /// <see langword="default"/>.</param>
    /// <param name="distinct">A value indicating whether to return only distinct results. Default is <see langword="false"/>.</param>
    /// <param name="filter">An optional <see cref="SqlFilter"/> object to apply filtering criteria to the query. Default is <see
    /// langword="null"/>.</param>
    /// <param name="offset">The number of rows to skip before starting to return results. Default is <c>0</c>.</param>
    /// <param name="maxResults">The maximum number of rows to return. A value of <c>-1</c> indicates no limit. Default is <c>-1</c>.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> containing the entities retrieved from the database. The collection will be
    /// empty if no matching entities are found.</returns>
    public IEnumerable<T> Select<T>(T data = default, bool distinct = false, SqlFilter filter = null, int offset = 0, int maxResults = -1) where T : new()
    {
        List<T> results = [];
        ClassMetadata classMetadata = ClassMetadataCache.GetClassMetadata(typeof(T));

        using (_connection)
        {
            _connection.Open();

            using DbCommand command = SetupCommand(_commandFactory.CreateSelectCommand(data, distinct, filter, offset, maxResults));
            using DbDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                T result = new();

                foreach (PropertyMetadata propertyMetadata in classMetadata.PropertiesMetadata)
                {
                    object value = reader[propertyMetadata.PropertyInfo.Name];
                    Type propertyType = propertyMetadata.PropertyInfo.PropertyType;

                    if (value == DBNull.Value)
                        value = null;
                    else if (propertyType == typeof(DateTime) || propertyType == typeof(DateTime?))
                        value = Convert.ToDateTime(value);
                    else if (propertyType == typeof(TimeSpan) || propertyType == typeof(TimeSpan?))
                        value = TimeSpan.Parse(value.ToString());
                    else if ((propertyType == typeof(bool) || propertyType == typeof(bool?)) && value is long longValue)
                        value = longValue switch
                        {
                            0 => false,
                            1 => true,
                            _ => throw new InvalidCastException($"Cannot convert value '{longValue}' to System.Boolean.")
                        };
                    else if (propertyType.IsEnum)
                    {
                        if (value is string stringValue)
                            value = Enum.Parse(propertyType, stringValue);
                        else
                            value = Enum.ToObject(propertyType, value);
                    }

                    propertyMetadata.PropertyInfo.SetValue(result, value);
                }

                results.Add(result);
            }

            return results;
        }
    }
    #endregion

    #region SelectFirst
    /// <summary>
    /// Retrieves the first record from the result set based on the specified criteria.
    /// </summary>
    /// <remarks>This method is a convenience wrapper for retrieving the first record from a query result. If
    /// no records match the criteria, the method returns <see langword="null"/>.</remarks>
    /// <typeparam name="T">The type of the data to be retrieved.</typeparam>
    /// <param name="data">An optional instance of <typeparamref name="T"/> used to specify the data structure or provide default values.
    /// Defaults to <see langword="default"/>.</param>
    /// <param name="filter">An optional <see cref="SqlFilter"/> object used to define filtering criteria for the query. Can be <see
    /// langword="null"/>.</param>
    /// <returns>The first record of type <typeparamref name="T"/> that matches the specified criteria, or <see langword="null"/>
    /// if no records are found.</returns>
    public T SelectFirst<T>(T data = default, SqlFilter filter = null) where T : new() =>
        Select(data, false, filter, 0, 1).FirstOrDefault();
    #endregion

    #region Update
    /// <summary>
    /// Updates records in the database that match the provided data's primary keys and an optional additional filter.
    /// </summary>
    /// <remarks>This method constructs and executes an SQL UPDATE command based on the provided data and filter.</remarks>
    /// <typeparam name="T">The type of the data object representing the table schema to update.</typeparam>
    /// <param name="data">The object containing the updated values to apply to the matching records. Cannot be null.</param>
    /// <param name="filter">An additional <see cref="SqlFilter"/> that specifies the conditions for selecting the records to update.</param>
    /// <returns>The number of rows affected by the update operation.</returns>
    public int Update<T>(T data, SqlFilter filter = null) =>
        ExecuteNonQueryCommand(_commandFactory.CreateUpdateCommand(data, filter));
    #endregion

    #region Upsert
    /// <summary>
    /// Inserts a new record or updates an existing record in the database based on the provided data.
    /// </summary>
    /// <remarks>The operation performed (insert or update) is determined by the state of the data object and
    /// the database schema.</remarks>
    /// <typeparam name="T">The type of the data object to be inserted or updated.</typeparam>
    /// <param name="data">The data object to insert or update. Cannot be <see langword="null"/>.</param>
    /// <returns>The number of rows affected by the operation.</returns>
    public int Upsert<T>(T data) =>
        ExecuteNonQueryCommand(_commandFactory.CreateUpsertCommand(data));
    #endregion

    #region Delete
    /// <summary>
    /// Deletes records from the database based on the specified data and filter criteria.
    /// </summary>
    /// <remarks>This method constructs and executes a SQL DELETE command based on the provided parameters. 
    /// Ensure that <paramref name="data"/> and/or <paramref name="filter"/> are specified appropriately to avoid
    /// unintended deletions.</remarks>
    /// <typeparam name="T">The type of the entity to delete. Typically, this represents a database table or model.</typeparam>
    /// <param name="data">An instance of the entity type <typeparamref name="T"/> containing values used to identify records to delete. 
    /// If <paramref name="checkPrimaryKeyOnly"/> is <see langword="true"/>, only the primary key values are considered.</param>
    /// <param name="filter">An optional <see cref="SqlFilter"/> object specifying additional conditions for the delete operation.  If <see
    /// langword="null"/>, the operation uses only the data provided in <paramref name="data"/>.</param>
    /// <param name="checkPrimaryKeyOnly">A value indicating whether to restrict the delete operation to records matching the primary key values in
    /// <paramref name="data"/>.  If <see langword="true"/>, only primary key fields are used; otherwise, all fields in
    /// <paramref name="data"/> are considered.</param>
    /// <returns>The number of rows affected by the delete operation.</returns>
    public int Delete<T>(T data = default, SqlFilter filter = null, bool checkPrimaryKeyOnly = true) =>
        ExecuteNonQueryCommand(_commandFactory.CreateDeleteCommand(data, filter, checkPrimaryKeyOnly));
    #endregion

    #endregion

    #region Private Methods

    #region ExecuteNonQueryCommand
    /// <summary>
    /// Executes a non-query SQL command and returns the number of rows affected.
    /// </summary>
    /// <param name="commandData">The <see cref="SqlCommand"/> containing the SQL statement to execute.</param>
    /// <returns>The number of rows affected by the command.</returns>
    private int ExecuteNonQueryCommand(SqlCommand commandData)
    {
        using (_connection)
        {
            _connection.Open();

            using DbCommand command = SetupCommand(commandData);
            return command.ExecuteNonQuery();
        }
    }
    #endregion

    #region SetupCommand
    /// <summary>
    /// Configures and returns a <see cref="DbCommand"/> instance based on the provided <see cref="SqlCommand"/> data.
    /// </summary>
    /// <remarks>This method creates a new <see cref="DbCommand"/> using the current database connection and
    /// populates it with the command text and parameters specified in the <paramref name="commandData"/>. Parameters
    /// with a null value are converted to <see cref="DBNull.Value"/>.</remarks>
    /// <param name="commandData">The <see cref="SqlCommand"/> containing the command text and parameters to configure the <see
    /// cref="DbCommand"/>.</param>
    /// <returns>A <see cref="DbCommand"/> instance configured with the command text and parameters from <paramref
    /// name="commandData"/>.</returns>
    private DbCommand SetupCommand(SqlCommand commandData)
    {
        DbCommand command = _connection.CreateCommand();
        command.CommandText = commandData.Text;

        foreach (SqlParameter sqlParameter in commandData.Parameters)
        {
            DbParameter parameter = command.CreateParameter();
            parameter.ParameterName = sqlParameter.Name;
            parameter.Value = sqlParameter.Value ?? DBNull.Value;

            command.Parameters.Add(parameter);
        }

        return command;
    }
    #endregion

    #endregion
}