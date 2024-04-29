using DAL;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Uno.UnoEngine;

namespace RazorPagesController.Pages;

public class GamePlayerViewModel : PageModel
{
    private readonly IGameRepository _repository;
    public GameEngine Engine;
    public string BgColor;

    public GamePlayerViewModel(IGameRepository repository)
    {
        _repository = repository;
    }

    [BindProperty(SupportsGet = true)] public Guid GameId { get; set; }

    [BindProperty(SupportsGet = true)] public string PlayerNickname { get; set; }


    public async Task<IActionResult> OnGetAsync(Guid? gameIdNew, string? playerNickNameNew)
    {
        
        if (gameIdNew.HasValue)
        {
            GameId = gameIdNew.Value;
        }

        if (!string.IsNullOrEmpty(playerNickNameNew))
        {
            PlayerNickname = playerNickNameNew;
        }

        var state = await Task.Run(() => _repository.LoadGame(GameId));
        Engine = new GameEngine(state);
        Engine.Update();
        if (state.isOver)
        {
            return RedirectToPage("./WinPage", new {name = Engine.GetState().winner.NickName});
        }
        if (Engine.GetCurrentPlayer().PlayerType == EPlayerType.AI)
        {
            Engine.MakeAiMove();
        }

        Engine.Update();

        _repository.SaveGame(Engine.GetState().Id, Engine.GetState());

        return Page();
    }
    

}