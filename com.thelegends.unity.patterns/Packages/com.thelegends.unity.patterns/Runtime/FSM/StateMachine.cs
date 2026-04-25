namespace TheLegends.Base.FSM
{
    public class StateMachine<T>
    {
        public IState<T> CurrentState { get; private set; }
        private readonly T _context;

        public StateMachine(T context)
        {
            _context = context;
        }

        public void ChangeState(IState<T> newState)
        {
            if (CurrentState == newState) return;

            CurrentState?.OnExit(_context);
            CurrentState = newState;
            CurrentState?.OnEnter(_context);
        }

        public void Update() => CurrentState?.OnUpdate(_context);
        public void FixedUpdate() => CurrentState?.OnFixedUpdate(_context);
        public void LateUpdate() => CurrentState?.OnLateUpdate(_context);
    }
}
