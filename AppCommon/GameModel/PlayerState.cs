using Microsoft.ServiceFabric.Actors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using static AppCommon.GameModel.PlayerState;

namespace AppCommon.GameModel
{
    [DataContract]
    public class PlayerState
    {
        [DataContract]
        public enum CharacterTypes
        {
            [EnumMember(Value = "Paladin")]
            Paladin,
            [EnumMember(Value = "Barbarian")]
            Barbarian,
            [EnumMember(Value = "Wizard")]
            Wizard,
            [EnumMember(Value = "Rogue")]
            Rogue
        }

        [DataMember]
        [JsonConverter(typeof(ActorIdSerializer))]
        public ActorId ActorId { get; set; }
        [DataMember]
        public int HealthPoints { get; set; }
        [DataMember]
        public bool IsBot { get; set; }
        [DataMember]
        public CharacterTypes CharacterType { get; set; }
        [DataMember]
        public Deck Deck { get; set; }
        [DataMember]
        public int Block {  get; set; }
        

        public PlayerState(ActorId actorId, bool isBot, List<CharacterTypes> usedCharacterTypes)
        {
            ActorId = actorId;
            HealthPoints = 10;
            IsBot = isBot;
            CharacterType = getRandomCharacter(usedCharacterTypes);
            Deck = new Deck(CharacterType);
            Deck.DealInitialHand();
            Block = 0;
        }
        private CharacterTypes getRandomCharacter(List<CharacterTypes> usedCharacterTypes)
        {
            List<CharacterTypes> values = new List<CharacterTypes>(Enum.GetValues(typeof(CharacterTypes)).Cast<CharacterTypes>());
            foreach (var item in usedCharacterTypes)
            {
                values.Remove(item);
            }
            Random random = new Random();
            return values[random.Next(values.Count)];
        }
        public bool IsDead()
        {
            return HealthPoints <= 0;
        }

        internal void Heal(int value)
        {
            HealthPoints = Math.Min(10, HealthPoints + value);
        }

        internal void Damage(int value)
        {
            if(Block > 0)
            {
                var oldBlock = Block;
                Block = Math.Max(Block - value, 0);
                value = Math.Max(value - oldBlock, 0);
            }
            HealthPoints -= value;
            if(IsDead())
            {
                Deck.InitZombieDeck();
            }
        }

        internal void AddBlock(int value)
        {
            Block += value;
        }
    }
}
