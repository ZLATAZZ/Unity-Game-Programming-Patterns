using NUnit.Framework;
using PrototypeForge.Core.Enemies;
using PrototypeForge.Core.Prototypes;

namespace PrototypeForge.Tests.EditMode
{
    public sealed class EnemyPrototypeTests
    {
        [Test]
        public void Clone_ReturnsDifferentEnemyInstance()
        {
            EnemyPrototype original = CreatePrototype();

            EnemyPrototype clone = original.Clone();

            Assert.That(clone, Is.Not.SameAs(original));
        }

        [Test]
        public void Clone_PreservesValues()
        {
            EnemyPrototype original = CreatePrototype();

            EnemyPrototype clone = original.Clone();

            Assert.That(clone.Id, Is.EqualTo(original.Id));
            Assert.That(clone.DisplayName, Is.EqualTo(original.DisplayName));
            Assert.That(clone.MaxHealth, Is.EqualTo(original.MaxHealth));
            Assert.That(clone.MoveSpeed, Is.EqualTo(original.MoveSpeed));
            Assert.That(clone.Weapon.Damage, Is.EqualTo(original.Weapon.Damage));
            Assert.That(clone.Weapon.Cooldown, Is.EqualTo(original.Weapon.Cooldown));
        }

        [Test]
        public void Clone_CreatesIndependentWeaponInstance()
        {
            EnemyPrototype original = CreatePrototype();

            EnemyPrototype clone = original.Clone();

            clone.Weapon.IncreaseDamage(20);

            Assert.That(clone.Weapon, Is.Not.SameAs(original.Weapon));
            Assert.That(original.Weapon.Damage, Is.EqualTo(8));
            Assert.That(clone.Weapon.Damage, Is.EqualTo(28));
        }

        private static EnemyPrototype CreatePrototype()
        {
            WeaponData weapon = new(
                new PrototypeId("test-weapon"),
                8,
                0.3f);

            return new EnemyPrototype(
                new PrototypeId("test-enemy"),
                "Test Enemy",
                40,
                6f,
                weapon);
        }
    }
}