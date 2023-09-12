using System.ComponentModel.DataAnnotations;

namespace DibariBot.Database.Models;

public class DbModel
{
    [Key]
    public uint Id { get; set; }
}
