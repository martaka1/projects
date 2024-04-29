using System.ComponentModel.DataAnnotations.Schema;
namespace Domain
{
    [ComplexType]
    public class GameCard
    {
        
        public ECardColor CardColor { get; set; }
        public ECardValue CardValue { get; set; }

        public override string ToString()
        {
            return CardColorToString() +" "+ CardValueToString();
        }
        
        private string CardColorToString()
        {
            return CardColor switch
            {
                ECardColor.Red => "🟥",
                ECardColor.Blue => "🟦",
                ECardColor.Yellow => "🟨",
                ECardColor.Green => "🟩",
                ECardColor.Wild => "⬛️",
                _ => "-"
            };
        }
        
        private string CardValueToString()
        {
            return CardValue switch
            {
                ECardValue.Value0 => "0",
                ECardValue.Value1 => "1",
                ECardValue.Value2 => "2",
                ECardValue.Value3 => "3",
                ECardValue.Value4 => "4",
                ECardValue.Value5 => "5",
                ECardValue.Value6 => "6",
                ECardValue.Value7 => "7",
                ECardValue.Value8 => "8",
                ECardValue.Value9 => "9",
                ECardValue.ValueSkip => "🚫",
                ECardValue.ValueReverse => "🔃",
                ECardValue.ValueAdd2 => "+2",
                ECardValue.ValueAdd4 => "+4",
                ECardValue.ValueChangeColor=> "🔴🟡🟢🔵",
                ECardValue.ValueColorChanged => " "
            };
        }

        public object GetValue()
        {
            return CardValue switch
            {
                ECardValue.Value0 => "0",
                ECardValue.Value1 => "1",
                ECardValue.Value2 => "2",
                ECardValue.Value3 => "3",
                ECardValue.Value4 => "4",
                ECardValue.Value5 => "5",
                ECardValue.Value6 => "6",
                ECardValue.Value7 => "7",
                ECardValue.Value8 => "8",
                ECardValue.Value9 => "9",
                ECardValue.ValueSkip => "🚫",
                ECardValue.ValueReverse => "🔃",
                ECardValue.ValueAdd2 => "+2",
                ECardValue.ValueAdd4 => "+4",
                ECardValue.ValueChangeColor=> "🔴🟡🟢🔵",
                ECardValue.ValueColorChanged => " ",
                _=>" "
            };
        }

        public GameCard Clone()
        {
            var clone = new GameCard();
            clone.CardColor = CardColor;
            clone.CardValue = CardValue;
            return clone;
        }
    }
}