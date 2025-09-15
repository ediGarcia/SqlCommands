using SqlCommands.Attributes;
using System.Reflection;

namespace SqlCommands.Metadata;

public static class ClassMetadataCache
{
    private static readonly Dictionary<Type, ClassMetadata> CachedData = [];

    #region Public Methods

    #region GetClassMetadata
    /// <summary>
    /// Retrieves the class metadata for the specified class type.
    /// </summary>
    /// <param name="classType"></param>
    /// <returns></returns>
    /// <remarks>The obtained class metadata is cached for future requests.</remarks>
    public static ClassMetadata GetClassMetadata(Type classType)
    {
        if (CachedData.TryGetValue(classType, out ClassMetadata metadata))
            return metadata;

        SqlTableAttribute tableAttribute = classType.GetCustomAttribute<SqlTableAttribute>() ?? new();
        PropertyInfo[] properties = classType.GetProperties();
        List<PropertyMetadata> propertiesMetadata = new(properties.Length);

        foreach (PropertyInfo property in properties)
        {
            SqlColumnAttribute columnAttribute = null;
            bool skipProperty = false;

            foreach (Attribute attribute in property.GetCustomAttributes())
                if (attribute is SqlIgnoreAttribute)
                {
                    skipProperty = true;
                    break;
                }
                else if (attribute is SqlColumnAttribute sqlColumnAttribute)
                    columnAttribute = sqlColumnAttribute;

            if (skipProperty)
                continue;

            propertiesMetadata.Add(new(property, columnAttribute ?? new()));
        }

        ClassMetadata cachedMetadata = new(classType, tableAttribute, propertiesMetadata.ToArray());
        CachedData[classType] = cachedMetadata;

        return cachedMetadata;
    }
    #endregion

    #endregion
}