using System.ComponentModel.DataAnnotations;

namespace DibariBot.Database;

public abstract class DbModel
{
    [Key]
    public uint Id { get; set; }
}
