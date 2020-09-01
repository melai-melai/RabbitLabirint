using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RabbitLabirint
{
    public abstract class State : MonoBehaviour
    {
        [HideInInspector]
        public GameManager gameManager;

        public abstract void Enter(State from);
        public abstract void Exit(State to);
        public abstract void Tick();

        public abstract string GetName();

        public virtual void LoadSelectedLevel() { }
        public virtual void Pause(bool displayMenu) { }
        public virtual void Resume() { }
    }
}
