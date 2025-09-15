namespace SqlCommands.Factories;

// ReSharper disable once UnusedMember.Global
public class SqlServerCommandFactory : SqlCommandFactoryBase
{
    #region Protected Properties

    /// <inheritdoc />
    protected override string AutoIncrementFlag { get; } = "IDENTITY";

    /// <inheritdoc />
    protected override Dictionary<Type, string> DataTypes { get; } = new()
    {
        { typeof(int), "INT" },
        { typeof(int?), "INT" },
        { typeof(long), "BIGINT" },
        { typeof(long?), "BIGINT" },
        { typeof(short), "SMALLINT" },
        { typeof(short?), "SMALLINT" },
        { typeof(byte), "TINYINT" },
        { typeof(byte?), "TINYINT" },
        { typeof(bool), "BIT" },
        { typeof(bool?), "BIT" },
        { typeof(decimal), "DECIMAL(18, 2)" },
        { typeof(decimal?), "DECIMAL(18, 2)" },
        { typeof(double), "FLOAT" },
        { typeof(double?), "FLOAT" },
        { typeof(float), "REAL" },
        { typeof(float?), "REAL" },
        { typeof(DateTime), "DATETIME2" },
        { typeof(DateTime?), "DATETIME2" },
        { typeof(DateTimeOffset), "DATETIMEOFFSET" },
        { typeof(DateTimeOffset?), "DATETIMEOFFSET" },
        { typeof(TimeSpan), "TIME" },
        { typeof(TimeSpan?), "TIME" },
        { typeof(Guid), "UNIQUEIDENTIFIER" },
        { typeof(Guid?), "UNIQUEIDENTIFIER" },
        { typeof(string), "NVARCHAR(MAX)" },
        { typeof(char), "NCHAR(1)" },
        { typeof(char?), "NCHAR(1)" },
        { typeof(Enum), "INT" }
    };

    #endregion
}