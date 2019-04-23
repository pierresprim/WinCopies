namespace WinCopiesProcessesManager
{
    public enum HowToRetry
    {
        None = 0,
        Ignore = 1,
        Retry = 2,
        Rename = 3,
        Replace = 4,
        Cancel = 5,
        CheckFiles = 6,
        IgnoreWhenSameContent = 7,
        RenameWhenNotSameContent = 8,
        ReplaceWhenNotSameContent = 9
    }
}
