using System;
using BW.StateMachine;
using UnityEngine;

namespace BW.AIs
{
    public enum BooleanOperator
    {
        AND,
        OR,
        NONE
    }

    [CreateAssetMenu(menuName = "Actor/AI/DecisionGroup", fileName = "DecisionGroup")]
    public class BrainDecisionGroup : BrainDecision
    {
        [SerializeField] private DecisionOperator[] _decisionOperators;

        public override bool Decide(MachineInterface machine)
        {
            var result = false;

            foreach (var decisionOperator in this._decisionOperators)
            {
                var decision = decisionOperator.Decision.Decide(machine);
                decision = decisionOperator.IsNot ? !decision : decision;

                switch (decisionOperator.Operator)
                {
                    case BooleanOperator.NONE:
                        result = decision;
                        break;
                    case BooleanOperator.AND:
                        result = result && decision;
                        break;
                    default:
                        result = result || decision;
                        break;
                }
            }

            return result;
        }

        [Serializable]
        private class DecisionOperator
        {
            [SerializeField] private BrainDecision _decision;

            [SerializeField] private BooleanOperator _operator;

            [SerializeField] private bool _not;

            public BrainDecision Decision
            {
                get { return this._decision; }
            }

            public BooleanOperator Operator
            {
                get { return this._operator; }
            }

            public bool IsNot
            {
                get { return this._not; }
            }
        }
    }
}