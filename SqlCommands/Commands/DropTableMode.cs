namespace SqlCommands.Commands;

public enum DropTableMode
{
    Unsafe,
    Cascade,
    Restrict,
}