using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RabbitLabirint
{
    /// <summary>
    /// The Game manager is a state machine, that will switch between state according to current gamestate.
    /// </summary>
    public class GameManager : MonoSingleton<GameManager>
    {
        public State[] states;
        protected List<State> stateStack = new List<State>();
        protected Dictionary<string, State> stateDictionary = new Dictionary<string, State>();

        private AudioSource audioSource;

        public State topState
        {
            get { 
                if (stateStack.Count == 0)
                { return null; }

                return stateStack[stateStack.Count - 1];
            }
        }

        public override void Init()
        {
            base.Init();

            

            audioSource = GetComponent<AudioSource>();
            //PlayNewBackgroundMusic(menuClip);

            // We build a dictionnary from state for easy switching using their name.
            stateDictionary.Clear();

            if (states.Length == 0)
            {
                return;
            }

            for (int i = 0; i < states.Length; i += 1)
            {
                states[i].gameManager = Instance;
                stateDictionary.Add(states[i].GetName(), states[i]);
            }

            stateStack.Clear();

            PushState(states[0].GetName());
        }

        private void OnEnable()
        {
            PlayerData.Create();
        }

        
        
        private void Update()
        {
            if (stateStack.Count > 0)
            {
                stateStack[stateStack.Count - 1].Tick();
            }
        }

        #region State management
        /// <summary>
        /// Switch state on the state with name: newState
        /// </summary>
        /// <param name="newState">Name of the state</param>
        public void SwitchState(string newState)
        {
            State state = FindState(newState);
            if (state == null)
            {
                Debug.LogError("Can't find the state named " + newState);
                return;
            }

            State oldState = stateStack[stateStack.Count - 1];
            stateStack[stateStack.Count - 1].Exit(state);
            stateStack.RemoveAt(stateStack.Count - 1);
            stateStack.Add(state);
            state.Enter(oldState);
        }

        /// <summary>
        /// Finds the state in the dictionary
        /// </summary>
        /// <param name="stateName">Name to search</param>
        /// <returns>Found state</returns>
        public State FindState(string stateName)
        {
            State state;
            if (!stateDictionary.TryGetValue(stateName, out state))
            {
                return null;
            }

            return state;
        }

        /// <summary>
        /// Pop state from stack
        /// </summary>
        public void PopState()
        {
            if (stateStack.Count < 2)
            {
                Debug.LogError("Can't pop states, only one in stack.");
                return;
            }

            stateStack[stateStack.Count - 1].Exit(stateStack[stateStack.Count - 2]);
            stateStack[stateStack.Count - 2].Enter(stateStack[stateStack.Count - 2]);
            stateStack.RemoveAt(stateStack.Count - 1);
        }
        
        /// <summary>
        /// Push state to stack
        /// </summary>
        /// <param name="name">Name of the state</param>
        public void PushState(string name)
        {
            State state;
            if (!stateDictionary.TryGetValue(name, out state))
            {
                Debug.LogError("Can't find the state named " + name);
                return;
            }

            if (stateStack.Count > 0)
            {
                stateStack[stateStack.Count - 1].Exit(state);
                state.Enter(stateStack[stateStack.Count - 1]);
            }
            else
            {
                state.Enter(null);
            }

            stateStack.Add(state);
        }
        #endregion

        #region Helpers
        /// <summary>
        /// Set new background audio clip and play it
        /// </summary>
        /// <param name="newClip"></param>
        public void PlayNewBackgroundMusic(AudioClip newClip)
        {
            audioSource.clip = newClip;
            audioSource.Play();
        }

        /// <summary>
        /// Change active screen
        /// </summary>
        /// <param name="oldScreen"></param>
        /// <param name="newScreen"></param>
        public void ChangeScreen(GameObject oldScreen, GameObject newScreen)
        {
            oldScreen.gameObject.SetActive(false);
            newScreen.gameObject.SetActive(true);
        }
        #endregion
    }

}
