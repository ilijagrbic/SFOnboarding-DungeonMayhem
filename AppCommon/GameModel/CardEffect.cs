using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace AppCommon.GameModel
{
    [DataContract]
    public class CardEffect
    {
        [DataContract]
        public enum CardEffectTypes
        {
            [EnumMember(Value = "Block")]
            Block,
            [EnumMember(Value = "Draw")]
            Draw,
            [EnumMember(Value = "Damage")]
            Damage,
            [EnumMember(Value = "ExtraCard")]
            ExtraCard,
            [EnumMember(Value = "Heal")]
            Heal,
            // TODO add other effects
        }

        [DataMember]
        public CardEffectTypes CardEffectType { get; set; }
        [DataMember]
        public int Value { get; set; }

        public CardEffect Clone()
        {
            return new CardEffect { CardEffectType = CardEffectType, Value = Value };
        }
    }
}
