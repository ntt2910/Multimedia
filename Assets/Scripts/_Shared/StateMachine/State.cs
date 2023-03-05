// COMMENT TO SILENCE
//#define BYTHETALE_STATEMACHINE_VERBOSE

using System;
using UnityEngine;

namespace BW.StateMachine
{
    [Serializable]
    public abstract class State : MonoBehaviour, StateInterface
    {
        public virtual void Execute()
        {
        }

        public virtual void InitializeStateMachine()
        {
#if (BYTHETALE_STATEMACHINE_VERBOSE)
            UnityEngine.Debug.Log(machine.name + "." + GetType().Name + "::" + System.Reflection.MethodBase.GetCurrentMethod().Name + "()");
#endif // BYTHETALE_STATEMACHINE_VERBOSE
        }

        public virtual void Enter()
        {
#if (BYTHETALE_STATEMACHINE_VERBOSE)
            UnityEngine.Debug.Log(machine.name + "." + GetType().Name + "::" + System.Reflection.MethodBase.GetCurrentMethod().Name + "()");
#endif // BYTHETALE_STATEMACHINE_VERBOSE
        }

        public virtual void Exit()
        {
#if (BYTHETALE_STATEMACHINE_VERBOSE)
            UnityEngine.Debug.Log(machine.name + "." + GetType().Name + "::" + System.Reflection.MethodBase.GetCurrentMethod().Name + "()");
#endif // BYTHETALE_STATEMACHINE_VERBOSE
        }

        public virtual void Reset()
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

        public MachineInterface machine { get; set; }

        public bool IsActive
        {
            get { return machine.IsCurrentState(GetType()); }
        }

#if UNITY_EDITOR
        [ContextMenu("Enter State")]
        protected void EditorChangeState()
        {
            machine.ChangeState(GetType());
        }
#endif
    }

    public abstract class State<M> : State where M : MonoBehaviour, MachineInterface
    {
        public new M machine
        {
            get { return (M) base.machine; }
        }
    }
}