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
            public int state;

            //public Button.ButtonClickedEvent onClickEvent;
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
                GameObject newButton = Instantiate(levelButton, levelBox.transform, false) as GameObject;
                Text btnText = newButton.GetComponentInChildren<Text>();
                btnText.text = level.levelName;
                newButton.GetComponent<Button>().onClick.AddListener(() => LoadLevel(level));
            }
        }

        /// <summary>
        /// Load level (Instantiate level prefab and prepare UI)
        /// </summary>
        /// <param name="level"></param>
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

            LevelProvider newLevelProvider = CreateLevel();
            PlayerController.Instance.PreparePlayerForLevel(newLevelProvider);
            UIStep.Instance.SetValue(PlayerController.Instance.Steps);
            UICarrot.Instance.SetValue(0);

            if (GameManager.Instance.topState.GetName() == "Loadout")
            {
                GameManager.Instance.topState.LoadSelectedLevel();
            }

            ToggleLevelSelectPopup(false);

            PlayerData.Instance.currentLevel = currentLevel.levelName;
            PlayerData.Instance.Save();
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
        /// <returns>LevelProvider contains data about level</returns>
        private LevelProvider CreateLevel()
        {
            GameObject newLevelGO = Instantiate(currentLevel.levelPrefab);
            LevelProvider newLevelProvider = newLevelGO.GetComponent<LevelProvider>();

            return newLevelProvider;
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
            }
        }

        /// <summary>
        /// Repeat current active level
        /// </summary>
        public void RepeatLevel()
        {
            DeleteLevel();

            LevelProvider newLevelProvider = CreateLevel();

            PlayerController.Instance.PreparePlayerForLevel(newLevelProvider);

            UIStep.Instance.SetValue(PlayerController.Instance.Steps);
            UICarrot.Instance.SetValue(0);
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
                LoadLevel(nextLevel);
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
