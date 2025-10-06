using SqlCommands.Attributes;

namespace SqlCommands.Metadata;

public class ClassMetadata(Type classType, SqlTableAttribute tableAttribute, IReadOnlyCollection<PropertyMetadata> propertiesMetadata)
{
    #region Properties

    /// <summary>
    /// Gets the type of the class.
    /// </summary>
    public Type ClassType { get; } = classType;

    /// <summary>
    /// Gets the metadata of the properties of the class.
    /// </summary>
    public IReadOnlyCollection<PropertyMetadata> PropertiesMetadata { get; } = propertiesMetadata;

    /// <summary>
    /// Gets the SQL table attribute associated with the class.
    /// </summary>
    public SqlTableAttribute TableAttribute { get; } = tableAttribute;

    /// <summary>
    /// Gets the name of the SQL table associated with the class.
    /// </summary>
    public string TableName { get; } = tableAttribute.Name ?? classType.Name;

    #endregion
}