using Domain;

namespace ConsoleUI;



public class ConsoleVisualization
{
    public static void DrawDesk(GameState state)
    {
        Console.WriteLine($">>{state.Players[state.CurrentPlayerIndex].NickName}'s turn<<");
        for (var i = 0; i < state.Players.Count; i++) {
                    Console.WriteLine($"Player {i+1}-{state.Players[i].NickName} has {state.Players[i].PlayerHand.Count} cards");
        }
        Console.WriteLine($"Card in the pile: {state.Pile[state.Pile.Count-1]}");
        
    }
    public static void DrawPlayerHand(Player player)
    {
        Console.WriteLine("Your hand is: ");
        for (int i = 0; i < player.PlayerHand.Count; i++)
        {
            Console.Write($"{player.PlayerHand[i]}   ");
        }
        Console.WriteLine();
    }

}