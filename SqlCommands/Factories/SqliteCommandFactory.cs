using System.Reflection;
using HelperExtensions;
using SqlCommands.Attributes;
using SqlCommands.Commands;
using SqlCommands.Metadata;
using System.Text;

namespace SqlCommands.Factories;

public class SqliteCommandFactory : SqlCommandFactoryBase
{
    #region Protected Properties

    /// <inheritdoc />
    protected override string AutoIncrementFlag { get; } = "AUTOINCREMENT";

    /// <inheritdoc />
    protected override Dictionary<Type, string> DataTypes { get; } = new()
    {
        { typeof(int), "INTEGER" },
        { typeof(int?), "INTEGER" },
        { typeof(long), "INTEGER" },
        { typeof(long?), "INTEGER" },
        { typeof(short), "INTEGER" },
        { typeof(short?), "INTEGER" },
        { typeof(byte), "INTEGER" },
        { typeof(byte?), "INTEGER" },
        { typeof(bool), "INTEGER" },
        { typeof(bool?), "INTEGER" },
        { typeof(decimal), "REAL" },
        { typeof(decimal?), "REAL" },
        { typeof(double), "REAL" },
        { typeof(double?), "REAL" },
        { typeof(float), "REAL" },
        { typeof(float?), "REAL" },
        { typeof(DateTime), "TEXT" },
        { typeof(DateTime?), "TEXT" },
        { typeof(DateTimeOffset), "TEXT" },
        { typeof(DateTimeOffset?), "TEXT" },
        { typeof(TimeSpan), "TEXT" },
        { typeof(TimeSpan?), "TEXT" },
        { typeof(Guid), "TEXT" },
        { typeof(Guid?), "TEXT" },
        { typeof(string), "TEXT" },
        { typeof(char), "TEXT" },
        { typeof(char?), "TEXT" },
        { typeof(Enum), "INTEGER" }
    };

    #endregion

    #region Public Methods

    #region CreateDropTableCommand
    /// <inheritdoc />
    /// <remarks>SQLite only supports unsafe table drop.</remarks>
    /// <exception cref="NotSupportedException"></exception>
    public override SqlCommand CreateDropTableCommand(string tableName, bool ignoreIfNotExists = true, DropTableMode mode = DropTableMode.Unsafe) =>
        mode != DropTableMode.Unsafe
            ? throw new NotSupportedException($"SQLite only supports {nameof(DropTableMode.Unsafe)} table drop.")
            : base.CreateDropTableCommand(tableName, ignoreIfNotExists, mode);

    #endregion

    #region CreateUpsertCommand
    /// <inheritdoc />
    public override SqlCommand CreateUpsertCommand<T>(T data)
    {
        ClassMetadata classMetadata = ClassMetadataCache.GetClassMetadata(typeof(T));

        StringBuilder columnsText = new();
        StringBuilder valuesText = new();
        StringBuilder conflictText = new();
        StringBuilder updateText = new();
        List<SqlParameter> parameters = new(classMetadata.PropertiesMetadata.Count);

        foreach (PropertyMetadata propertyMetadata in classMetadata.PropertiesMetadata)
        {
            PropertyInfo propertyInfo = propertyMetadata.PropertyInfo;
            SqlColumnAttribute columnAttribute = propertyMetadata.ColumnAttribute;

            if (columnAttribute.IgnoreRules.HasFlag(IgnoreRule.UpsertAlways)
                || !columnAttribute.Expression.IsNullOrWhiteSpace()
                || (!propertyInfo.TryGetValue(data, out object columnValue) || columnValue is null)
                && columnAttribute.IgnoreRules.HasFlag(IgnoreRule.UpsertIfNull))
                continue;

            string columnName = QuoteIdentifier(propertyMetadata.ColumnName);
            string parameterName = ParameterPrefix + propertyInfo.Name;

            columnsText.Append(columnName, ", ");

            if (columnValue is null)
                valuesText.Append("NULL, ");
            else
            {
                valuesText.Append(parameterName, ", ");
                parameters.Add(new(parameterName, columnValue));
            }

            if (columnAttribute.IsPrimaryKey)
                conflictText.Append(columnName, ", ");
            else
                updateText.Append(columnName, " = excluded.", columnName, ", ");
        }

        if (columnsText.Length == 0)
            throw new InvalidOperationException($"No eligible columns with the '{nameof(SqlColumnAttribute)}' found in '{classMetadata.ClassType.Name}'.");

        if (conflictText.Length == 0)
            throw new InvalidOperationException("At least one eligible primary key is required for the UPSERT command.");

        if (updateText.Length == 0)
            throw new InvalidOperationException($"No eligible update fields found in '{classMetadata.ClassType.Name}'.");

        // Removes last ", "
        columnsText.Length -= 2;
        valuesText.Length -= 2;
        conflictText.Length -= 2;
        updateText.Length -= 2;

        return new($"INSERT INTO {QuoteIdentifier(classMetadata.TableName)} ({columnsText}) VALUES ({valuesText}) ON CONFLICT ({conflictText}) DO UPDATE SET {updateText};", parameters);
    }
    #endregion

    #endregion

    #region Protected Methods

    #region AppendSelectPaginationClause
    /// <inheritdoc />
    protected override void AppendSelectPaginationClause(StringBuilder commandText, SqlTableAttribute tableAttribute, int offset, int maxResults)
    {
        commandText.Append(" LIMIT ", Math.Max(-1, maxResults));

        if (offset > 0)
            commandText.Append(" OFFSET ", offset);
    }
    #endregion

    #region ValidateSelectPaginationInfo
    /// <inheritdoc />
    protected override void ValidateSelectPaginationInfo(int offset, int maxResults)
    {
        if (offset < 0)
            throw new ArgumentOutOfRangeException(nameof(offset), $"'{nameof(offset)}' must be greater than or equal to zero.");
    }
    #endregion

    #endregion
}