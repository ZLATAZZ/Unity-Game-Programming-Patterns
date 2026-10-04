using System;
using NUnit.Framework;
using PrototypeForge.Core.Enemies;
using PrototypeForge.Core.Prototypes;

namespace PrototypeForge.Tests.EditMode
{
    public sealed class PrototypeRegistryTests
    {
        [Test]
        public void Create_ReturnsNewCloneEachTime()
        {
            PrototypeRegistry<EnemyPrototype> registry = new();

            registry.Register(CreatePrototype());

            EnemyPrototype first = registry.Create(new PrototypeId("test-enemy"));
            EnemyPrototype second = registry.Create(new PrototypeId("test-enemy"));

            Assert.That(first, Is.Not.SameAs(second));
            Assert.That(first.Weapon, Is.Not.SameAs(second.Weapon));
        }

        [Test]
        public void Register_DuplicateId_Throws()
        {
            PrototypeRegistry<EnemyPrototype> registry = new();

            registry.Register(CreatePrototype());

            Assert.Throws<InvalidOperationException>(() => registry.Register(CreatePrototype()));
        }

        private static EnemyPrototype CreatePrototype()
        {
            WeaponData weapon = new(
                new PrototypeId("test-weapon"),
                10,
                0.5f);

            return new EnemyPrototype(
                new PrototypeId("test-enemy"),
                "Test Enemy",
                100,
                3f,
                weapon);
        }
    }
}