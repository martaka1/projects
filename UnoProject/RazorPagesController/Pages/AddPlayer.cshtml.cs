using DAL;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

public class AddPLayerModel : PageModel
{
    
    private readonly IGameRepository _gameRepository;
    
    public AddPLayerModel(IGameRepository repository)
    {
        _gameRepository = repository;
    }
    
    [BindProperty]
    public Database.Game Game { get; set; } = default!;

    [BindProperty] public Guid Gameid { get; set; } = default;

    [BindProperty] public GameState State { get; set; } = default!;

    [BindProperty] public Player _player { get; set; } = new Player();

    [BindProperty] public string name { get; set; } = default;
    
    public SelectList Options { get; set; }
    
    [BindProperty] public string Type { get; set; } = "False";
    
    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        Gameid = id;

        State = await Task.Run(() => _gameRepository.LoadGame(id));
        
        var optionsList = new List<SelectListItem>
        {
            new SelectListItem { Value = "AI", Text = "AI" },
            new SelectListItem { Value = "Human", Text = "Human" }
        };
        
        
        Options = new SelectList(optionsList, "Value", "Text");
        
        return Page();
    }
    
    public async Task<IActionResult> OnPostAsync()
    {
        _player.NickName = name;
        if (Type.Equals("Human"))
        {
            _player.PlayerType = EPlayerType.Human;
        }
        else
        {
            _player.PlayerType = EPlayerType.AI;
        }

        var state = await Task.Run(() => _gameRepository.LoadGame(Gameid));

        if (!state.Players.Any(m => m.NickName.Equals(_player.NickName)))
        {
            state.Players.Add(_player);
        }
        
        _gameRepository.SaveGame(Gameid, state);
        
        return RedirectToPage("./NewGame", new {id = Game.Id});
    }
}