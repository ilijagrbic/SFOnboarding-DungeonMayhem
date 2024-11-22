using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using static AppCommon.GameModel.CardEffect;

namespace AppCommon.GameModel
{
    [DataContract]
    public class Card
    {
        [DataMember]
        public required List<CardEffect> CardEffects { get; set; }
        [DataMember]
        public required string CardName { get; set; }
        public Card Clone()
        {
            return new Card { CardEffects = CardEffects.Select(s => s.Clone()).ToList(), CardName = CardName };
        }

        public bool HasEffect(CardEffectTypes cardEffect)
        {
            foreach (var effect in CardEffects)
            {
                if(effect.CardEffectType == cardEffect)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
