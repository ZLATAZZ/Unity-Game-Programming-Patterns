using System;
using CursedForest.Core.FSM;
using CursedForest.Unity.Agents;
using CursedForest.Unity.Agents.Capabilities;
using CursedForest.Unity.Behaviors;
using CursedForest.Unity.Movement;
using CursedForest.Unity.Perception;
using UnityEngine;
using Zenject;

namespace CursedForest.Unity.Composition
{
    [DisallowMultipleComponent]
    public sealed class AgentInstaller : MonoInstaller
    {
        [SerializeField] private NavMeshAgentMovement _movement;
        [SerializeField] private NearestTargetPerception _perception;
        [SerializeField] private Transform[] _patrolWaypoints;
        [SerializeField] private string _patrolStateId = "patrol";

        public override void InstallBindings()
        {
            if (_movement == null)
            {
                throw new InvalidOperationException("Agent movement is not assigned.");
            }

            if (_perception == null)
            {
                throw new InvalidOperationException("Agent perception is not assigned.");
            }

            if (_patrolWaypoints == null || _patrolWaypoints.Length == 0)
            {
                throw new InvalidOperationException("At least one patrol waypoint is required.");
            }

            StateId patrolId = new StateId(_patrolStateId);
            Vector3[] positions = new Vector3[_patrolWaypoints.Length];

            for (int i = 0; i < _patrolWaypoints.Length; i++)
            {
                if (_patrolWaypoints[i] == null)
                {
                    throw new InvalidOperationException($"Patrol waypoint at index {i} is missing.");
                }

                positions[i] = _patrolWaypoints[i].position;
            }

            Container.Bind<IAgentMovement>().FromInstance(_movement);
            Container.Bind<IAgentPerception>().FromInstance(_perception);
            Container.Bind<AgentContext>().AsSingle();
            Container.BindInstance(patrolId);

            Container.Bind<StateMachine<AgentContext>>().FromMethod(injectContext =>
            {
                AgentContext agentContext = injectContext.Container.Resolve<AgentContext>();
                StateMachine<AgentContext> machine = new StateMachine<AgentContext>(agentContext);

                machine.RegisterState(new PatrolState(patrolId, positions));

                return machine;
            }).AsSingle();

            Container.BindInterfacesTo<AgentBrain>().AsSingle().NonLazy();
        }
    }
}
