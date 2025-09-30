namespace BingeBuddy.Helpers
{
    public static class Constants
    {
        // Replace with your actual TMDb API key
        public const string TMDB_API_KEY = "32abc095da76ccc08b0d657136d45b04";
        public const string TMDB_BASE_URL = "https://api.themoviedb.org/3";
        public const string TMDB_IMAGE_BASE_URL = "https://image.tmdb.org/t/p";

        // Database
        public const string DatabaseFilename = "binge_buddy.db3";
        public const SQLite.SQLiteOpenFlags Flags =
            SQLite.SQLiteOpenFlags.ReadWrite |
            SQLite.SQLiteOpenFlags.Create |
            SQLite.SQLiteOpenFlags.SharedCache;

        public static string DatabasePath =>
            Path.Combine(FileSystem.AppDataDirectory, DatabaseFilename);
    }
}