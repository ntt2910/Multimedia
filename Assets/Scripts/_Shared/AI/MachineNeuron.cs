using System;
using BW.StateMachine;
using BW.Inspector;
using TypeReferences;
using UnityEngine;

namespace BW.AIs
{
    [Serializable]
    public class MachineNeuron
    {
        [SerializeField, ClassExtends(typeof(State)), Background(ColorEnum.Red)]
        private ClassTypeReference _state;

        [SerializeField, Background(ColorEnum.Red)]
        private BrainTransition[] _transitions;

        public Type StateType
        {
            get { return this._state.Type; }
        }

        public BrainTransition[] Transitions
        {
            get { return this._transitions; }
        }
    }
}