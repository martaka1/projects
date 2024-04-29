// NewGame.cshtml.cs
using DAL;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Helpers;
using Microsoft.AspNetCore.Mvc.Rendering;

public class NewGameModel : PageModel
{
    private readonly IGameRepository _gameRepository = default!;

    public NewGameModel(IGameRepository repository)
    {
        _gameRepository = repository;
    }

    [BindProperty]
    public Database.Game Game { get; set; } = default!;

    [BindProperty] 
    public GameOptions GameOptions { get; set; } = new GameOptions();

    [BindProperty] public GameState State { get; set; } = default!;

    [BindProperty] public List<Player> Players { get; set; }

    
    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        State = await Task.Run(() => _gameRepository.LoadGame(id));
        
        GameOptions = State.GameOptions;
        Players = State.Players;
        
        
        Game = new Database.Game()
        {
            Id = id,
            State = JsonSerializer.Serialize(State, JsonHelpers.JsonSerializerOptions),
            Players = State.Players.Select(p => new Database.Player()
            {
                NickName = p.NickName,
                PlayerType = p.PlayerType,
            }).ToList(),
        };
        return Page();
    }
    public async Task<IActionResult> OnPostAsync()
    {
        var players = await Task.Run(() => _gameRepository.LoadGame(Game.Id).Players);
        State.GameOptions = GameOptions;
        State.Players = players;
        Game.State = JsonSerializer.Serialize(State, JsonHelpers.JsonSerializerOptions);
        
        
        if (!ModelState.IsValid)
        {
            return Page();
        }
        
        _gameRepository.SaveGame(State.Id, State);

        return Page();
    }
}
