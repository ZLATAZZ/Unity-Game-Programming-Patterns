using System;

namespace PrototypeForge.Unity.Data
{
    [Serializable]
    internal sealed class EnemyPrototypeCollectionDto
    {
        public EnemyPrototypeDto[] prototypes;
    }

    [Serializable]
    internal sealed class EnemyPrototypeDto
    {
        public string id;
        public string displayName;
        public int maxHealth;
        public float moveSpeed;
        public WeaponDataDto weapon;
    }

    [Serializable]
    internal sealed class WeaponDataDto
    {
        public string id;
        public int damage;
        public float cooldown;
    }
}