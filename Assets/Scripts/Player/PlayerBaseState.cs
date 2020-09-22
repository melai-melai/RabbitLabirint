using UnityEngine;

namespace RabbitLabirint
{
    public abstract class PlayerBaseState
    {
        public virtual Vector3 InputPosition { get; }

        public abstract void Enter(PlayerBaseState prevState);
        public abstract void Exit(PlayerBaseState nextState);
        public abstract void Tick();

        public abstract void FixedTick();

        public abstract string GetName();
    }
}
