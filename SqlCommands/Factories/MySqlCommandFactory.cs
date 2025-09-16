using System.Reflection;
using HelperExtensions;
using SqlCommands.Attributes;
using SqlCommands.Commands;
using SqlCommands.Metadata;
using System.Text;

namespace SqlCommands.Factories;

public class MySqlCommandFactory : SqlCommandFactoryBase
{
    #region Protected Properties

    /// <inheritdoc />
    protected override string AutoIncrementFlag { get; } = "AUTO_INCREMENT";

    /// <inheritdoc />
    protected override Dictionary<Type, string> DataTypes { get; } = new()
    {
        { typeof(int), "INT" },
        { typeof(int?), "INT" },
        { typeof(long), "BIGINT" },
        { typeof(long?), "BIGINT" },
        { typeof(short), "SMALLINT" },
        { typeof(short?), "SMALLINT" },
        { typeof(byte), "TINYINT UNSIGNED" },
        { typeof(byte?), "TINYINT UNSIGNED" },
        { typeof(bool), "TINYINT(1)" },
        { typeof(bool?), "TINYINT(1)" },
        { typeof(decimal), "DECIMAL(18,2)" },
        { typeof(decimal?), "DECIMAL(18,2)" },
        { typeof(double), "DOUBLE" },
        { typeof(double?), "DOUBLE" },
        { typeof(float), "FLOAT" },
        { typeof(float?), "FLOAT" },
        { typeof(DateTime), "DATETIME" },
        { typeof(DateTime?), "DATETIME" },
        { typeof(DateTimeOffset), "TIMESTAMP" },
        { typeof(DateTimeOffset?), "TIMESTAMP" },
        { typeof(TimeSpan), "TIME" },
        { typeof(TimeSpan?), "TIME" },
        { typeof(Guid), "CHAR(36)" },
        { typeof(Guid?), "CHAR(36)" },
        { typeof(string), "VARCHAR(65535)" },
        { typeof(char), "CHAR(1)" },
        { typeof(char?), "CHAR(1)" },
        { typeof(Enum), "INT" }
    };

    #endregion

    #region Public Methods

    #region CreateDropTableCommand
    /// <inheritdoc />
    /// <remarks>SQLite only supports unsafe table drop.</remarks>
    /// <exception cref="NotSupportedException"></exception>
    public override SqlCommand CreateDropTableCommand(string tableName, bool ignoreIfNotExists = true, DropTableMode mode = DropTableMode.Unsafe)
    {
        if (mode != DropTableMode.Unsafe)
            throw new NotSupportedException($"SQLite only supports {nameof(DropTableMode.Unsafe)} table drop.");

        return base.CreateDropTableCommand(tableName, ignoreIfNotExists, mode);
    }
    #endregion

    #region CreateUpsertCommand
    /// <inheritdoc />
    public override SqlCommand CreateUpsertCommand<T>(T data)
    {
        ClassMetadata classMetadata = ClassMetadataCache.GetClassMetadata(typeof(T));

        StringBuilder columnsText = new();
        StringBuilder valuesText = new();
        StringBuilder updateText = new();
        List<SqlParameter> parameters = new(classMetadata.PropertiesMetadata.Length);

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

            columnsText.Append(columnName, ", ");

            if (columnValue is null)
                valuesText.Append("NULL, ");
            else
            {
                valuesText.Append(ParameterPrefix, propertyInfo.Name, ", ");
                parameters.Add(new($"{ParameterPrefix}{propertyInfo.Name}", columnValue));
            }

            updateText.Append(columnName, " = VALUES(", columnName, "), ");
        }

        if (columnsText.Length == 0)
            throw new InvalidOperationException($"No eligible columns with the '{nameof(SqlColumnAttribute)}' found in '{classMetadata.ClassType.Name}'.");

        if (updateText.Length == 0)
            throw new InvalidOperationException($"No eligible update fields found in '{classMetadata.ClassType.Name}'.");

        // Removes last ", "
        columnsText.Length -= 2;
        valuesText.Length -= 2;
        updateText.Length -= 2;

        return new(
            $"INSERT INTO {QuoteIdentifier(classMetadata.TableName)} ({columnsText}) VALUES ({valuesText}) ON DUPLICATE KEY UPDATE {updateText};",
            parameters.ToArray());
    }
    #endregion

    #endregion

    #region Protected Methods

    #region AppendSelectPaginationClause
    /// <inheritdoc />
    protected override void AppendSelectPaginationClause(StringBuilder commandText, SqlTableAttribute tableAttribute, int offset, int maxResults)
    {
        commandText.Append(" LIMIT ", maxResults >= 0 ? maxResults : "18446744073709551615");

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

    #region QuoteIdentifier
    /// <inheritdoc />
    protected override string QuoteIdentifier(string identifier) =>
        $"`{identifier}`";
    #endregion

    #endregion
}