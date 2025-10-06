namespace SqlCommands.Commands;

public class SqlCommand(string text, IEnumerable<SqlParameter> parameters)
{
    #region Properties

    /// <summary>
    /// Gets the parameters used in the SQL command.
    /// </summary>
    public IEnumerable<SqlParameter> Parameters { get; } = parameters;

    /// <summary>
    /// Gets the SQL command text.
    /// </summary>
    public string Text { get; } = text;

    #endregion
}