using System.Reflection;
using SqlCommands.Attributes;

namespace SqlCommands.Metadata;

public class PropertyMetadata(PropertyInfo propertyInfo, SqlColumnAttribute columnAttribute)
{
    #region Properties

    /// <summary>
    /// Gets the SQL column attribute associated with the property.
    /// </summary>
    public SqlColumnAttribute ColumnAttribute { get; } = columnAttribute;

    /// <summary>
    /// Gets the name of the SQL column associated with the property.
    /// </summary>
    public string ColumnName { get; } = columnAttribute.Name ?? propertyInfo.Name;

    /// <summary>
    /// Gets the property information.
    /// </summary>
    public PropertyInfo PropertyInfo { get; } = propertyInfo;

    #endregion
}