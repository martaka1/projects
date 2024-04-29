using DAL;
using Domain;
using Microsoft.Extensions.Logging;

namespace Uno.UnoEngine;

public class GameEngine
{
    private readonly Random _rnd = new Random();

    private readonly GameOptions _gameOptions;
    
    private List<Player> _players;
    private List<GameCard> _deck;
    private List<GameCard> _pile;
    private readonly GameState _state;
    private int _currentPlayerIndex;
    private bool _isOver;
    private int _cardsToDraw;
    private bool _isReverse;
    private Player? _winner;
    private GameCard _onTable;
    

    public GameEngine(GameState gameState)
    {
        _players = gameState.Players;
        _deck = gameState.Deck;
        _pile = gameState.Pile;
        _currentPlayerIndex = gameState.CurrentPlayerIndex;
        _isOver = gameState.isOver;
        _cardsToDraw = gameState.CardsToDraw;
        _isReverse = gameState.isReverse;
        _winner = gameState.winner;
        _gameOptions = gameState.GameOptions ?? new GameOptions();
        _onTable = gameState.OnTable;
        
        var count = _players.Sum(t => t.GetHand().Count);
        if (count == 0)
        {
            InitializeDeck();
        }
        _state = gameState;
    }

    public void Update()
    {
        _state.Deck = _deck;
        _state.Players = _players;
        _state.Pile = _pile;
        _state.isOver = _isOver;
        _state.CardsToDraw = _cardsToDraw;
        _state.isReverse = _isReverse;
        _state.winner = _winner;
        _state.CurrentPlayerIndex = _currentPlayerIndex;
        _state.OnTable = _onTable;
    }
    
    public void InitializeDeck()
    {
        //============CREATE DECK===============
        // Add 4 cards of each color (0)
        CreateDeck();

        //============DESTRIBUTE CARDS===============
        DestributeCards();
        //============START CARD===============
        _pile.Add(_deck[0]);
        _onTable = _deck[0];
        _deck.RemoveAt(0);
        
        
    }

    public void Shuffle()
    {
        int n = _deck.Count;
        while (n > 1)
        {
            n--;
            int k = _rnd.Next(n + 1);
            (_deck[k], _deck[n]) = (_deck[n], _deck[k]);
        }
    }

    private void DestributeCards()
    {
        GameCard? card;
        var handSize = _gameOptions.HandSize;
        if (handSize == null)
        {
            handSize = 7;
        }
        foreach (var player in _players)
        {
            for (int i = 0; i < handSize; i++)
            {
                card = _deck[0];
                _deck.RemoveAt(0);

                player.PlayerHand.Add(card);
            }
        }
    }

    public void CreateDeck()
    {
        _deck = new List<GameCard>(); 
        
        
        _deck.Add(new GameCard { CardColor = ECardColor.Red, CardValue = ECardValue.Value0 });
        _deck.Add(new GameCard { CardColor = ECardColor.Blue, CardValue = ECardValue.Value0 });
        _deck.Add(new GameCard { CardColor = ECardColor.Green, CardValue = ECardValue.Value0 });
        _deck.Add(new GameCard { CardColor = ECardColor.Yellow, CardValue = ECardValue.Value0 });

        // Add 8 cards of each color (1-9, Skip, Reverse, Add2)
        foreach (ECardColor color in Enum.GetValues(typeof(ECardColor)))
        {
            if (color == ECardColor.Wild) continue; // Skip Wild cards
            foreach (ECardValue value in Enum.GetValues(typeof(ECardValue)))
            {
                if (value >= ECardValue.Value1 && value <= ECardValue.ValueAdd2)
                {
                    _deck.Add(new GameCard { CardColor = color, CardValue = value });
                    _deck.Add(new GameCard { CardColor = color, CardValue = value });
                }
            }
        }

        // Add 4 Wild cards (ChangeColor)
        for (int i = 0; i < 4; i++)
        {
            _deck.Add(new GameCard { CardColor = ECardColor.Wild, CardValue = ECardValue.ValueChangeColor });
        }

        // Add 4 Wild Draw Four cards (Add4)
        for (int i = 0; i < 4; i++)
        {
            _deck.Add(new GameCard { CardColor = ECardColor.Wild, CardValue = ECardValue.ValueAdd4 });
        }
        Shuffle();
    }

    public void MakePlayerMove(GameCard? gameCard)
    {
        if (gameCard == null)
        {
            CurrentPlayerIndex(false);
        }
        else
        {
            _players[_currentPlayerIndex].Remove(gameCard);

            switch (gameCard.CardValue)
            {
                case ECardValue.ValueAdd2:
                    _cardsToDraw += 2;
                    CurrentPlayerIndex(true);
                    break;

                case ECardValue.ValueSkip:
                    CurrentPlayerIndex(true);
                    break;

                case ECardValue.ValueReverse:
                    _isReverse = !_isReverse;
                    break;
            }

            _pile.Add(gameCard);
            _onTable = gameCard;
            CheckForWinner(); 
            CurrentPlayerIndex(false);
        }
    }


    public void ChangeColor(char ChosenColor, int playerChoice)
    {
        GameCard? selectedCard =_players[_currentPlayerIndex].PlayerHand[playerChoice] ;
        _players[_currentPlayerIndex].PlayerHand.Remove(selectedCard);
        if (selectedCard. CardValue == ECardValue.ValueAdd4 )
        {
            _cardsToDraw += 4;
        }
        selectedCard.CardColor = GetColorFromChar(ChosenColor);
        _pile.Add(selectedCard);
        MakePlayerMove(null);
    }

