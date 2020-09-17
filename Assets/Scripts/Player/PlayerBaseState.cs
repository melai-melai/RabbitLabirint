using UnityEngine;

namespace RabbitLabirint
{
    public abstract class PlayerBaseState
    {
        public abstract void Enter(PlayerBaseState prevState);
        public abstract void Exit(PlayerBaseState nextState);
        public abstract void Tick();

        public abstract string GetName();
    }
}
