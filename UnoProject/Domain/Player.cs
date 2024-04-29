namespace Domain;

public class Player
{
    public string NickName { get; set; } = default!;
    public EPlayerType PlayerType { get; set; }
    
    public List<GameCard> PlayerHand { get; set; } = new List<GameCard?>();
    public Player()
    {
        this.NickName = "UnknownPlayer";
        this.PlayerHand = new List<GameCard>();
        this.PlayerType = EPlayerType.Human;
    }
    public Player(string name = "UnknownPlayer", EPlayerType type = EPlayerType.Human)
    {
        this.NickName = name;
        this.PlayerHand = new List<GameCard>();
        this.PlayerType = type;
    }
    
    public void Remove(GameCard card)
    {
        for (int i = 0; i < PlayerHand.Count; i++)
        {
            if (PlayerHand[i].Equals(card))
            { 
                PlayerHand.Remove(PlayerHand[i]);
                break;
            }
        }
    }
    public List<GameCard> GetHand()
    {
        return PlayerHand;
    }

    public Player Clone()
    {
        Player result = new Player(NickName, PlayerType);
        result.PlayerHand = PlayerHand;

        return result;
    }
}