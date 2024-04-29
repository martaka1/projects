using System.Text.Json;
using DAL;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Game = Database.Game;

namespace RazorPagesController.Pages;
public class GamesPageModel : PageModel
{
    private readonly IGameRepository _repository;

    public GamesPageModel(ILogger<GamesPageModel> logger, IGameRepository repository)
    {
        _repository = repository;
    }
    public List<Game> Game { get; set; }

    public Task OnGetAsync()
    {
        var games = _repository.GetSaveGames();

        Game = new List<Game>();

        foreach (var gameData in games)
        {
            var state = _repository.LoadGame(gameData.id);
            if (state != null)
                Game.Add(new Game()
                {
                    Id = state.Id,
                    Players = state.Players
                        .Select(player => new Database.Player
                        {
                            NickName = player.NickName,
                            PlayerType = player.PlayerType
                        }).ToList(),
                    isOver=state.isOver 
                });
        }

        return Task.CompletedTask;
    }
    
}
