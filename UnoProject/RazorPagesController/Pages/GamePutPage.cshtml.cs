using DAL;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Uno.UnoEngine;

public class GamePutPageModel : PageModel
{
    private IGameRepository _repository;

    public GameEngine Engine { get; set; } = default!;

    public GamePutPageModel(IGameRepository repository)
    {
        _repository = repository;
    }
    [BindProperty] public Guid GameId { get; set; }

    [BindProperty(SupportsGet = true)] public string PlayerNickName { get; set; }

    [BindProperty] public int CardIndex { get; set; }

    [BindProperty] public string PickCard { get; set; } = "False";

    public async Task<IActionResult> OnGetAsync(Guid gameidnew, string playerNameNew, int cardindexnew, string pickcardnew)
    {
        GameId = gameidnew;
        PlayerNickName = playerNameNew;
        CardIndex = cardindexnew;
        PickCard = pickcardnew;
        
        var state = await Task.Run(() => _repository.LoadGame(GameId));
        Engine = new GameEngine(state);

        if (Engine.GetCurrentPlayer().NickName != PlayerNickName)
        {
            return RedirectToPage("/GamePlayerView", new { GameId = gameidnew, PlayerNickName = playerNameNew });
        }

        if (state.isOver)
        {
            return RedirectToPage("./WinPage", new {name = Engine.GetState().winner.NickName});
        }
        if (CardIndex == 0 && PickCard.Equals("False"))
        {
            return RedirectToPage("./GamePlayerView", new {GameIdNew = GameId, PlayerNameNew = PlayerNickName});
        }

        if (PickCard.Equals("True"))
        {
            Engine.DrawCard(Engine.GetCurrentPlayer());
            Engine.MakePlayerMove(null);
            Engine.Update();
            _repository.SaveGame(Engine.GetState().Id, Engine.GetState());
            return RedirectToPage("/GamePlayerView", new { GameId = gameidnew, PlayerNickName = playerNameNew });
        }

        if (Engine.GetState().CardsToDraw != 0)
        {
            do
            {
                Engine.DrawCard(Engine.GetCurrentPlayer());
                Engine.Update();
            } while (Engine.GetState().CardsToDraw != 0);
            Engine.MakePlayerMove(null);
            Engine.Update();
            _repository.SaveGame(Engine.GetState().Id, Engine.GetState());
            return RedirectToPage("/GamePlayerView", new { GameId = gameidnew, PlayerNickName = playerNameNew });
        }

        if (CardIndex - 1 >= 0)
        {
            CardIndex--;
            if (Engine.GetCurrentPlayer().NickName.Equals(PlayerNickName))
            {
                var selected = Engine.GetPlayer(PlayerNickName).PlayerHand.ToArray();
                if (CardIndex < 0) CardIndex++;
                
                else
                {
                    var card = selected[CardIndex];
                    if (Engine.ValidatePlayerMove(card))
                    {
                        Engine.MakePlayerMove(card);
                    }
                    
                }
                Engine.Update();
                _repository.SaveGame(Engine.GetState().Id, Engine.GetState());
                return RedirectToPage("/GamePlayerView", new { GameId = gameidnew, PlayerNickName = playerNameNew });

            }
        }
        return RedirectToPage("/GamePlayerView", new { GameId = gameidnew, PlayerNickName = playerNameNew });
    }
}