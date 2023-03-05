using System;

namespace BW.StateMachine
{
    public interface MachineInterface
    {
        void SetInitialState<T>() where T : StateInterface;
        void SetInitialState(Type T);

        void ChangeState<T>() where T : StateInterface;
        void ChangeState(Type T);

        bool IsCurrentState<T>(bool allowBaseType = false) where T : StateInterface;
        bool IsCurrentState(Type T);

        T GetState<T>() where T : StateInterface;

        bool AddState(StateInterface state);
        bool AddState<T>(StateInterface state);
        bool AddState(Type T);

        void RemoveState<T>() where T : StateInterface;
        void RemoveState(Type T);

        bool HasState<T>() where T : StateInterface;
        bool HasState(Type T);

        void RemoveAllStates();
        void Reset();

        string name { get; set; }
    }
}