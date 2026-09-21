using UnityEngine;

namespace Patterns.Flyweight.Player
{
    [CreateAssetMenu(fileName = "PlayerDefinition", menuName = "Player/Player Definition")]
    public sealed class PlayerDefinition : ScriptableObject
    {
        [SerializeField] private string _playerName;
        [SerializeField] private Sprite _profileImage;
        [SerializeField] private int _maxHealth;
        [SerializeField] private int _maxEnergy;
        [SerializeField] private int _handCapacity;

        public string PlayerName => _playerName;
        public Sprite ProfileImage => _profileImage;
        public int MaxHealth => _maxHealth;
        public int MaxEnergy => _maxEnergy;
        public int HandCapacity => _handCapacity;
    }
}