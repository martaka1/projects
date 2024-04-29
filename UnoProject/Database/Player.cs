using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Database;

public class Player : BaseEntity
{
    [MaxLength(128)]
    public string NickName { get; set; } = default!;

    public EPlayerType PlayerType { get; set; }

    // use convenience naming <class>Id
    // nullability decides relationship type - mandatory or not
    public List<Game> GamePlayers { get; set; }
}