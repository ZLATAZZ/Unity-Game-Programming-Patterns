using System;
using CursedForest.Core.FSM;
using CursedForest.Unity.Agents;
using CursedForest.Unity.Agents.Capabilities;
using CursedForest.Unity.Agents.Stats;
using CursedForest.Unity.Brain.Definitions;
using CursedForest.Unity.Brain.Runtime;
using CursedForest.Unity.Combat;
using CursedForest.Unity.Movement;
using CursedForest.Unity.Navigation;
using UnityEngine;
using Zenject;

namespace CursedForest.Unity.Composition
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(NavMeshAgentMovement))]
    public sealed class AgentInstaller : MonoInstaller
    {
        [Header("Brain")]
        [SerializeField] private BrainDefinition _brainDefinition;

        [Header("Required capabilities (on this agent)")]
        [SerializeField] private NavMeshAgentMovement _movement;
        [Tooltip("A component implementing IAgentPerception: NearestTargetPerception or DamageReactionPerception.")]
        [SerializeField] private MonoBehaviour _perception;

        [Header("Optional capabilities (on this agent)")]
        [SerializeField] private MeleeCombat _combat;
        [SerializeField] private NavMeshEscapePlanner _escapePlanner;

        [Header("Patrol points (scene objects, NOT prefab children)")]
        [SerializeField] private Transform[] _patrolWaypoints = Array.Empty<Transform>();

        [Header("Energy (enable for Monster's Sleep)")]
        [SerializeField] private bool _useEnergy;
        [SerializeField, Min(0.01f)] private float _maximumEnergy = 100f;
        [SerializeField, Min(0f)] private float _initialEnergy = 100f;
        [SerializeField, Min(0f)] private float _energyCostPerMeter = 2f;

        public override void InstallBindings()
        {
            if (_brainDefinition == null)
            {
                throw new InvalidOperationException($"Agent '{name}' has no BrainDefinition assigned.");
            }

            if (_movement == null || _perception == null)
            {
                throw new InvalidOperationException($"Agent '{name}' has no movement or perception component assigned.");
            }

            if (_movement.gameObject != gameObject || _perception.gameObject != gameObject)
            {
                throw new InvalidOperationException("Movement and perception must be components on this NPC root.");
            }

            IAgentPerception perception = _perception as IAgentPerception;

            if (perception == null)
            {
                throw new InvalidOperationException($"Agent '{name}': the Perception component must implement IAgentPerception.");
            }

            if ((_combat != null && _combat.gameObject != gameObject) ||
                (_escapePlanner != null && _escapePlanner.gameObject != gameObject))
            {
                throw new InvalidOperationException("Combat and escape planner must belong to this NPC root.");
            }

            Transform[] waypoints = _patrolWaypoints ?? Array.Empty<Transform>();
            Vector3[] patrolPositions = new Vector3[waypoints.Length];

            for (int i = 0; i < waypoints.Length; i++)
            {
                if (waypoints[i] == null)
                {
                    throw new InvalidOperationException($"Agent '{name}' has a missing patrol waypoint at index {i}.");
                }

                patrolPositions[i] = waypoints[i].position;
            }

            IAgentEnergy energy = _useEnergy ? new AgentEnergy(_maximumEnergy, _initialEnergy) : null;
            AgentContext agentContext = new AgentContext(_movement, perception);

            var dependencies = new AgentRuntimeDependencies(
                agentContext,
                _combat,
                _escapePlanner,
                energy,
                patrolPositions);

            BrainRuntime runtime = BrainBuilder.Build(_brainDefinition, dependencies);

            Container.Bind<IAgentMovement>().FromInstance(_movement);
            Container.Bind<IAgentPerception>().FromInstance(perception);
            Container.Bind<AgentContext>().FromInstance(agentContext);
            Container.Bind<StateMachine<AgentContext>>().FromInstance(runtime.StateMachine);
            Container.BindInstance(runtime.InitialStateId);

            if (_combat != null)
            {
                Container.Bind<IAgentCombat>().FromInstance(_combat);
            }

            if (_escapePlanner != null)
            {
                Container.Bind<IEscapePlanner>().FromInstance(_escapePlanner);
            }

            if (energy != null)
            {
                Container.Bind<IAgentEnergy>().FromInstance(energy);

                if (_energyCostPerMeter > 0f)
                {
                    Container.BindInterfacesTo<MovementEnergyDrain>()
                        .AsSingle()
                        .WithArguments(_energyCostPerMeter)
                        .NonLazy();
                }
            }

            Container.BindInterfacesTo<AgentBrain>().AsSingle().NonLazy();
        }
    }
}
