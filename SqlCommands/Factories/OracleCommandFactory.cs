using System.Reflection;
using HelperExtensions;
using SqlCommands.Attributes;
using SqlCommands.Commands;
using SqlCommands.Metadata;
using System.Text;

namespace SqlCommands.Factories;

public class OracleCommandFactory : SqlCommandFactoryBase
{
    #region Protected Properties

    /// <inheritdoc />
    protected override Dictionary<Type, string> DataTypes { get; } = new()
    {
        { typeof(int), "NUMBER(10)" },
        { typeof(int?), "NUMBER(10)" },
        { typeof(long), "NUMBER(19)" },
        { typeof(long?), "NUMBER(19)" },
        { typeof(short), "NUMBER(5)" },
        { typeof(short?), "NUMBER(5)" },
        { typeof(byte), "NUMBER(3)" },
        { typeof(byte?), "NUMBER(3)" },
        { typeof(bool), "NUMBER(1)" },
        { typeof(bool?), "NUMBER(1)" },
        { typeof(decimal), "NUMBER(18,2)" },
        { typeof(decimal?), "NUMBER(18,2)" },
        { typeof(double), "BINARY_DOUBLE" },
        { typeof(double?), "BINARY_DOUBLE" },
        { typeof(float), "BINARY_FLOAT" },
        { typeof(float?), "BINARY_FLOAT" },
        { typeof(DateTime), "DATE" }, // Stores date + time.
        { typeof(DateTime?), "DATE" },
        { typeof(DateTimeOffset), "TIMESTAMP WITH TIME ZONE" },
        { typeof(DateTimeOffset?), "TIMESTAMP WITH TIME ZONE" },
        { typeof(TimeSpan), "INTERVAL DAY TO SECOND" },
        { typeof(TimeSpan?), "INTERVAL DAY TO SECOND" },
        { typeof(Guid), "RAW(16)" },
        { typeof(Guid?), "RAW(16)" },
        { typeof(string), "NVARCHAR2(4000)" },
        { typeof(char), "CHAR(1)" },
        { typeof(char?), "CHAR(1)" },
        { typeof(Enum), "NUMBER(10)" }
    };

    /// <inheritdoc />
    protected override string ParameterPrefix { get; } = ":";

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

    #endregion

    #region Protected Methods

    #region AppendUpsertSourceClause
    /// <inheritdoc />
    protected override void AppendUpsertSourceClause<T>(T data, StringBuilder commandText, List<SqlParameter> parameters, IReadOnlyCollection<PropertyMetadata> propertiesMetadata)
    {
        StringBuilder columnsText = new();

        foreach (PropertyMetadata propertyMetadata in propertiesMetadata)
        {
            PropertyInfo propertyInfo = propertyMetadata.PropertyInfo;
            SqlColumnAttribute columnAttribute = propertyMetadata.ColumnAttribute;

            if (columnAttribute.IgnoreRules.HasFlag(IgnoreRule.UpsertAlways)
                || !columnAttribute.Expression.IsNullOrWhiteSpace())
                continue;

            string columnName = QuoteIdentifier(propertyMetadata.ColumnName);
            string parameterName = ParameterPrefix + propertyInfo.Name;

            if (propertyInfo.GetValueOrDefault(data) is { } columnValue)
            {
                columnsText.Append(parameterName, " AS ", columnName, ", ");
                parameters.Add(new(parameterName, columnValue));
            }
            else if (!columnAttribute.IgnoreRules.HasFlag(IgnoreRule.UpsertIfNull))
                columnsText.Append("NULL AS ", columnName, ", ");
        }

        if (columnsText.Length == 0)
            throw new InvalidOperationException($"No eligible properties with '{nameof(SqlColumnAttribute)}' found in '{typeof(T).Name}'.");

        columnsText.Length -= 2; // Removes last ", "
        commandText.Append("USING (SELECT ", columnsText, " FROM dual) AS source ");
    }
    #endregion

    #endregion
}