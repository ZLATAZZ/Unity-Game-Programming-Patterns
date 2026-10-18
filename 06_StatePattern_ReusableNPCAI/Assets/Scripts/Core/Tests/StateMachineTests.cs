
using System;
using System.Collections.Generic;
using CursedForest.Core.FSM;
using NUnit.Framework;

namespace CursedForest.Core.Tests
{
    public sealed class StateMachineTests
    {
        private sealed class TestContext
        {
            public bool CanChase { get; set; }
            public bool CanSleep { get; set; }
        }

        private sealed class FakeState : IState<TestContext>
        {
            private readonly List<string> _events;

            public StateId Id { get; }

            public int EnterCount { get; private set; }
            public int TickCount { get; private set; }
            public int ExitCount { get; private set; }

            public Action<TestContext> OnEnter { get; set; }
            public Action<TestContext, float> OnTick { get; set; }
            public Action<TestContext> OnExit { get; set; }

            public FakeState(string id, List<string> events = null)
                : this(new StateId(id), events)
            {
            }

            public FakeState(StateId id, List<string> events = null)
            {
                Id = id;
                _events = events;
            }

            public void Enter(TestContext context)
            {
                EnterCount++;
                _events?.Add($"{Id}.Enter");
                OnEnter?.Invoke(context);
            }

            public void Tick(TestContext context, float deltaTime)
            {
                TickCount++;
                _events?.Add($"{Id}.Tick");
                OnTick?.Invoke(context, deltaTime);
            }

            public void Exit(TestContext context)
            {
                ExitCount++;
                _events?.Add($"{Id}.Exit");
                OnExit?.Invoke(context);
            }
        }

        private static StateMachine<TestContext> CreateMachine(TestContext context, params FakeState[] states)
        {
            StateMachine<TestContext> machine = new StateMachine<TestContext>(context);

            foreach (FakeState state in states)
            {
                machine.RegisterState(state);
            }

            return machine;
        }

