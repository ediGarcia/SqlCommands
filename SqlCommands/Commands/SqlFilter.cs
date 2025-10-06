namespace SqlCommands.Commands;

public class SqlFilter(string text, IEnumerable<SqlParameter> parameters)
{
    #region Properties

    /// <summary>
    /// Gets the parameters of the filter.
    /// </summary>
    public IEnumerable<SqlParameter> Parameters { get; } = parameters;

    /// <summary>
    /// Gets the text of the filter.
    /// </summary>
    public string Text { get; } = text;

    #endregion
}