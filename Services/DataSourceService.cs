using Licenser.Models;
using SQLite;

namespace Licenser.Services;

public class DataSourceService : SQLiteConnection
{
    public DataSourceService() : base(Path.Join(FileSystem.Current.AppDataDirectory, "local.db"), true)
    {
        CreateTable<KeyApp>();
    }
}
