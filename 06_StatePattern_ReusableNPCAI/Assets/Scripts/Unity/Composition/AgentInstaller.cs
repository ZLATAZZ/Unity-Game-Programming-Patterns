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
using CursedForest.Unity.Perception;
using UnityEngine;
using Zenject;

namespace CursedForest.Unity.Composition
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(NavMeshAgentMovement))]
    [RequireComponent(typeof(NearestTargetPerception))]
    public sealed class AgentInstaller : MonoInstaller
    {
        [Header("Brain")]
        [SerializeField] private BrainDefinition _brainDefinition;

        [Header("Required agent components")]
        [SerializeField] private NavMeshAgentMovement _movement;
        [SerializeField] private NearestTargetPerception _perception;

        [Header("Optional capabilities")]
        [SerializeField] private MeleeCombat _combat;
        [SerializeField] private NavMeshEscapePlanner _escapePlanner;

        [Header("Patrol route (scene references; not stored in BrainDefinition)")]
        [SerializeField] private Transform[] _patrolWaypoints = Array.Empty<Transform>();

        [Header("Energy (only enable for agents that use it)")]
        [SerializeField] private bool _useEnergy;
        [SerializeField, Min(0.01f)] private float _maximumEnergy = 100f;
        [SerializeField, Min(0f)] private float _initialEnergy = 100f;

        public override void InstallBindings()
        {
            if (_brainDefinition == null)
            {
                throw new InvalidOperationException($"Agent '{name}' has no BrainDefinition assigned.");
            }

            if (_movement == null || _perception == null)
            {
                throw new InvalidOperationException($"Agent '{name}' is missing required movement or perception references.");
            }

            if (_movement.gameObject != gameObject || _perception.gameObject != gameObject)
            {
                throw new InvalidOperationException("Movement and perception must belong to the agent GameObject.");
            }

            if ((_combat != null && _combat.gameObject != gameObject) ||
                (_escapePlanner != null && _escapePlanner.gameObject != gameObject))
            {
                throw new InvalidOperationException("Optional capabilities must belong to the agent GameObject.");
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

            IAgentEnergy energy = null;

            if (_useEnergy)
            {
                energy = new AgentEnergy(_maximumEnergy, _initialEnergy);
            }

            var agentContext = new AgentContext(_movement, _perception);
            var dependencies = new AgentRuntimeDependencies(
                agentContext,
                _combat,
                _escapePlanner,
                energy,
                patrolPositions);

            BrainRuntime runtime = BrainBuilder.Build(_brainDefinition, dependencies);

            Container.Bind<IAgentMovement>().FromInstance(_movement);
            Container.Bind<IAgentPerception>().FromInstance(_perception);
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
            }

            Container.BindInterfacesTo<AgentBrain>().AsSingle().NonLazy();
        }
    }
}
