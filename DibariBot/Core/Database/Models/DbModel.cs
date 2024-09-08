using System.ComponentModel.DataAnnotations;

namespace DibariBot.Database;

public abstract class DbModel
{
    // TODO: Migrate this to int
    [Key]
    public uint Id { get; set; }
}
