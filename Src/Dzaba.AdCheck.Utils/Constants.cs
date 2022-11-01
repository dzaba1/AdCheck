namespace Dzaba.AdCheck.Utils
{
    public static class Constants
    {
        public static readonly string SerilogFormat = "{Timestamp:yyyy-MM-dd HH:mm:ss,fff} [{ThreadId}] {Level:u3} {SourceContext} - {Message:lj}{NewLine}{Exception}";
    }
}
