using System.Text;
using HelperExtensions;
using SqlCommands.Attributes;
using SqlCommands.Commands;

namespace SqlCommands.Factories;

public class PostgreSqlCommandFactory : SqlCommandFactoryBase
{
    #region Protected Properties

    /// <inheritdoc />
    protected override Dictionary<Type, string> DataTypes { get; } = new()
    {
        { typeof(int), "INTEGER" },
        { typeof(int?), "INTEGER" },
        { typeof(long), "BIGINT" },
        { typeof(long?), "BIGINT" },
        { typeof(short), "SMALLINT" },
        { typeof(short?), "SMALLINT" },
        { typeof(byte), "SMALLINT" },
        { typeof(bool), "BOOLEAN" },
        { typeof(bool?), "BOOLEAN" },
        { typeof(decimal), "NUMERIC(18, 2)" },
        { typeof(decimal?), "NUMERIC(18, 2)" },
        { typeof(double), "DOUBLE PRECISION" },
        { typeof(double?), "DOUBLE PRECISION" },
        { typeof(float), "REAL" },
        { typeof(float?), "REAL" },
        { typeof(DateTime), "TIMESTAMP" },
        { typeof(DateTime?), "TIMESTAMP" },
        { typeof(DateTimeOffset), "TIMESTAMPTZ" },
        { typeof(DateTimeOffset?), "TIMESTAMPTZ" },
        { typeof(TimeSpan), "INTERVAL" },
        { typeof(TimeSpan?), "INTERVAL" },
        { typeof(Guid), "UUID" },
        { typeof(Guid?), "UUID" },
        { typeof(string), "TEXT" },
        { typeof(char), "CHAR(1)" },
        { typeof(char?), "CHAR(1)" },
        { typeof(Enum), "INTEGER" }
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

    #endregion

    #region Protected Methods

    #region AppendSelectPaginationClause
    /// <inheritdoc />
    protected override void AppendSelectPaginationClause(StringBuilder commandText, SqlTableAttribute tableAttribute, int offset, int maxResults)
    {
        if (maxResults >= 0)
            commandText.Append(" LIMIT ", maxResults);

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