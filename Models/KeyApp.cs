using SQLite;

namespace Licenser.Models;

public class KeyApp
{
    [PrimaryKey, AutoIncrement]
    public uint Id { get; set; }
    [MaxLength(255), NotNull]
    public string Name { get; set; } = null!;
    [MaxLength(255), NotNull]
    public string Secret { get; set; } = null!;
}
