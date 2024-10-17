using KeyGenerator.Models;
using SQLite;

namespace KeyGenerator.Services;

public class DataSourceService : SQLiteConnection
{
    public DataSourceService() : base(Path.Join(FileSystem.Current.AppDataDirectory, "local.db"), true)
    {
        CreateTable<KeyApp>();
    }
}
