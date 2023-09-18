using System.ComponentModel.DataAnnotations;

namespace DibariBot.Core.Database.Models;

public class DbModel
{
    [Key]
    public uint Id { get; set; }
}
