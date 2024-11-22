using AppCommon.GameModel.Spec;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using static AppCommon.GameModel.PlayerState;

namespace AppCommon.GameModel
{
    [DataContract]
    public class Deck(PlayerState.CharacterTypes characterType)
    {
        [DataMember]
        public List<Card> DrawPile { get; set; } = CharacterDecks.GetCardsByCharacterType(characterType);
        [DataMember]
        public List<Card> DiscardPile { get; set; } = new List<Card>();
        [DataMember]
        public List<Card> Hand { get; set; } = new List<Card>();
        [DataMember]
        public List<Card> InPlay { get; set; } = new List<Card>();

        public void DealInitialHand() 
        {
            for(int i = 0; i < 5; i++)
            {
                Hand.Add(DrawPile[0]);
                DrawPile.RemoveAt(0);
            }
        }
        public void DrawCards(int amount)
        {
            for(int  i = 0; i < amount; i++)
            {
                if (DrawPile.Count > 0)
                {
                    Hand.Add(DrawPile[0]);
                    DrawPile.RemoveAt(0);
                }
                else
                {
                    MoveDiscardToDraw();
                    ShuffleDeck();
                    Hand.Add(DrawPile[0]);
                    DrawPile.RemoveAt(0);
                }
            }
        }
        public void ShuffleDeck() 
        { 
            DrawPile = [.. DrawPile.OrderBy(x => Guid.NewGuid())];
        }
        public void MoveDiscardToDraw() 
        {
            DrawPile.AddRange(DiscardPile);
            DiscardPile.Clear();
        }
        public Card GetCard(int index) 
        {  
            return Hand[index];
        }
        public void PlayCard(int index)
        {
            DiscardPile.Add(Hand[index]);
            Hand.RemoveAt(index);
        }
        public void PlaceInPlay(int index)
        {
            InPlay.Add(Hand[index]);
            Hand.RemoveAt(index);
        }
        public void ResolveDamage(int index)
        {
            throw new NotImplementedException();
        }
        public void DestroyAllInPlay()
        {
            for (int i = 0; i < InPlay.Count; i++)
            {
                DiscardPile.Add(InPlay[0]);
                InPlay.RemoveAt(0);
            }
        }

        internal void InitZombieDeck()
        {
            DrawPile = new List<Card>();
            DiscardPile = new List<Card>();
            Hand = new List<Card>();
            InPlay = new List<Card>();

            Hand.Add(new Card {CardName = "ghostTouch",  CardEffects = new List<CardEffect> { new CardEffect { CardEffectType=CardEffect.CardEffectTypes.Damage, Value = 1 } } });
        }
    }
}
