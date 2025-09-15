namespace SqlCommands.Commands;

public class SqlParameter(string name, object value)
{
    #region Properties

    /// <summary>
    /// Gets the name of the parameter.
    /// </summary>
    public string Name { get; } = name;

    /// <summary>
    /// Gets the value of the parameter.
    /// </summary>
    public object Value { get; } = value;

    #endregion
}