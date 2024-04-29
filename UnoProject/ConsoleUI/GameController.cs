using DAL;
using Domain;
using Uno.UnoEngine;

namespace ConsoleUI;

public class GameController
{
    private readonly GameEngine _engine;
    private readonly IGameRepository _repository;
    private GameState _state;
    

    public GameController(GameEngine engine, IGameRepository repository, GameState state)
    {
        _engine = engine;
        _repository = repository;
        _state = state;
    }
    public void Run()
    {
        Console.Clear();
        while (true)
        {
            _engine.Update();
            if (_engine.GetState().isOver)
            {
                Console.WriteLine($"{_engine.GetState().winner.NickName} has won!");
                break;
            }

            if (_engine.GetState().Deck.Count == 0)
            {
                _engine.CreateDeck();
            }
            _engine.Update();
            if (_engine.GetState().CardsToDraw != 0)
            {
                do
                {
                    _engine.DrawCard(_engine.GetCurrentPlayer());
                    _engine.Update();
                } while (_engine.GetState().CardsToDraw != 0);

                // Make the player move if needed
                _engine.MakePlayerMove(null);
                _engine.Update();
            }

            // one move in loop
            _engine.Update();
            Console.WriteLine($"Player {_engine.GetState().CurrentPlayerIndex + 1} - {_engine.GetCurrentPlayer().NickName}");
            Console.Write("Your turn, make sure you are alone looking at screen! Press enter to continue...");
            Console.ReadLine();
            
            Console.Clear();
            
            Console.WriteLine($"Player {_engine.GetState().CurrentPlayerIndex + 1} - {_engine.GetCurrentPlayer().NickName}");
            ConsoleVisualization.DrawDesk(_engine.GetState());

            while (true)
            {
                _engine.Update();
                  if (_engine.GetCurrentPlayer().PlayerType == EPlayerType.AI)
                  {
                      ConsoleVisualization.DrawPlayerHand(_engine.GetCurrentPlayer());
                      _engine.MakeAiMove();
                  }
                  else
                  {
                      ConsoleVisualization.DrawPlayerHand(_engine.GetCurrentPlayer());
                      Console.Write($"Do you want to draw from the deck?[Y/N]");
                      var playerDraw = Console.ReadLine()?.ToLower().Trim();
                      if (playerDraw == "y")
                          
                      {
                          _engine.DrawCard(_engine.GetCurrentPlayer());
                          _engine.MakePlayerMove(null);
                      }
                      else
                      {
                          var playerChoice = ChooseCard(_engine.GetCurrentPlayer());
                  
                          if (_engine.ValidatePlayerMove(_engine.GetCurrentPlayer().PlayerHand[playerChoice]) == false)
                          {
                              Console.WriteLine("Selected cards are not valid");
                              continue;
                          }
                          if (_engine.GetCurrentPlayer().PlayerHand[playerChoice]!.CardValue ==
                              ECardValue.ValueAdd4 ||
                              _engine.GetCurrentPlayer().PlayerHand[playerChoice]!.CardValue ==
                              ECardValue.ValueChangeColor)
                          {
                              char colorChoice;
                              do
                              {
                                  Console.WriteLine($"Choose color (R)ed, (B)lue, (G)reen, (Y)ellow");
                                  colorChoice = Convert.ToChar(Console.ReadLine()?.ToUpper().Trim()!);
                              } while (colorChoice != 'R' && colorChoice != 'B' && colorChoice != 'G' && colorChoice != 'Y');
                                   
                              _engine.ChangeColor(colorChoice, playerChoice);
                          }
                          else
                          { 
                              _engine.MakePlayerMove(_engine.GetCurrentPlayer().PlayerHand[playerChoice]);
                          } 
                      }

                  }
                  
                  break;
            }  
            _engine.Update();
            _repository.SaveGame(_state.Id, _state);
            
            Console.Write("State saved. Continue (Y/N)[Y]?");
            var continueAnswer = Console.ReadLine()?.Trim().ToLower();
            
            if (continueAnswer is "n") break;
        }
    }
    
    public static int ChooseCard(Player currentPlayer)
    {
        Console.WriteLine("Your hand:");
        int selectedIndex = 0;
        
        int padding = 2;
        Console.SetCursorPosition(0, Console.CursorTop);
        for (int i = 0; i < currentPlayer.PlayerHand.Count; i++)
        {
            if (i == selectedIndex)
            {
                Console.Write($"> {currentPlayer.PlayerHand[i]} ".PadRight(padding));
            }
            else
            {
                Console.Write($"  {currentPlayer.PlayerHand[i]} ".PadRight(padding));
            }
        }


        ConsoleKeyInfo key;

        do
        {
            key = Console.ReadKey(true);

            if (key.Key == ConsoleKey.UpArrow || key.Key == ConsoleKey.LeftArrow)
            {
                selectedIndex = (selectedIndex - 1 + currentPlayer.PlayerHand.Count) % currentPlayer.PlayerHand.Count;
                Console.SetCursorPosition(0, Console.CursorTop);
            }
            else if (key.Key == ConsoleKey.RightArrow || key.Key == ConsoleKey.DownArrow)
            {
                selectedIndex = (selectedIndex + 1) % currentPlayer.PlayerHand.Count;
                Console.SetCursorPosition(0, Console.CursorTop);
            }
            
            if (key.Key != ConsoleKey.Enter)
            {
                for (int i = 0; i < currentPlayer.PlayerHand.Count; i++)
                {
                    if (i == selectedIndex)
                    {
                        Console.Write($"> {currentPlayer.PlayerHand[i]} ".PadRight(padding));
                    }
                    else
                    {
                        Console.Write($"  {currentPlayer.PlayerHand[i]} ".PadRight(padding));
                    }
                }
            }
        } while (key.Key != ConsoleKey.Enter);

        return selectedIndex;
    }
}