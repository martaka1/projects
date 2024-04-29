namespace Domain;

public class Game
{
    public Guid Id { get; private set; }
    public GameState CurrentGameState { get; private set; }
    public List<Player> Players { get; set; }
    public Game(Guid id, GameState currentGameState)
    {
        Id = id;
        CurrentGameState = currentGameState;
    }
}