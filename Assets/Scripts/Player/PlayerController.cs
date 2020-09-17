using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace RabbitLabirint
{
    public class PlayerController : MonoSingleton<PlayerController>
    {
        #region Player Variables

        [SerializeField]
        private GameObject playerPrefab;
        [HideInInspector]
        public GameObject currentPlayerGO;

        
        private Camera cam;
        private int currentPoints;
        private RouteBuilder routeBuilder;
        private Grid grid;
        private Tilemap tilemap;

        public bool OnFinish;

        public Vector3 Coordinate
        {
            get
            {
                return gameObject.transform.position;
            }
        }

        public int Steps { get; private set; }

        public RouteBuilder RouteBuilder
        {
            get
            {
                return routeBuilder;
            }
        }

        #endregion

        public delegate void OnHappenedFinish();
        public static event OnHappenedFinish onHappenedFinish;





        protected PlayerBaseState[] states = new PlayerBaseState[] { 
            new PlayerEmptyState(),
            new PlayerIdleState(),
            new PlayerWaitingState(),
            new PlayerMovingState(),
            new PlayerAttackedState(),
            new PlayerFinishingState(),
            new PlayerFinishedState(),
        };
        protected List<PlayerBaseState> stateStack = new List<PlayerBaseState>();
        protected Dictionary<string, PlayerBaseState> stateDictionary = new Dictionary<string, PlayerBaseState>();

        public PlayerBaseState topState
        {
            get
            {
                if (stateStack.Count == 0)
                { return null; }

                return stateStack[stateStack.Count - 1];
            }
        }

        public override void Init()
        {
            base.Init();

            // We build a dictionnary from state for easy switching using their name.
            stateDictionary.Clear();

            if (states.Length == 0)
            {
                return;
            }

            for (int i = 0; i < states.Length; i += 1)
            {
                //states[i].gameManager = Instance;
                stateDictionary.Add(states[i].GetName(), states[i]);
            }

            stateStack.Clear();

            PushState(states[0].GetName());

            OnFinish = false;
        }

        private void Update()
        {
            if (stateStack.Count > 0)
            {
                stateStack[stateStack.Count - 1].Tick();
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.name == "RabbitHole")
            {
                OnFinish = true;
            }
            else if (collision.tag == "Enemy")
            {
                Debug.Log("You are died!");
                SwitchState("Attacked");
            }
        }


        //private void OnGUI()
        //{
        //    Event currentEvent = Event.current;
        //    Vector2 mousePos = new Vector2();
        //    Vector2 point = new Vector2();

        //    // compute where the mouse is in world space
        //    mousePos.x = currentEvent.mousePosition.x;
        //    mousePos.y = cam.pixelHeight - currentEvent.mousePosition.y;
        //    point = cam.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 0.0f));

        //    if (Input.GetMouseButtonDown(0))
        //    {
        //        // set the target to the mouse click location
        //        target = point;
        //    }
        //}

        public void ChangePoints(int amount)
        {
            currentPoints += amount;
            UICarrot.Instance.SetValue(currentPoints);
        }

        public void ChangeSteps(int amount)
        {
            Steps -= amount;
            UIStep.Instance.SetValue(Steps);
        }

        /// <summary>
        /// Set player's data (for the level)
        /// </summary>
        public void SetPlayerData()
        {
            LevelProvider lvl = LevelManager.Instance.LevelData;
            if (lvl == null)
            {
                Debug.Log("Level is not available!");
                GameManager.Instance.SwitchState("Loadout");
                return;
            }

            transform.position = lvl.PlayerStartPosition.position;

            currentPoints = 0;
            Steps = lvl.MaxSteps;

            grid = lvl.Grid;
            tilemap = lvl.Tilemap;
            routeBuilder = lvl.RouteBuilder;

            DestroyPlayerGO();
            CreatePlayerGO();
        }

        /// <summary>
        /// Reset player's data (reset attached game object and etc.)
        /// </summary>
        public void ResetPlayerData()
        {
            transform.position = Vector3.zero;

            currentPoints = 0;
            Steps = 0;

            grid = null;
            tilemap = null;
            routeBuilder = null;

            DestroyPlayerGO();
        }

        public void CreatePlayerGO()
        {
            currentPlayerGO = Instantiate(playerPrefab, gameObject.transform, false) as GameObject;
        }

        private void DestroyPlayerGO()
        {
            if (currentPlayerGO != null)
            {
                Destroy(currentPlayerGO);
                Debug.Log("Reset player " + currentPlayerGO.ToString());
            }
        }

        #region State management
        /// <summary>
        /// Switch state on the state with name: newState
        /// </summary>
        /// <param name="newState">Name of the state</param>
        public void SwitchState(string newState)
        {
            PlayerBaseState state = FindState(newState);
            if (state == null)
            {
                Debug.LogError("Can't find the state named " + newState);
                return;
            }

            PlayerBaseState oldState = stateStack[stateStack.Count - 1];
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
        public PlayerBaseState FindState(string stateName)
        {
            PlayerBaseState state;
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
            PlayerBaseState state;
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


        public static void CallOnFinished()
        {
            if (onHappenedFinish != null)
            {
                onHappenedFinish();
            }
        }

    }

}
