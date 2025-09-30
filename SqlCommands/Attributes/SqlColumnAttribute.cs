namespace SqlCommands.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class SqlColumnAttribute(string name = null, string type = null, bool isPrimaryKey = false, bool isAutoIncrement = false, string expression = null, IgnoreRule ignoreRules = IgnoreRule.AlwaysIfNull) : Attribute
{
    #region Properties

    /// <summary>
    /// Gets the SQL expression used to compute the value of the column.
    /// </summary>
    /// <remarks>Defining this property will make the current property SELECT-only.</remarks>
    public string Expression { get; } = expression;

    /// <summary>
    /// Gets the command types in which the column should be ignored if its value is <see langword="null"/>.
    /// </summary>
    public IgnoreRule IgnoreRules { get; } = ignoreRules;

    /// <summary>
    /// Gets a value indicating whether the column is auto-incremented in the database table.
    /// </summary>
    public bool IsAutoIncrement { get; } = isAutoIncrement;

    /// <summary>
    /// Gets a value indicating whether the column is the primary key in the database table.
    /// </summary>
    public bool IsPrimaryKey { get; } = isPrimaryKey;

    /// <summary>
    /// Gets the column name.
    /// </summary>
    /// <remarks>If <see langword="null"/>, the property name will be considered.</remarks>
    public string Name { get; } = name;

    /// <summary>
    /// Gets the SQL data type of the column. Only for CREATE TABLE commands.
    /// </summary>
    /// <remarks>If <see langword="null"/>, the best matching SQL data will be used according to the property type.</remarks>
    public string Type { get; } = type;

    #endregion
}