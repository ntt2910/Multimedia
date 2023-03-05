using UnityEngine;

namespace BW.StateMachine
{
    public class EmptyState : StateInterface
    {
        public static readonly StateInterface NullState = new EmptyState();

        private EmptyState()
        {

        }

        public bool IsActive { get { return false; } }

        public MachineInterface machine { set; get; }

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
            return (T)machine;
        }

        public void InitializeStateMachine()
        {

        }

        public void OnAnimatorIK(int layerIndex)
        {

        }

        public void PhysicsExecute()
        {

        }

        public void PostExecute()
        {

        }

        public void Reset()
        {

        }

        public void TriggerEnterEvent(Collider collider)
        {

        }

        public void TriggerEnterEvent2D(Collider2D collider)
        {

        }

        public void TriggerExitEvent(Collider collider)
        {

        }

        public void TriggerExitEvent2D(Collider2D collider)
        {

        }

        public void TriggerStayEvent(Collider collider)
        {

        }

        public void TriggerStayEvent2D(Collider2D collider)
        {

        }
    }
}