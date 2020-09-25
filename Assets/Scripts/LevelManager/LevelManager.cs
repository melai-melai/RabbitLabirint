using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;
using System;

namespace RabbitLabirint
{
    public class LevelManager : MonoSingleton<LevelManager>
    {
        [System.Serializable]
        public class Level
        {
            public string levelName;
            public GameObject levelPrefab;

            //public Button.ButtonClickedEvent onClickEvent;

            public enum LevelState
            {
                Locked,
                Unlocked                
            }

            public LevelState currentState;

            public enum ResultPassing
            {
                NotPassed,
                Low,
                Middle,
                High
            }

            public ResultPassing currentResult;
        }

        public GameObject canvas;
        public GameObject levelButton;
        public Transform levelBox;
        public List<Level> levelList = new List<Level>();

        private Level currentLevel;
        [SerializeField]
        private string defaultLevelName;

        public string CurLevelName
        {
            get
            {
                return currentLevel.levelName;
            }
        }

        public string DefaultLevelName
        {
            get
            {
                return defaultLevelName;
            }
        }

        public LevelProvider LevelData { get; private set; }

        private void Start()
        {
            string savedCurrentLevelName = PlayerData.Instance.currentLevel;
            Debug.Log("Saved current level name: " + savedCurrentLevelName);
            currentLevel = FindLevelByName(savedCurrentLevelName);

            FillList();
        }

        /// <summary>
        /// Add UI buttons of levels
        /// </summary>
        private void FillList()
        {
            foreach (var level in levelList)
            {
                GameObject newButtonGO = Instantiate(levelButton, levelBox.transform, false) as GameObject;
                Text btnText = newButtonGO.GetComponentInChildren<Text>();
                Button newButton = newButtonGO.GetComponent<Button>();
                btnText.text = level.levelName + " - " + level.currentState + " - " + level.currentResult;

                switch (level.currentState)
                {
                    case Level.LevelState.Locked:
                        newButton.interactable = false;
                        Debug.Log("Locked");
                        break;
                    case Level.LevelState.Unlocked:
                        newButton.GetComponent<Button>().onClick.AddListener(() => LoadSelectedLevel(level));
                        Debug.Log("Unlocked");
                        break;
                }

                switch (level.currentResult)
                {
                    case Level.ResultPassing.NotPassed:
                        Debug.Log("NotPassed");
                        break;
                    case Level.ResultPassing.Low:
                        Debug.Log("Low");
                        break;
                    case Level.ResultPassing.Middle:
                        Debug.Log("Middle");
                        break;
                    case Level.ResultPassing.High:
                        Debug.Log("High");
                        break;
                }
            }
        }

        /// <summary>
        /// Load level (Instantiate level prefab and prepare UI)
        /// </summary>        
        public void LoadLevel(Level level = null)
        {
            if (level != null)
            {
                currentLevel = level;
            }

            DeleteLevel();

            if (currentLevel == null)
            {
                Debug.Log("The current level cannot be loaded");
                GameManager.Instance.SwitchState("Loadout");
                return;
            }

            CreateLevel();
            PlayerController.Instance.SwitchState("Idle");

            PlayerData.Instance.currentLevel = currentLevel.levelName;
            PlayerData.Instance.Save();

            Debug.Log("Load level");
        }

        /// <summary>
        /// Show/Hidden level select popup (canvas)
        /// </summary>
        /// <param name="isHidden"></param>
        public void ToggleLevelSelectPopup(bool isHidden)
        {
            canvas.gameObject.SetActive(isHidden);

            if (GameManager.Instance.topState.GetName() == "Game")
            {
                if (canvas.gameObject.activeSelf)
                {
                    GameManager.Instance.topState.Pause(false);
                }
                else
                {
                    GameManager.Instance.topState.Resume();
                }
            }

        }


        /// <summary>
        /// Create new level
        /// </summary>
        private void CreateLevel()
        {
            GameObject newLevelGO = Instantiate(currentLevel.levelPrefab);
            LevelData = newLevelGO.GetComponent<LevelProvider>();
        }

        /// <summary>
        /// Delete level from scene
        /// </summary>
        public void DeleteLevel()
        {
            GameObject oldLevel = GameObject.FindGameObjectWithTag("Level");

            if (oldLevel != null)
            {
                Destroy(oldLevel);
                LevelData = null;
            }
        }

        /// <summary>
        /// Repeat current active level
        /// </summary>
        public void RepeatLevel()
        {
            GameManager.Instance.SwitchState("Game");
        }

        /// <summary>
        /// Load selected level
        /// </summary>
        /// <param name="level"></param>
        public void LoadSelectedLevel(Level level)
        {
            currentLevel = level;
            ToggleLevelSelectPopup(false);
            GameManager.Instance.SwitchState("Game");
        }

        /// <summary>
        /// Load next level
        /// </summary>
        public void LoadNextLevel()
        {
            //Level nextLevel = levelList.Find(item => item.levelName == currentLevel.levelName)
            Level nextLevel = FindNextLevel();
            if (nextLevel == null)
            {
                Debug.Log("The last level was finished! There is not a new next level. Go to Loadout state!");
                GameManager.Instance.SwitchState("Loadout"); 
            }
            else
            {
                currentLevel = nextLevel;
                GameManager.Instance.SwitchState("Game");
            }            
        }

        /// <summary>
        /// Searches for a level by name
        /// </summary>
        /// <param name="levelName">The name of the level to find</param>
        /// <returns></returns>
        private Level FindLevelByName(string levelName)
        {
            if (levelName == "" || levelName == null)
            {
                return null;
            }

            Level level = levelList.Find(item => item.levelName == levelName);
            return level;
        }

        /// <summary>
        /// Searches for the next level after the current one
        /// </summary>
        /// <returns></returns>
        private Level FindNextLevel()
        {
            int currentLevelIndex = levelList.IndexOf(currentLevel);
            if (currentLevelIndex == (levelList.Count - 1)) // reach the last level
            {
                return null;
            }
            else
            {
                Level nextLevel = levelList[currentLevelIndex + 1];
                return nextLevel;
            }
        }
    }
}
