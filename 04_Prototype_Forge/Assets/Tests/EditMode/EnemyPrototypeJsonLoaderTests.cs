using NUnit.Framework;
using PrototypeForge.Core.Enemies;
using PrototypeForge.Unity.Data;

namespace PrototypeForge.Tests.EditMode
{
    public sealed class EnemyPrototypeJsonLoaderTests
    {
        [Test]
        public void Load_ValidJson_CreatesEnemyPrototype()
        {
            const string json = "{\"prototypes\":[{\"id\":\"test-enemy\",\"displayName\":\"Test Enemy\",\"maxHealth\":50,\"moveSpeed\":4.0,\"weapon\":{\"id\":\"test-weapon\",\"damage\":10,\"cooldown\":0.5}}]}";

            EnemyPrototypeJsonLoader loader = new();

            var prototypes = loader.Load(json);

            Assert.That(prototypes.Count, Is.EqualTo(1));

            EnemyPrototype prototype = prototypes[0];

            Assert.That(prototype.DisplayName, Is.EqualTo("Test Enemy"));
            Assert.That(prototype.MaxHealth, Is.EqualTo(50));
            Assert.That(prototype.Weapon.Damage, Is.EqualTo(10));
        }
    }
}