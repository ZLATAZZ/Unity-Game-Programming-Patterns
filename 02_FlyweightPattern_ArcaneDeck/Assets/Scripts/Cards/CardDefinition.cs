using UnityEngine;

namespace Patterns.Flyweight.Cards
{
    [CreateAssetMenu(fileName = "CardDefinition", menuName = "Patterns/Flyweight/Card Definition")]
    public sealed class CardDefinition : ScriptableObject
    {
        [SerializeField] private CardId _cardId;
        [SerializeField] private CardEffect _cardEffect;
        [SerializeField] private string _displayName;
        [SerializeField] private string _description;
        [SerializeField] private int _baseEffectValue;
        [SerializeField] private int _energyCost;
        [SerializeField] private int _upgradeValue;
        [SerializeField] private int _maxUpgradeLevel;
        [SerializeField] private Sprite _artwork;

        public CardId CardId => _cardId;
        public CardEffect CardEffect => _cardEffect;
        public string DisplayName => _displayName;
        public string Description => _description;
        public int BaseEffectValue => _baseEffectValue;
        public int EnergyCost => _energyCost;
        public int UpgradeValue => _upgradeValue;
        public int MaxUpgradeLevel => _maxUpgradeLevel;
        public Sprite Artwork => _artwork;
    }
}