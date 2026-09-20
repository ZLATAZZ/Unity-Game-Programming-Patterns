using UnityEngine;

namespace Patterns.Flyweight.Enemy
{
    [CreateAssetMenu(fileName = "EnemyDefinition", menuName = "Enemy/Enemy Definition")]
    public sealed class EnemyDefinition : ScriptableObject
    {
        [SerializeField] private string _enemyName;
        [SerializeField] private Sprite _artwork;
        [SerializeField] private int _maxHealth;
        [SerializeField] private int _minAttackDamage;
        [SerializeField] private int _maxAttackDamage;

        public string EnemyName => _enemyName;
        public Sprite Artwork => _artwork;
        public int MaxHealth => _maxHealth;
        public int MinAttackDamage => _minAttackDamage;
        public int MaxAttackDamage => _maxAttackDamage;
    }
}