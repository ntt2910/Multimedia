using System;
using BW.StateMachine;
using BW.Inspector;
using TypeReferences;
using UnityEngine;

namespace BW.AIs
{
    [Serializable]
    public class BrainTransition
    {
        [SerializeField, Background(ColorEnum.Green)]
        private BrainDecision _decision;

        [SerializeField, ClassExtends(typeof(State)), Background(ColorEnum.Green)]
        private ClassTypeReference _trueBranch;

        [SerializeField, ClassExtends(typeof(State)), Background(ColorEnum.Green)]
        private ClassTypeReference _falseBranch;

        public BrainDecision Decision
        {
            get { return this._decision; }
        }

        public Type TrueBranchState
        {
            get { return this._trueBranch.Type == null ? null : this._trueBranch.Type; }
        }

        public Type FalseBranchState
        {
            get { return this._falseBranch.Type == null ? null : this._falseBranch.Type; }
        }
    }
}