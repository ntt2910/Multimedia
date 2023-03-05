using System;

namespace BW.StateMachine
{
    #region NONE GENERIC VERSION

    public abstract class MachineState : MachineBehaviour, StateInterface
    {
        public bool IsActive
        {
            get { return machine.IsCurrentState(GetType()); }
        }

        public MachineInterface machine { get; set; }

        public void Enter()
        {
        }

        public void Execute()
        {
        }

        public void Exit()
        {
        }

        public T GetMachine<T>() where T : MachineInterface
        {
            try
            {
                return (T) machine;
            }
            catch (InvalidCastException e)
            {
                if (typeof(T) == typeof(MachineState) || typeof(T).IsSubclassOf(typeof(MachineState)))
                {
                    throw new Exception(machine.name + ".GetMachine() cannot return the type you requested!\tYour machine is derived from MachineBehaviour not MachineState!" + e.Message);
                }
                else if (typeof(T) == typeof(MachineBehaviour) || typeof(T).IsSubclassOf(typeof(MachineBehaviour)))
                {
                    throw new Exception(machine.name + ".GetMachine() cannot return the type you requested!\tYour machine is derived from MachineState not MachineBehaviour!" + e.Message);
                }
                else
                {
                    throw new Exception(machine.name + ".GetMachine() cannot return the type you requested!\n" + e.Message);
                }
            }
        }
    }

    #endregion

    #region GENERIC VERSION

    public abstract class MachineState<M> : MachineState where M : MachineBehaviour
    {
        public new M machine
        {
            get { return (M) base.machine; }
        }
    }

    #endregion
}