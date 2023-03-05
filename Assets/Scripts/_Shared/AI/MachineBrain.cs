using BW.Inspector;
using UnityEngine;

namespace BW.AIs
{
    [CreateAssetMenu(menuName = "Actor/AI/Brain", fileName = "Brain")]
    public class MachineBrain : ScriptableObject
    {
        [SerializeField, ComplexHeader("Always Check", Style.Box, Alignment.Left, ColorEnum.White, ColorEnum.Blue)]
        private BrainTransition[] coreTransitions;

        [SerializeField, ComplexHeader("Only Check If AI Was Enable", Style.Box, Alignment.Left, ColorEnum.White, ColorEnum.Blue)]
        private BrainTransition[] globalTransitions;

        [SerializeField]
        private MachineNeuron[] localTransitions;

        public BrainTransition[] CoreTransitions => this.coreTransitions;
        public BrainTransition[] GlobalTransitions => this.globalTransitions;
        public MachineNeuron[] LocalTransitions => this.localTransitions;
    }
}