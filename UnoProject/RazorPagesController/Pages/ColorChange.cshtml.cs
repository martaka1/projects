using DAL;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Uno.UnoEngine;

public class ColorChangeModel : PageModel
{
    

    private readonly IGameRepository _gameRepository = default!;
    
    public ColorChangeModel(IGameRepository gameRepository)
    {
        _gameRepository = gameRepository;
    }
    
    [BindProperty] public Guid GameIdReal { get; set; }

    [BindProperty] public string PlayerNameSaved { get; set; }

    [BindProperty] public int CardIndexSaved { get; set; }

    [BindProperty] public string PickCardSaved { get; set; } = "False";
    
    [BindProperty] public string Color { get; set; } = "False";
    
    public SelectList Options { get; set; }
    
    

    public async Task<IActionResult> OnPostAsync()
    {
       
        var state = await Task.Run(() => _gameRepository.LoadGame(GameIdReal));
        var engine = new GameEngine(state);
        var ordered = engine.GetPlayer(PlayerNameSaved).GetHand().ToList();
        switch (Color)
        {
            case("Red"):
                engine.WildCard(ECardColor.Red, CardIndexSaved);
                break;
            case("Blue"):
                engine.WildCard(ECardColor.Blue, CardIndexSaved);
                break;
            case("Yellow"):
                engine.WildCard(ECardColor.Yellow, CardIndexSaved);
                break;
            case("Green"):
                engine.WildCard(ECardColor.Green, CardIndexSaved);
                break;
        }
        engine.MakePlayerMove(null);
        engine.Update();
        _gameRepository.SaveGame(engine.GetState().Id, engine.GetState());
        return RedirectToPage("/GamePlayerView", new { GameIdNew = GameIdReal, PlayerNickname = PlayerNameSaved});
    }
    
    
    public async Task<IActionResult> OnGetAsync(Guid GameId, string PlayerName, int CardIndex, string PickCard)
    {
         GameIdReal= GameId;
         PlayerNameSaved = PlayerName;
         CardIndexSaved = CardIndex;
         PickCardSaved = PickCard;
         var state = await Task.Run(() => _gameRepository.LoadGame(GameIdReal));

         var engine = new GameEngine(state);
         if (engine.GetCurrentPlayer().NickName != PlayerNameSaved)
         {
             return RedirectToPage("/GamePlayerView", new { GameIdNew = GameIdReal, PlayerNickname = PlayerNameSaved });
         }
         
         var optionsList = new List<SelectListItem>
         {
             new SelectListItem { Value = "Red", Text = "Red" },
             new SelectListItem { Value = "Blue", Text = "Blue" },
             new SelectListItem { Value = "Yellow", Text = "Yellow" },
             new SelectListItem { Value = "Green", Text = "Green" },
         };

         Options = new SelectList(optionsList, "Value", "Text");

         return Page();
    }
}