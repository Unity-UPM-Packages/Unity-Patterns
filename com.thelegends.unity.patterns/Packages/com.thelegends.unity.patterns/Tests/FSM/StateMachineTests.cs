using NUnit.Framework;
using TheLegends.Base.FSM;

namespace TheLegends.Base.Tests.FSM
{
    public class StateMachineTests
    {
        private class TestContext { }

        private class TestState : IState<TestContext>
        {
            public bool EnterCalled { get; private set; }
            public bool UpdateCalled { get; private set; }
            public bool FixedUpdateCalled { get; private set; }
            public bool LateUpdateCalled { get; private set; }
            public bool ExitCalled { get; private set; }

            public void OnEnter(TestContext context) => EnterCalled = true;
            public void OnUpdate(TestContext context) => UpdateCalled = true;
            public void OnFixedUpdate(TestContext context) => FixedUpdateCalled = true;
            public void OnLateUpdate(TestContext context) => LateUpdateCalled = true;
            public void OnExit(TestContext context) => ExitCalled = true;
        }

        [Test]
        public void ChangeState_CallsEnterOnNewState()
        {
            var context = new TestContext();
            var stateMachine = new StateMachine<TestContext>(context);
            var state = new TestState();

            stateMachine.ChangeState(state);

            Assert.IsTrue(state.EnterCalled);
            Assert.AreEqual(state, stateMachine.CurrentState);
        }

        [Test]
        public void ChangeState_CallsExitOnOldState()
        {
            var context = new TestContext();
            var stateMachine = new StateMachine<TestContext>(context);
            var state1 = new TestState();
            var state2 = new TestState();

            stateMachine.ChangeState(state1);
            stateMachine.ChangeState(state2);

            Assert.IsTrue(state1.ExitCalled);
            Assert.IsTrue(state2.EnterCalled);
        }

        [Test]
        public void Update_CallsUpdateOnCurrentState()
        {
            var context = new TestContext();
            var stateMachine = new StateMachine<TestContext>(context);
            var state = new TestState();

            stateMachine.ChangeState(state);
            stateMachine.Update();

            Assert.IsTrue(state.UpdateCalled);
        }

        [Test]
        public void FixedUpdate_CallsFixedUpdateOnCurrentState()
        {
            var context = new TestContext();
            var stateMachine = new StateMachine<TestContext>(context);
            var state = new TestState();

            stateMachine.ChangeState(state);
            stateMachine.FixedUpdate();

            Assert.IsTrue(state.FixedUpdateCalled);
        }

        [Test]
        public void LateUpdate_CallsLateUpdateOnCurrentState()
        {
            var context = new TestContext();
            var stateMachine = new StateMachine<TestContext>(context);
            var state = new TestState();

            stateMachine.ChangeState(state);
            stateMachine.LateUpdate();

            Assert.IsTrue(state.LateUpdateCalled);
        }
    }
}
