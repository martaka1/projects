namespace Domain;

public class GameState
{
    public List<Player> Players { get; set; } = new List<Player>();
    public List<GameCard?> Deck { get; set; } = new List<GameCard?>();
    public List<GameCard?> Pile { get; set; } = new List<GameCard?>();
    
    public int CardsToDraw { get; set; }
    public int CurrentPlayerIndex { get; set; }
    public string FileName { get; set; }
    
    public GameOptions GameOptions { get; set; }
    public GameCard OnTable { get; set; }
    
    public bool isReverse { get; set; }

    public bool isOver { get; set; } = false;
    public Player winner { get; set; }
    
    public Guid Id { get; set; } =Guid.NewGuid();
    public int MaxPlayers { get; set; } = 10;

    public GameState Clone()
    {
        var cloned = new GameState
        {
            Id = Id,
            Players = Players.Select(player => player.Clone()).ToList(),
            CurrentPlayerIndex = CurrentPlayerIndex,
            CardsToDraw = CardsToDraw,
            isReverse = isReverse,
            isOver = isOver,
            OnTable = OnTable?.Clone(),
            MaxPlayers = MaxPlayers,
            Pile = Pile,
            Deck = Deck,
            winner = winner,
            GameOptions = GameOptions

        };
        return cloned;
    }
}
