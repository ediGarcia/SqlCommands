namespace SqlCommands.Commands;

public class SqlCommand(string text, SqlParameter[] parameters)
{
    #region Properties

    /// <summary>
    /// Gets the parameters used in the SQL command.
    /// </summary>
    public SqlParameter[] Parameters { get; } = parameters;

    /// <summary>
    /// Gets the SQL command text.
    /// </summary>
    public string Text { get; } = text;

    #endregion
}