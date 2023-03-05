namespace BW.StateMachine
{
    public interface StateInterface
    {
        bool IsActive { get; }

        MachineInterface machine { set; get; }

        T GetMachine<T>() where T : MachineInterface;

        void InitializeStateMachine();

        void Enter();
        void Execute();
        void Exit();
        void Reset();
    }
}