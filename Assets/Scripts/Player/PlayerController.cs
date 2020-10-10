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

        [Header("Particles")]
        public ParticleSystem jumpEffect;
        public ParticleSystem stepEffect;
        public ParticleSystem shieldEffect;
        public ParticleSystem carrotEffect;

        private AudioSource audioSource;
        public enum PlayerSound
        {
            Run,
            Jump
        }
        [Header("Sound")]
        public AudioClip runClip;
        public AudioClip jumpClip;
        public AudioClip jumpClip2;
        public AudioClip carrotClip;
        public AudioClip shieldClip;

        [HideInInspector]
        public Animator animator;
        private Camera cam;
        private Grid grid;
        private Tilemap tilemap;
        [HideInInspector]
        public bool OnFinish;
        private bool hasShield;
        private float shieldTime = -1.0f;

        private int layerMask = 1 << 8; // Bit shift the index of the layer (8 - Tilemap) to get a bit mask

        public Vector3 Coordinate
        {
            get
            {
                return gameObject.transform.position;
            }
        }

        public int Steps { get; private set; }

        public int Points { get; private set; }

        public RouteBuilder RouteBuilder { get; private set; }

        #endregion

        #region Events
        public delegate void OnHappenedFinish();
        public static event OnHappenedFinish onHappenedFinish;
        #endregion

        #region States Variables
        private const string nameEmptyState = "Empty";
        private const string nameIdleState = "Idle";
        private const string nameWaitingState = "Waiting";
        private const string nameMovingState = "Moving";
        private const string nameAttackedState = "Attacked";
        private const string nameFinishingState = "Finishing";
        private const string nameFinishedState = "Finished";
        
        protected PlayerBaseState[] states = new PlayerBaseState[] { 
            new PlayerEmptyState(nameEmptyState),
            new PlayerIdleState(nameIdleState),
            new PlayerWaitingState(nameWaitingState),
            new PlayerMovingState(nameMovingState),
            new PlayerAttackedState(nameAttackedState),
            new PlayerFinishingState(nameFinishingState),
            new PlayerFinishedState(nameFinishedState),
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
        #endregion

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

            audioSource = gameObject.GetComponent<AudioSource>();
        }

        private void Update()
        {
            if (stateStack.Count > 0)
            {
                stateStack[stateStack.Count - 1].Tick();
            }

            if (shieldTime > 0)
            {
                shieldTime -= Time.deltaTime;
                if (shieldTime < 0)
                {
                    RemoveShield();
                }
            }
        }

        private void FixedUpdate()
        {
            if (stateStack.Count > 0)
            {
                stateStack[stateStack.Count - 1].FixedTick();
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.tag == "Hole")
            {
                OnFinish = true;
            }
            else if (collision.tag == "Enemy" && topState.GetName() == nameMovingState)
            {
                EnemyController enemy = collision.gameObject.GetComponent<EnemyController>();

                if (hasShield)
                {
                    RemoveShield();
                    if (enemy != null)
                    {
                        StartCoroutine(enemy.HideAway());
                    }
                }
                else
                {
                    if (enemy != null)
                    {
                        StartCoroutine(enemy.Attack());
                    }
                }                                
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.gameObject.tag == "Hole")
            {
                Debug.Log("Collision HOLE END");
                OnFinish = false;
            }
        }

        /// <summary>
        /// Change player's points
        /// </summary>
        /// <param name="amount"></param>
        public void ChangePoints(int amount)
        {
            Points += amount;
            UICarrot.Instance.SetValue(Points);
        }

        /// <summary>
        /// Change player's steps
        /// </summary>
        /// <param name="amount"></param>
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

            Steps = lvl.MaxSteps;
            
            grid = lvl.Grid;
            tilemap = lvl.Tilemap;
            RouteBuilder = lvl.RouteBuilder;

            DestroyPlayerGO();
            CreatePlayerGO();

            animator = currentPlayerGO.GetComponent<Animator>();
        }

        /// <summary>
        /// Reset player's data (reset attached game object and etc.)
        /// </summary>
        public void ResetPlayerData()
        {
            transform.position = Vector3.zero;

            Points = 0;
            OnFinish = false;

            Steps = 0;

            grid = null;
            tilemap = null;
            RouteBuilder = null;
            animator = null;

            DestroyPlayerGO();
        }

        /// <summary>
        /// Create player's game object (game object with player's sprite)
        /// </summary>
        public void CreatePlayerGO()
        {
            currentPlayerGO = Instantiate(playerPrefab, gameObject.transform, false) as GameObject;
            stepEffect.Play();
        }

        /// <summary>
        /// Destroy player's game object (game object with player's sprite)
        /// </summary>
        private void DestroyPlayerGO()
        {
            if (currentPlayerGO != null)
            {
                Destroy(currentPlayerGO);
            }
            stepEffect.Stop();
        }

        /// <summary>
        /// Set player's shield
        /// </summary>
        /// <param name="time">Duration</param>
        public void SetShield(float time)
        {
            hasShield = true;
            shieldTime = time;
            shieldEffect.Play();
            audioSource.PlayOneShot(shieldClip);
        }

        /// <summary>
        /// Remove player's shield
        /// </summary>
        public void RemoveShield()
        {
            hasShield = false;
            shieldTime = -1.0f;
            shieldEffect.Stop();
        }

        /// <summary>
        /// Eat carrot
        /// </summary>
        /// <param name="point"></param>
        public void EatCarrot(int point)
        {
            carrotEffect.Play();
            audioSource.PlayOneShot(carrotClip);
            ChangePoints(point);
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

        #region Animation
        /// <summary>
        /// Set moving animation depending on the direction of movement
        /// </summary>
        /// <param name="moving"></param>
        public void SetMovingAnimation(bool moving)
        {
            Vector2 direction = new Vector2(0.0f, 0.0f);

            if (moving)
            {
                direction = RouteBuilder.TargetDirection;
            }

            animator.SetFloat("Move X", direction.x);
            animator.SetFloat("Move Y", direction.y);
        }

        /// <summary>
        /// Trigger finish jump animation
        /// </summary>
        private void TriggerFinishJump()
        {
            animator.SetTrigger("Jump_Hole");
        }

        /// <summary>
        /// Trigger attacked animation
        /// </summary>
        public void TriggerAttacked()
        {
            stepEffect.Stop();
            animator.SetTrigger("Attacked");
        }
        #endregion

        #region Helpers
        /// <summary>
        /// Check hit on tilemap // TODO: need refactoring of raycast
        /// </summary>
        /// <param name="inputPosition">Construct a ray from the input coordinates</param>
        /// <returns></returns>
        public bool CheckHitTilemap(Vector3 inputPosition)
        {
            // Construct a ray from the current input coordinates
            Vector2 worldPoint = Camera.main.ScreenToWorldPoint(inputPosition);
            RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero, Mathf.Infinity, layerMask);

            //If something was hit, the RaycastHit2D.collider will not be null.
            if (hit.collider != null)
            {
                //Debug.DrawLine(worldPoint, Vector3.zero, Color.green, 10.0f, true);
                //Debug.Log(hit.transform.gameObject.name + ": " + hit.transform.gameObject.layer);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Call at the finish // TODO: needs refactoring
        /// </summary>
        public void CallOnFinished()
        {
            RemoveShield();
            stepEffect.Stop();
            TriggerFinishJump();
            jumpEffect.Play();
            PlaySound(PlayerSound.Jump);
            StartCoroutine("WaitUntilFinish");            
        }

        /// <summary>
        /// Wait until finish of level and call finish event
        /// </summary>
        /// <returns></returns>
        private IEnumerator WaitUntilFinish()
        {
            yield return new WaitForSecondsRealtime(3.0f);
            onHappenedFinish?.Invoke();
        }
        #endregion

        #region Sounds
        /// <summary>
        /// Play sound by enum PlayerSound
        /// </summary>
        /// <param name="playerSound"></param>
        public void PlaySound(PlayerSound playerSound)
        {
            switch (playerSound)
            {
                case PlayerSound.Run:
                    audioSource.loop = true;
                    audioSource.clip = runClip;
                    audioSource.Play();
                    break;
                case PlayerSound.Jump:
                    audioSource.loop = false;
                    audioSource.clip = jumpClip;
                    audioSource.PlayDelayed(1.0f);
                    StartCoroutine("WaitUntilJumpEnd");
                    break;
            }
            
        }

        /// <summary>
        /// Stop current player's sound
        /// </summary>
        public void StopSound()
        {
            audioSource.Stop();
        }

        /// <summary>
        /// Wait and play ending jump audio clip
        /// </summary>
        /// <returns></returns>
        IEnumerator WaitUntilJumpEnd()
        {
            yield return new WaitForSeconds(2.2f);
            audioSource.PlayOneShot(jumpClip2);
        }
        #endregion        
    }

}
