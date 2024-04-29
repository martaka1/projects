using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using Domain;
using Helpers;

namespace Database;
using Microsoft.EntityFrameworkCore;


public class Game : BaseEntity
{
    public DateTime CreatedAtDt { get; set; } = DateTime.Now;
    public DateTime UpdatedAtDt { get; set; } = DateTime.Now;

    public string State { get; set; } = default!;

    // null, if you did not do the join (.include in c#)
    public List<Player> Players { get; set; }

    public bool isOver { get; set; } = false;

}