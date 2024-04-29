using System.Text.Json;
using Domain;
using Helpers;
using Microsoft.EntityFrameworkCore;

namespace DAL;

public class GameRepositoryEF : IGameRepository
{
    private readonly AppDbContext _ctx;

    public GameRepositoryEF(AppDbContext ctx)
    {
        _ctx = ctx;
    }

    public void SaveGame(Guid id, GameState state)
    {
        var game = _ctx.Games.FirstOrDefault(g => g.Id == id);
        if (game == null)
        {
            game = new Database.Game()
            {
                Id = id,
                State = JsonSerializer.Serialize(state, JsonHelpers.JsonSerializerOptions),
                Players = state.Players.Select(p => new Database.Player()
                {
                    NickName = p.NickName,
                    PlayerType = p.PlayerType
                }).ToList(),
                isOver = state.isOver
            };
            _ctx.Games.Add(game);
        }
        else
        {
            game.UpdatedAtDt = DateTime.Now;
            game.State = JsonSerializer.Serialize(state, JsonHelpers.JsonSerializerOptions);
            game.isOver = state.isOver;
        }

        var changeCount = _ctx.SaveChanges();
        Console.WriteLine("SaveChanges: " + changeCount);
    }
    
    public List<(Guid id, DateTime dt)> GetSaveGames()
    {
        return _ctx.Games
            .OrderByDescending(g => g.UpdatedAtDt)
            .ToList()
            .Select(g => (g.Id, g.UpdatedAtDt))
            .ToList();
    }

    public GameState LoadGame(Guid id)
    {
        var game = _ctx.Games.FirstOrDefault(g => g.Id == id);

        if (game != null)
        {
            return JsonSerializer.Deserialize<GameState>(game.State, JsonHelpers.JsonSerializerOptions)!;
        }
    
        // Handle the case where no game is found with the specified id
        return null;
    }

}