    public void MakeAiMove()
    {
        GameCard? playCard = null;
        List<GameCard?> matchingColorCards = new List<GameCard?>();
        List<GameCard?> matchingValueCards = new List<GameCard?>();
        List<GameCard?> wildCards = new List<GameCard?>();

        // Separate the cards into matching color, matching value, and wild cards
        foreach (GameCard? card in _players[_currentPlayerIndex].PlayerHand)
        {
            if (card!.CardColor == _pile[^1]!.CardColor)
            {
                matchingColorCards.Add(card);
            }
            else if (card.CardValue == _pile[^1]!.CardValue)
            {
                matchingValueCards.Add(card);
            }
            else if (card.CardColor == ECardColor.Wild)
            {
                wildCards.Add(card);
            }
        }

        // First, choose a card by color
        if (matchingColorCards.Count > 0)
        {
            playCard = ChooseCardByColor(matchingColorCards);
        }
        // If no matching color card, choose by value
        else if (matchingValueCards.Count > 0)
        {
            playCard = ChooseCardByValue(matchingValueCards);
        }
        // If no matching color or value card, choose a wild card and set its color randomly
        else if (wildCards.Count > 0)
        {
            playCard = ChooseWildCard(wildCards);
            if (playCard.CardValue == ECardValue.ValueAdd4)
            {
                _cardsToDraw += 4;
            }
            // Set the color of the wild card randomly (red, blue, yellow, green)
            playCard!.CardColor = GetRandomColor();
            _players[_currentPlayerIndex].PlayerHand.Remove(playCard);

            _pile.Add(playCard);
            MakePlayerMove(null);
        }
        else
        {
            DrawCard(_players[_currentPlayerIndex]);
            CurrentPlayerIndex(false);
        }

        if (playCard != null)
        {
            MakePlayerMove(playCard);
        }
        
    }

    // Helper method to choose a card by color
    private GameCard? ChooseCardByColor(List<GameCard?> cards)
    {
        return cards[0];
    }

    // Helper method to choose a card by value
    private GameCard? ChooseCardByValue(List<GameCard?> cards)
    {
        return cards[0];
    }

    // Helper method to choose a wild card
    private GameCard? ChooseWildCard(List<GameCard?> wildCards)
    {
        return wildCards[0];
    }

    // Helper method to get a random color (red, blue, yellow, green)
    private ECardColor GetRandomColor()
    {
            int index = _rnd.Next(4);  // 0 for red, 1 for blue, 2 for yellow, 3 for green
            return (ECardColor)index;
        
    }


    public void CurrentPlayerIndex(bool isSkip)
    {
        int direction = _isReverse ? -1 : 1;

        if (isSkip)
        {
            _currentPlayerIndex += 2 * direction;
            if (_currentPlayerIndex < 0)
            {
                _currentPlayerIndex = _players.Count - 1;
            }
            else if (_currentPlayerIndex >= _players.Count)
            {
                _currentPlayerIndex = 0;
                    
            }
        }
        else
        {
            _currentPlayerIndex += direction;
            if (_currentPlayerIndex < 0)
            {
                _currentPlayerIndex = _players.Count - 1;
            }
            else if (_currentPlayerIndex >= _players.Count)
            {
                _currentPlayerIndex = 0;
            }
        }
    }


    private void CheckForWinner()
    {
        foreach (var player in _players)
        {
            if (player.PlayerHand.Count == 0)
            {
                _isOver = true;
                _winner = player;
            }
        }
    }

    public void DrawCard(Player currentPlayer)
    {
        if (_deck.Count <= 0)
        {
            InitializeDeck();
        }
        currentPlayer.PlayerHand.Add(_deck[0]);
        _deck.RemoveAt(0);
        if (_cardsToDraw != 0)
        {
            _cardsToDraw -= 1;
        }
    }

    public Player GetCurrentPlayer()
    {
        var current = _players[_currentPlayerIndex];
        return current;
    }

    public GameState GetState()
    {
        var current = _state.Clone();
        return current;
    }

    public Player GetPlayer(string name)
    {
        return (from index in _players where index.NickName.Equals(name) select index).FirstOrDefault()!;
    }

    
    public bool ValidatePlayerMove(GameCard? selectedCard)
    {
        if (selectedCard.CardColor == _pile[^1]!.CardColor)
        {
            return true;
        }
        if (selectedCard.CardValue == _pile[^1]!.CardValue)
        {
            return true;
        }

        if (selectedCard.CardColor == ECardColor.Wild)
        {
            return true;
        }
        return false;
    }
    

    private ECardColor GetColorFromChar(char colorChoice)
    {
        switch (colorChoice)
        {
            case 'R':
                return ECardColor.Red;
            case 'B':
                return ECardColor.Blue;
            case 'G':
                return ECardColor.Green;
            case 'Y':
                return ECardColor.Yellow;
            default:
                return ECardColor.Wild; // Default to Wild if invalid choice
        }
    }
    

    public void WildCard(ECardColor color, int cardIndexSaved)
    {
        var card = _players[_currentPlayerIndex].GetHand()[cardIndexSaved-1];
        if (card.CardValue == ECardValue.ValueAdd4)
        {
            _cardsToDraw += 4;
        }
        _players[_currentPlayerIndex].Remove(card);
        card.CardColor = color;
        card.CardValue = ECardValue.ValueColorChanged;
        _pile.Add(card);
        _onTable = card;
    }
}