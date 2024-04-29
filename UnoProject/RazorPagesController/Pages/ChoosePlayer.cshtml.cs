using DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Database;

public class ChoosePlayerModel : PageModel
{
    private readonly IGameRepository _repository;

    public ChoosePlayerModel(IGameRepository repository)
    {
        _repository = repository;
    }

    [BindProperty(SupportsGet = true)]
    public Guid GameId { get; set; }

    public List<Player> Players { get; set; }

    public async Task OnGetAsync()
    {
        var state = _repository.LoadGame(GameId);
        
        if (state != null)
        {
            Players = state.Players.Select(player => new Database.Player
            {
                NickName = player.NickName,
                PlayerType = player.PlayerType
            }).ToList();
        }
    }
}