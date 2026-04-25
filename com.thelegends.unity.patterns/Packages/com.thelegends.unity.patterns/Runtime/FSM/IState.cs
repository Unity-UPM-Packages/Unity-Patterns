namespace TheLegends.Base.FSM
{
    public interface IState<T>
    {
        void OnEnter(T context);
        void OnUpdate(T context);
        void OnFixedUpdate(T context);
        void OnLateUpdate(T context);
        void OnExit(T context);
    }
}
