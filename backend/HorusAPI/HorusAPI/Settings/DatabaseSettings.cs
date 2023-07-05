namespace HorusAPI.Settings
{
    public class DatabaseSettings
    {
        public string ConnectionString { get; set; } = null!;

        public string DatabaseName { get; set; } = null!;

        public Dictionary<string, string> Collections { get; set; } = null!;
    }
}
