namespace SqlCommands.Attributes;

[Flags]
public enum IgnoreRule
{
    None,
    SelectAlways = 1,
    InsertAlways = 2,
    UpdateAlways = 4,
    DeleteAlways = 8,
    UpsertAlways = 16,
    SelectIfNull = 32,
    InsertIfNull = 64,
    UpdateIfNull = 128,
    DeleteIfNull = 256,
    UpsertIfNull = 512,
    Always = SelectAlways | InsertAlways | UpdateAlways | DeleteAlways | UpsertAlways,
    AlwaysIfNull = SelectIfNull | InsertIfNull | UpdateIfNull | DeleteIfNull | UpsertIfNull
}