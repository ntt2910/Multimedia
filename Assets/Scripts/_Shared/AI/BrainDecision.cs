using BW.StateMachine;
using UnityEngine;

namespace BW.AIs
{
    public abstract class BrainDecision : ScriptableObject
    {
        public abstract bool Decide(MachineInterface machine);
    }
}