        [Test]
        public void Constructor_NullContext_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new StateMachine<TestContext>(null));
        }

        [Test]
        public void RegisterState_NullState_Throws()
        {
            StateMachine<TestContext> machine = CreateMachine(new TestContext());

            Assert.Throws<ArgumentNullException>(() => machine.RegisterState(null));
        }

        [Test]
        public void RegisterState_EmptyId_Throws()
        {
            StateMachine<TestContext> machine = CreateMachine(new TestContext());
            FakeState invalidState = new FakeState(default(StateId));

            Assert.Throws<ArgumentException>(() => machine.RegisterState(invalidState));
        }

        [Test]
        public void RegisterState_DuplicateId_Throws()
        {
            FakeState patrol = new FakeState("patrol");
            FakeState duplicate = new FakeState("patrol");
            StateMachine<TestContext> machine = CreateMachine(new TestContext(), patrol);

            Assert.Throws<ArgumentException>(() => machine.RegisterState(duplicate));
        }

        [Test]
        public void AddTransition_NullTransition_Throws()
        {
            StateMachine<TestContext> machine = CreateMachine(new TestContext());

            Assert.Throws<ArgumentNullException>(() => machine.AddTransition(null));
        }

        [Test]
        public void AddTransition_UnregisteredSource_Throws()
        {
            FakeState chase = new FakeState("chase");
            StateMachine<TestContext> machine = CreateMachine(new TestContext(), chase);

            StateTransition<TestContext> transition = new StateTransition<TestContext>(
                new StateId("patrol"), chase.Id, 100, context => true);

            Assert.Throws<ArgumentException>(() => machine.AddTransition(transition));
        }

        [Test]
        public void AddTransition_UnregisteredTarget_Throws()
        {
            FakeState patrol = new FakeState("patrol");
            StateMachine<TestContext> machine = CreateMachine(new TestContext(), patrol);

            StateTransition<TestContext> transition = new StateTransition<TestContext>(
                patrol.Id, new StateId("chase"), 100, context => true);

            Assert.Throws<ArgumentException>(() => machine.AddTransition(transition));
        }

        [Test]
        public void CurrentStateId_BeforeStart_Throws()
        {
            StateMachine<TestContext> machine = CreateMachine(new TestContext());

            Assert.Throws<InvalidOperationException>(() =>
            {
                StateId currentStateId = machine.CurrentStateId;
            });
        }

        [Test]
        public void Start_RegisteredState_CallsEnterOnce()
        {
            FakeState patrol = new FakeState("patrol");
            StateMachine<TestContext> machine = CreateMachine(new TestContext(), patrol);

            machine.Start(patrol.Id);

            Assert.That(patrol.EnterCount, Is.EqualTo(1));
            Assert.That(patrol.TickCount, Is.Zero);
            Assert.That(machine.CurrentStateId, Is.EqualTo(patrol.Id));
        }

        [Test]
        public void Start_EmptyId_Throws()
        {
            StateMachine<TestContext> machine = CreateMachine(new TestContext());

            Assert.Throws<ArgumentException>(() => machine.Start(default(StateId)));
        }

        [Test]
        public void Start_UnregisteredState_Throws()
        {
            StateMachine<TestContext> machine = CreateMachine(new TestContext());

            Assert.Throws<ArgumentException>(() => machine.Start(new StateId("unknown")));
        }

        [Test]
        public void Start_CalledTwice_Throws()
        {
            FakeState patrol = new FakeState("patrol");
            StateMachine<TestContext> machine = CreateMachine(new TestContext(), patrol);

            machine.Start(patrol.Id);

            Assert.Throws<InvalidOperationException>(() => machine.Start(patrol.Id));
            Assert.That(patrol.EnterCount, Is.EqualTo(1));
        }

        [Test]
        public void RegisterState_AfterStart_Throws()
        {
            FakeState patrol = new FakeState("patrol");
            StateMachine<TestContext> machine = CreateMachine(new TestContext(), patrol);

            machine.Start(patrol.Id);

            Assert.Throws<InvalidOperationException>(() => machine.RegisterState(new FakeState("chase")));
        }

        [Test]
        public void AddTransition_AfterStart_Throws()
        {
            FakeState patrol = new FakeState("patrol");
            FakeState chase = new FakeState("chase");
            StateMachine<TestContext> machine = CreateMachine(new TestContext(), patrol, chase);

            machine.Start(patrol.Id);

            StateTransition<TestContext> transition = new StateTransition<TestContext>(
                patrol.Id, chase.Id, 100, context => true);

            Assert.Throws<InvalidOperationException>(() => machine.AddTransition(transition));
        }

        [Test]
        public void Tick_BeforeStart_Throws()
        {
            StateMachine<TestContext> machine = CreateMachine(new TestContext());

            Assert.Throws<InvalidOperationException>(() => machine.Tick(0.016f));
        }

        [Test]
        public void Tick_NoTransitions_TicksCurrentState()
        {
            FakeState patrol = new FakeState("patrol");
            StateMachine<TestContext> machine = CreateMachine(new TestContext(), patrol);

            machine.Start(patrol.Id);
            machine.Tick(0.016f);

            Assert.That(patrol.TickCount, Is.EqualTo(1));
            Assert.That(patrol.ExitCount, Is.Zero);
            Assert.That(machine.CurrentStateId, Is.EqualTo(patrol.Id));
        }

        [Test]
        public void Tick_ConditionFalse_RemainsInCurrentState()
        {
            TestContext context = new TestContext { CanChase = false };
            FakeState patrol = new FakeState("patrol");
            FakeState chase = new FakeState("chase");
            StateMachine<TestContext> machine = CreateMachine(context, patrol, chase);

            machine.AddTransition(new StateTransition<TestContext>(
                patrol.Id, chase.Id, 100, agent => agent.CanChase));

            machine.Start(patrol.Id);
            machine.Tick(0.016f);

            Assert.That(machine.CurrentStateId, Is.EqualTo(patrol.Id));
            Assert.That(patrol.TickCount, Is.EqualTo(1));
            Assert.That(patrol.ExitCount, Is.Zero);
            Assert.That(chase.EnterCount, Is.Zero);
        }

        [Test]
        public void Tick_ValidTransition_CallsLifecycleInCorrectOrder()
        {
            List<string> events = new List<string>();
            FakeState patrol = new FakeState("patrol", events);
            FakeState chase = new FakeState("chase", events);
            StateMachine<TestContext> machine = CreateMachine(new TestContext(), patrol, chase);

            machine.AddTransition(new StateTransition<TestContext>(
                patrol.Id, chase.Id, 100, context => true));

            machine.Start(patrol.Id);
            machine.Tick(0.016f);

            Assert.That(events, Is.EqualTo(new[]
            {
                "patrol.Enter",
                "patrol.Exit",
                "chase.Enter",
                "chase.Tick"
            }));

            Assert.That(machine.CurrentStateId, Is.EqualTo(chase.Id));
            Assert.That(patrol.TickCount, Is.Zero);
        }

        [Test]
        public void Tick_MultipleValidTransitions_SelectsHighestPriority()
        {
            FakeState patrol = new FakeState("patrol");
            FakeState chase = new FakeState("chase");
            FakeState sleep = new FakeState("sleep");
            StateMachine<TestContext> machine = CreateMachine(new TestContext(), patrol, chase, sleep);

            machine.AddTransition(new StateTransition<TestContext>(
                patrol.Id, chase.Id, 50, context => true));

            machine.AddTransition(new StateTransition<TestContext>(
                patrol.Id, sleep.Id, 100, context => true));

            machine.Start(patrol.Id);
            machine.Tick(0.016f);

            Assert.That(machine.CurrentStateId, Is.EqualTo(sleep.Id));
            Assert.That(chase.EnterCount, Is.Zero);
            Assert.That(sleep.EnterCount, Is.EqualTo(1));
        }

        [Test]
        public void Tick_HigherPriorityConditionFalse_SelectsNextValidTransition()
        {
            TestContext context = new TestContext
            {
                CanChase = true,
                CanSleep = false
            };

            FakeState patrol = new FakeState("patrol");
            FakeState chase = new FakeState("chase");
            FakeState sleep = new FakeState("sleep");
            StateMachine<TestContext> machine = CreateMachine(context, patrol, chase, sleep);

            machine.AddTransition(new StateTransition<TestContext>(
                patrol.Id, sleep.Id, 100, agent => agent.CanSleep));

            machine.AddTransition(new StateTransition<TestContext>(
                patrol.Id, chase.Id, 50, agent => agent.CanChase));

            machine.Start(patrol.Id);
            machine.Tick(0.016f);

            Assert.That(machine.CurrentStateId, Is.EqualTo(chase.Id));
            Assert.That(sleep.EnterCount, Is.Zero);
        }

        [Test]
        public void Tick_EqualPriorities_SelectsFirstRegisteredTransition()
        {
            FakeState patrol = new FakeState("patrol");
            FakeState chase = new FakeState("chase");
            FakeState sleep = new FakeState("sleep");
            StateMachine<TestContext> machine = CreateMachine(new TestContext(), patrol, chase, sleep);

            machine.AddTransition(new StateTransition<TestContext>(
                patrol.Id, chase.Id, 100, context => true));

            machine.AddTransition(new StateTransition<TestContext>(
                patrol.Id, sleep.Id, 100, context => true));

            machine.Start(patrol.Id);
            machine.Tick(0.016f);

            Assert.That(machine.CurrentStateId, Is.EqualTo(chase.Id));
            Assert.That(sleep.EnterCount, Is.Zero);
        }

        [Test]
        public void Tick_PerformsOnlyOneTransitionPerCall()
        {
            FakeState patrol = new FakeState("patrol");
            FakeState chase = new FakeState("chase");
            FakeState attack = new FakeState("attack");
            StateMachine<TestContext> machine = CreateMachine(new TestContext(), patrol, chase, attack);

            machine.AddTransition(new StateTransition<TestContext>(
                patrol.Id, chase.Id, 100, context => true));

            machine.AddTransition(new StateTransition<TestContext>(
                chase.Id, attack.Id, 100, context => true));

            machine.Start(patrol.Id);
            machine.Tick(0.016f);

            Assert.That(machine.CurrentStateId, Is.EqualTo(chase.Id));
            Assert.That(attack.EnterCount, Is.Zero);

            machine.Tick(0.016f);

            Assert.That(machine.CurrentStateId, Is.EqualTo(attack.Id));
            Assert.That(attack.EnterCount, Is.EqualTo(1));
        }

        [Test]
        public void Tick_SelfTransition_ReentersCurrentState()
        {
            FakeState patrol = new FakeState("patrol");
            StateMachine<TestContext> machine = CreateMachine(new TestContext(), patrol);

            machine.AddTransition(new StateTransition<TestContext>(
                patrol.Id, patrol.Id, 100, context => true));

            machine.Start(patrol.Id);
            machine.Tick(0.016f);

            Assert.That(patrol.EnterCount, Is.EqualTo(2));
            Assert.That(patrol.ExitCount, Is.EqualTo(1));
            Assert.That(patrol.TickCount, Is.EqualTo(1));
        }

        [Test]
        public void Tick_InvalidDeltaTime_Throws()
        {
            FakeState patrol = new FakeState("patrol");
            StateMachine<TestContext> machine = CreateMachine(new TestContext(), patrol);

            machine.Start(patrol.Id);

            float[] invalidValues =
            {
                -1f,
                float.NaN,
                float.PositiveInfinity,
                float.NegativeInfinity
            };

            foreach (float deltaTime in invalidValues)
            {
                Assert.Throws<ArgumentOutOfRangeException>(() => machine.Tick(deltaTime));
            }

            Assert.That(patrol.TickCount, Is.Zero);
        }

        [Test]
        public void Tick_ZeroDeltaTime_IsAllowed()
        {
            FakeState patrol = new FakeState("patrol");
            StateMachine<TestContext> machine = CreateMachine(new TestContext(), patrol);

            machine.Start(patrol.Id);
            machine.Tick(0f);

            Assert.That(patrol.TickCount, Is.EqualTo(1));
        }

        [Test]
        public void Start_EnterThrows_PreventsFurtherExecution()
        {
            FakeState patrol = new FakeState("patrol");
            patrol.OnEnter = context => throw new ApplicationException("Enter failed.");

            StateMachine<TestContext> machine = CreateMachine(new TestContext(), patrol);

            Assert.Throws<ApplicationException>(() => machine.Start(patrol.Id));
            Assert.Throws<InvalidOperationException>(() => machine.Tick(0.016f));
            Assert.Throws<InvalidOperationException>(() => machine.Start(patrol.Id));
        }

        [Test]
        public void Tick_ConditionThrows_MarksMachineAsFaulted()
        {
            FakeState patrol = new FakeState("patrol");
            FakeState chase = new FakeState("chase");
            StateMachine<TestContext> machine = CreateMachine(new TestContext(), patrol, chase);

            machine.AddTransition(new StateTransition<TestContext>(
                patrol.Id, chase.Id, 100, context => throw new ApplicationException("Condition failed.")));

            machine.Start(patrol.Id);

            Assert.Throws<ApplicationException>(() => machine.Tick(0.016f));
            Assert.Throws<InvalidOperationException>(() => machine.Tick(0.016f));
        }

        [Test]
        public void Tick_StateTickThrows_MarksMachineAsFaulted()
        {
            FakeState patrol = new FakeState("patrol");
            patrol.OnTick = (context, deltaTime) => throw new ApplicationException("Tick failed.");

            StateMachine<TestContext> machine = CreateMachine(new TestContext(), patrol);

            machine.Start(patrol.Id);

            Assert.Throws<ApplicationException>(() => machine.Tick(0.016f));
            Assert.Throws<InvalidOperationException>(() => machine.Tick(0.016f));
        }

        [Test]
        public void Tick_ExitThrows_MarksMachineAsFaulted()
        {
            FakeState patrol = new FakeState("patrol");
            FakeState chase = new FakeState("chase");
            patrol.OnExit = context => throw new ApplicationException("Exit failed.");

            StateMachine<TestContext> machine = CreateMachine(new TestContext(), patrol, chase);

            machine.AddTransition(new StateTransition<TestContext>(
                patrol.Id, chase.Id, 100, context => true));

            machine.Start(patrol.Id);

            Assert.Throws<ApplicationException>(() => machine.Tick(0.016f));
            Assert.Throws<InvalidOperationException>(() => machine.Tick(0.016f));
        }

        [Test]
        public void Tick_NextStateEnterThrows_MarksMachineAsFaulted()
        {
            FakeState patrol = new FakeState("patrol");
            FakeState chase = new FakeState("chase");
            chase.OnEnter = context => throw new ApplicationException("Chase.Enter failed.");

            StateMachine<TestContext> machine = CreateMachine(new TestContext(), patrol, chase);

            machine.AddTransition(new StateTransition<TestContext>(
                patrol.Id, chase.Id, 100, context => true));

            machine.Start(patrol.Id);

            Assert.Throws<ApplicationException>(() => machine.Tick(0.016f));
            Assert.Throws<InvalidOperationException>(() => machine.Tick(0.016f));
        }

        [Test]
        public void Tick_ReentrantCall_ThrowsAndMarksMachineAsFaulted()
        {
            FakeState patrol = new FakeState("patrol");
            StateMachine<TestContext> machine = CreateMachine(new TestContext(), patrol);

            patrol.OnTick = (context, deltaTime) => machine.Tick(deltaTime);

            machine.Start(patrol.Id);

            Assert.Throws<InvalidOperationException>(() => machine.Tick(0.016f));
            Assert.Throws<InvalidOperationException>(() => machine.Tick(0.016f));
        }

        [Test]
        public void TwoMachines_UseIndependentContextsAndStates()
        {
            TestContext contextA = new TestContext { CanChase = true };
            TestContext contextB = new TestContext { CanChase = false };

            FakeState patrolA = new FakeState("patrol");
            FakeState chaseA = new FakeState("chase");
            FakeState patrolB = new FakeState("patrol");
            FakeState chaseB = new FakeState("chase");

            StateMachine<TestContext> machineA = CreateMachine(contextA, patrolA, chaseA);
            StateMachine<TestContext> machineB = CreateMachine(contextB, patrolB, chaseB);

            StateTransition<TestContext> sharedTransition = new StateTransition<TestContext>(
                patrolA.Id, chaseA.Id, 100, context => context.CanChase);

            machineA.AddTransition(sharedTransition);
            machineB.AddTransition(sharedTransition);

            machineA.Start(patrolA.Id);
            machineB.Start(patrolB.Id);

            machineA.Tick(0.016f);
            machineB.Tick(0.016f);

            Assert.That(machineA.CurrentStateId, Is.EqualTo(chaseA.Id));
            Assert.That(machineB.CurrentStateId, Is.EqualTo(patrolB.Id));

            Assert.That(chaseA.EnterCount, Is.EqualTo(1));
            Assert.That(chaseB.EnterCount, Is.Zero);
            Assert.That(patrolB.TickCount, Is.EqualTo(1));
        }
    }
}
