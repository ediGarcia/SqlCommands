namespace SqlCommands.Attributes;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class SqlTableAttribute(string name = null, string orderBy = null, string groupBy = null, string having = null) : Attribute
{
    #region Properties

    /// <summary>
    /// Gets the column used to group items in a collection.
    /// </summary>
    public string GroupBy { get; } = groupBy;

    /// <summary>
    /// Gets the HAVING clause used to filter the results of a query after grouping.
    /// </summary>
    /// <remarks>The HAVING clause is typically used in conjunction with a GROUP BY clause to filter
    /// aggregated results. Ensure the value is a valid SQL HAVING clause to avoid query errors.</remarks>
    public string Having { get; } = having;

    /// <summary>
    /// Gets the name of the SQL table associated with the class.
    /// </summary>
    /// <remarks>If <see langword="null"/>, the class name will be considered.</remarks>
    public string Name { get; } = name;

    /// <summary>
    /// Gets the field by which the results should be ordered.
    /// </summary>
    /// <remarks>Ensure the specified field name matches the expected format or schema of the data source. 
    /// Invalid or unsupported field names may result in errors or undefined behavior.</remarks>
    public string OrderBy { get; } = orderBy;

    #endregion
}