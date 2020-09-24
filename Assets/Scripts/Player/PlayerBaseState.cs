using System;
using UnityEngine;

namespace RabbitLabirint
{
    public abstract class PlayerBaseState
    {
        private string name;

        public virtual Vector3 InputPosition { get; }

        protected PlayerBaseState(string name)
        {
            this.name = name ?? throw new ArgumentNullException(nameof(name));
        }        

        public abstract void Enter(PlayerBaseState prevState);
        public abstract void Exit(PlayerBaseState nextState);
        public abstract void Tick();

        public abstract void FixedTick();

        /// <summary>
        /// Get name of the state
        /// </summary>
        /// <returns>string name</returns>
        public string GetName()
        {
            return name;
        }
    }
}
