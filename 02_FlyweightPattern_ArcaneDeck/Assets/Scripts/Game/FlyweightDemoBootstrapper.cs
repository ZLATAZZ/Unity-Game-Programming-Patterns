using Patterns.Flyweight.Cards;
using UnityEngine;

namespace Patterns.Flyweight.Game
{
    public sealed class FlyweightDemoBootstrapper : MonoBehaviour
    {
        [SerializeField] private CardDefinition[] _cardDefinitions;

        private void Start()
        {
            CardDefinitionRegistry registry = new(_cardDefinitions);
            CardInstanceFactory factory = new(registry);

            CardInstance firstEmber = factory.Create(CardId.EmberBolt);
            CardInstance secondEmber = factory.Create(CardId.EmberBolt);
            CardInstance ward = factory.Create(CardId.ArcaneWard);

            firstEmber.TryUpgrade();

            bool emberDefinitionsAreShared = ReferenceEquals(
                firstEmber.Definition,
                secondEmber.Definition);

            bool differentCardTypesUseDifferentDefinitions = !ReferenceEquals(
                firstEmber.Definition,
                ward.Definition);

            Debug.Log(
                $"First Ember: Instance #{firstEmber.InstanceId}, " +
                $"Upgrade Level: {firstEmber.UpgradeLevel}, " +
                $"Effective Value: {firstEmber.EffectiveValue}");

            Debug.Log(
                $"Second Ember: Instance #{secondEmber.InstanceId}, " +
                $"Upgrade Level: {secondEmber.UpgradeLevel}, " +
                $"Effective Value: {secondEmber.EffectiveValue}");

            Debug.Log(
                $"First Ember Definition ID: {firstEmber.Definition.GetInstanceID()}");

            Debug.Log(
                $"Second Ember Definition ID: {secondEmber.Definition.GetInstanceID()}");

            Debug.Log(
                $"Ember definitions are shared: {emberDefinitionsAreShared}");

            Debug.Log(
                $"Ember and Ward use different definitions: {differentCardTypesUseDifferentDefinitions}");

            Debug.Assert(
                firstEmber.InstanceId != secondEmber.InstanceId,
                "Different card instances must have different IDs.");

            Debug.Assert(
                emberDefinitionsAreShared,
                "Instances of the same card type must share one CardDefinition.");

            Debug.Assert(
                differentCardTypesUseDifferentDefinitions,
                "Different card types should use different CardDefinitions.");

            Debug.Assert(
                firstEmber.UpgradeLevel == 1,
                "First Ember should have been upgraded.");

            Debug.Assert(
                secondEmber.UpgradeLevel == 0,
                "Upgrading one instance must not upgrade another instance.");
        }
    }
}