namespace Infastructure.StateMachine
{
    public interface IState
    {
        bool IsActive { get; }
        void Enter();
        void Exit();
    }
}