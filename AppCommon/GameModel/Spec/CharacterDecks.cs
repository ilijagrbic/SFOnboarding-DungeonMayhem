using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AppCommon.GameModel.PlayerState;

namespace AppCommon.GameModel.Spec
{
    internal class CharacterDecks
    {
        private static Dictionary<CharacterTypes, List<Card>> characterDecks = FillCardSpec();

        private static Dictionary<CharacterTypes, List<Card>> FillCardSpec()
        {
            var result = new Dictionary<CharacterTypes, List<Card>>();

            List<Card> cards = new List<Card>();
            var forJustice = new Card { CardName = "forJustice",  CardEffects = new List<CardEffect> { new CardEffect { CardEffectType = CardEffect.CardEffectTypes.Damage, Value = 1 }, new CardEffect { CardEffectType = CardEffect.CardEffectTypes.ExtraCard, Value = 1 } } };
            cards.Add(forJustice);
            cards.Add(forJustice);
            cards.Add(forJustice);
            var forTheMostJustice = new Card { CardName = "forTheMostJustice", CardEffects = new List<CardEffect> { new CardEffect { CardEffectType = CardEffect.CardEffectTypes.Damage, Value = 3 } } };
            cards.Add(forTheMostJustice);
            cards.Add(forTheMostJustice);
            var divineSmite = new Card { CardName = "divineSmite", CardEffects = new List<CardEffect> { new CardEffect { CardEffectType = CardEffect.CardEffectTypes.Damage, Value = 3 }, new CardEffect { CardEffectType = CardEffect.CardEffectTypes.Heal, Value = 1 } } };
            cards.Add(divineSmite);
            cards.Add(divineSmite);
            cards.Add(divineSmite);
            var fightingWords = new Card { CardName = "fightingWords", CardEffects = new List<CardEffect> { new CardEffect { CardEffectType = CardEffect.CardEffectTypes.Damage, Value = 2 }, new CardEffect { CardEffectType = CardEffect.CardEffectTypes.Heal, Value = 1 } } };
            cards.Add(fightingWords);
            cards.Add(fightingWords);
            cards.Add(fightingWords);
            var forEvenMoreJustice = new Card { CardName = "forEvenMoreJustice", CardEffects = new List<CardEffect> { new CardEffect { CardEffectType = CardEffect.CardEffectTypes.Damage, Value = 2 } } };
            cards.Add(forEvenMoreJustice);
            cards.Add(forEvenMoreJustice);
            cards.Add(forEvenMoreJustice);
            cards.Add(forEvenMoreJustice);
            var highCharisma = new Card { CardName = "highCharisma", CardEffects = new List<CardEffect> { new CardEffect { CardEffectType = CardEffect.CardEffectTypes.Draw, Value = 2 } } };
            cards.Add(highCharisma);
            cards.Add(highCharisma);
            var cureWounds = new Card { CardName = "cureWounds", CardEffects = new List<CardEffect> { new CardEffect { CardEffectType = CardEffect.CardEffectTypes.Heal, Value = 1 }, new CardEffect { CardEffectType = CardEffect.CardEffectTypes.Draw, Value = 2 } } };
            cards.Add(cureWounds);
            var spiningParry = new Card { CardName = "spiningParry", CardEffects = new List<CardEffect> { new CardEffect { CardEffectType = CardEffect.CardEffectTypes.Block, Value = 1 }, new CardEffect { CardEffectType = CardEffect.CardEffectTypes.Draw, Value = 1 } } };
            cards.Add(spiningParry);
            cards.Add(spiningParry);
            var divineShield = new Card { CardName = "divineShield", CardEffects = new List<CardEffect> { new CardEffect { CardEffectType = CardEffect.CardEffectTypes.Block, Value = 3 } } };
            cards.Add(divineShield);
            cards.Add(divineShield);
            cards.Add(divineShield);
            // TODO add 1 banishing extra and destroy all block in game
            result[CharacterTypes.Paladin] = cards;

            cards = new List<Card>();
            var headButt = new Card { CardName = "headBut", CardEffects = new List<CardEffect> { new CardEffect { CardEffectType = CardEffect.CardEffectTypes.Damage, Value = 1 }, new CardEffect { CardEffectType = CardEffect.CardEffectTypes.ExtraCard, Value = 1 } } };
            cards.Add(headButt);
            cards.Add(headButt);
            var bigAxe = new Card { CardName = "bigAxe", CardEffects = new List<CardEffect> { new CardEffect { CardEffectType = CardEffect.CardEffectTypes.Damage, Value = 3 } } };
            cards.Add(bigAxe);
            cards.Add(bigAxe);
            cards.Add(bigAxe);
            cards.Add(bigAxe);
            cards.Add(bigAxe);
            var rage = new Card { CardName = "rage", CardEffects = new List<CardEffect> { new CardEffect { CardEffectType = CardEffect.CardEffectTypes.Damage, Value = 4 } } };
            cards.Add(rage);
            cards.Add(rage);
            var punch = new Card { CardName = "punch", CardEffects = new List<CardEffect> { new CardEffect { CardEffectType = CardEffect.CardEffectTypes.Damage, Value = 2 } } };
            cards.Add(punch);
            cards.Add(punch);
            var armory = new Card { CardName = "armory", CardEffects = new List<CardEffect> { new CardEffect { CardEffectType = CardEffect.CardEffectTypes.Draw, Value = 2 } } };
            cards.Add(armory);
            cards.Add(armory);
            var snackTime = new Card { CardName = "snackTime", CardEffects = new List<CardEffect> { new CardEffect { CardEffectType = CardEffect.CardEffectTypes.Heal, Value = 1 }, new CardEffect { CardEffectType = CardEffect.CardEffectTypes.Draw, Value = 2 } } };
            cards.Add(snackTime);
            var flex = new Card { CardName = "flex", CardEffects = new List<CardEffect> { new CardEffect { CardEffectType = CardEffect.CardEffectTypes.Heal, Value = 1 }, new CardEffect { CardEffectType = CardEffect.CardEffectTypes.Draw, Value = 1 } } };
            cards.Add(flex);
            cards.Add(flex);
            var wolf = new Card { CardName = "wolf", CardEffects = new List<CardEffect> { new CardEffect { CardEffectType = CardEffect.CardEffectTypes.Block, Value = 3 }} };
            cards.Add(wolf);
            cards.Add(wolf);
            cards.Add(wolf);
            var bag = new Card { CardName = "bag", CardEffects = new List<CardEffect> { new CardEffect { CardEffectType = CardEffect.CardEffectTypes.Block, Value = 1 }, new CardEffect { CardEffectType = CardEffect.CardEffectTypes.Draw, Value = 1 } } };
            cards.Add(bag);
            // TODO add 2 whirlwind 1 dmg each heal 1 for each player
            result[CharacterTypes.Barbarian] = cards;

            cards = new List<Card>();
            var evilSmile = new Card { CardName = "evilSmile", CardEffects = new List<CardEffect> { new CardEffect { CardEffectType = CardEffect.CardEffectTypes.Heal, Value = 1 }, new CardEffect { CardEffectType = CardEffect.CardEffectTypes.ExtraCard, Value = 1 } } };
            cards.Add(evilSmile);
            cards.Add(evilSmile);
            var magicMissle = new Card { CardName = "magicMissle", CardEffects = new List<CardEffect> { new CardEffect { CardEffectType = CardEffect.CardEffectTypes.Damage, Value = 1 }, new CardEffect { CardEffectType = CardEffect.CardEffectTypes.ExtraCard, Value = 1 } } };
            cards.Add(magicMissle);
            cards.Add(magicMissle);
            var lightningBolt = new Card { CardName = "lightningBolt", CardEffects = new List<CardEffect> { new CardEffect { CardEffectType = CardEffect.CardEffectTypes.Damage, Value = 3 }} };
            cards.Add(lightningBolt);
            cards.Add(lightningBolt);
            var burningHands = new Card { CardName = "burningHands", CardEffects = new List<CardEffect> { new CardEffect { CardEffectType = CardEffect.CardEffectTypes.Damage, Value = 2 } } };
            cards.Add(burningHands);
            cards.Add(burningHands);
            cards.Add(burningHands);
            var shield = new Card { CardName = "shield", CardEffects = new List<CardEffect> { new CardEffect { CardEffectType = CardEffect.CardEffectTypes.Block, Value = 1 }, new CardEffect { CardEffectType = CardEffect.CardEffectTypes.Draw, Value = 1 } } };
            cards.Add(shield);
            cards.Add(shield);
            var knowledge = new Card { CardName = "knowledgeIsPower", CardEffects = new List<CardEffect> {  new CardEffect { CardEffectType = CardEffect.CardEffectTypes.Draw, Value = 3 } } };
            cards.Add(knowledge);
            cards.Add(knowledge);
            var mirrorImage = new Card { CardName = "mirrorImage", CardEffects = new List<CardEffect> { new CardEffect { CardEffectType = CardEffect.CardEffectTypes.Block, Value = 3 } } };
            cards.Add(mirrorImage);
            cards.Add(mirrorImage);
            // TODO add 2 fireball 3 dmg each
            result[CharacterTypes.Wizard] = cards;

            cards = new List<Card>();
            var booWhatDoWeDo = new Card { CardName = "booWhatDoWeDo", CardEffects = new List<CardEffect> { new CardEffect { CardEffectType = CardEffect.CardEffectTypes.Draw, Value = 2 } } };
            cards.Add(booWhatDoWeDo);
            cards.Add(booWhatDoWeDo);
            var wrapItUp = new Card { CardName = "wrapItUp", CardEffects = new List<CardEffect> { new CardEffect { CardEffectType = CardEffect.CardEffectTypes.Heal, Value = 1 }, new CardEffect { CardEffectType = CardEffect.CardEffectTypes.Draw, Value = 1 } } };
            cards.Add(wrapItUp); 
            var palePriestess = new Card { CardName = "palePriestess", CardEffects = new List<CardEffect> { new CardEffect { CardEffectType = CardEffect.CardEffectTypes.Block, Value = 1 }, new CardEffect { CardEffectType = CardEffect.CardEffectTypes.Heal, Value = 1 } } };
            cards.Add(palePriestess);
            cards.Add(palePriestess);
            var goForTheEyes = new Card { CardName = "goForTheEyes", CardEffects = new List<CardEffect> { new CardEffect { CardEffectType = CardEffect.CardEffectTypes.Damage, Value = 3 } } };
            cards.Add(goForTheEyes);
            cards.Add(goForTheEyes);
            cards.Add(goForTheEyes);
            var twiceTheSting = new Card { CardName = "twiceTheSting", CardEffects = new List<CardEffect> { new CardEffect { CardEffectType = CardEffect.CardEffectTypes.Damage, Value = 2 } } };
            cards.Add(twiceTheSting);
            cards.Add(twiceTheSting);
            cards.Add(twiceTheSting);
            var sneakyWheel = new Card { CardName = "sneakyWheel", CardEffects = new List<CardEffect> { new CardEffect { CardEffectType = CardEffect.CardEffectTypes.Damage, Value = 1 }, new CardEffect { CardEffectType = CardEffect.CardEffectTypes.ExtraCard, Value = 1 } } };
            cards.Add(sneakyWheel);
            cards.Add(sneakyWheel);
            cards.Add(sneakyWheel);
            var punchEvil = new Card { CardName = "punchEvil", CardEffects = new List<CardEffect> { new CardEffect { CardEffectType = CardEffect.CardEffectTypes.Damage, Value = 2 }, new CardEffect { CardEffectType = CardEffect.CardEffectTypes.ExtraCard, Value = 1 } } };
            cards.Add(punchEvil);
            var holdMyRodent = new Card { CardName = "holdMyRodent", CardEffects = new List<CardEffect> { new CardEffect { CardEffectType = CardEffect.CardEffectTypes.Damage, Value = 3 }, new CardEffect { CardEffectType = CardEffect.CardEffectTypes.Heal, Value = 1 } } };
            cards.Add(holdMyRodent);
            cards.Add(holdMyRodent);
            var mount = new Card { CardName = "mount", CardEffects = new List<CardEffect> { new CardEffect { CardEffectType = CardEffect.CardEffectTypes.Block, Value = 3 } } };
            cards.Add(mount);
            cards.Add(mount);
            cards.Add(mount);
            // TODO add 2 scout draw 1 from each oponent
            result[CharacterTypes.Rogue] = cards;

            return result;
        }


        public static List<Card> GetCardsByCharacterType(CharacterTypes characterType)
        {
            return characterDecks.ContainsKey(characterType) ? characterDecks[characterType].Select(s => s.Clone()).ToList(): [];
        }

    }
}
