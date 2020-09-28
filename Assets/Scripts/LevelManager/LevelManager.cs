using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;
using System;
using System.Linq;

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

        public Level.ResultPassing CurLevelResult
        {
            get
            {
                return currentLevel.currentResult;
            }
        }

        public LevelProvider LevelData { get; private set; }

        private void Start()
        {
            SetCurrentLevelFromSave();

            FillList();            
            // TODO: needs refactoring after refactoring FillList()
            //string nameLastSavedLevel = PlayerData.Instance.levels.Keys.Last();
            //currentLevel = FindLevelByName(nameLastSavedLevel); 
        }

        private void OnEnable()
        {
            PlayerController.onHappenedFinish += FinishLevel;
        }

        private void OnDisable()
        {
            PlayerController.onHappenedFinish -= FinishLevel;
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
        /// Save level
        /// </summary>
        private void SaveLevel(Level level)
        {
            if (!PlayerData.Instance.ChangeLevel(level.levelName, (int)level.currentResult))
            {
                PlayerData.Instance.AddLevel(level.levelName, (int)level.currentResult);
            }

            PlayerData.Instance.Save();
        }

        /// <summary>
        /// Save result of current level, update UI and unlock next level (use after finish) and etc.
        /// </summary>
        private void FinishLevel()
        {
            currentLevel.currentResult = GetResult();
            SaveLevel(currentLevel);
            GameObject btnGO = levelBox.Find(currentLevel.levelName).gameObject;
            ChangeButtonUIResult(btnGO, currentLevel.currentResult);

            Level nextLevel = FindNextLevel(); // TODO: need refactoring (without save next level)
            if (nextLevel != null)
            {
                nextLevel = UnlockLevel(nextLevel);
                SaveLevel(nextLevel);
                PlayerData.Instance.currentLevel = nextLevel.levelName;
                PlayerData.Instance.Save();
            } else
            {
                Debug.Log("The last level was finished! There is not a new next level");
            }           
        }

        /// <summary>
        /// Unlock level
        /// </summary>
        /// <param name="level">Locked level</param>
        /// <returns>Unlocked level</returns>
        private Level UnlockLevel(Level level)
        {
            level.currentState = Level.LevelState.Unlocked;
            level.currentResult = Level.ResultPassing.NotPassed;

            SaveLevel(level);

            UnlockButton(level);

            return level;
        }

        #region UI
        /// <summary>
        /// Add UI buttons of levels (possible game levels plus player's saved levels)
        /// </summary>
        private void FillList()
        {
            List<Level> playerLevelList = new List<Level>();

            Dictionary<string, int> savedLevels = PlayerData.Instance.levels;

            foreach (Level lvl in levelList)
            {
                if (savedLevels.ContainsKey(lvl.levelName))
                {
                    int savedResult = savedLevels[lvl.levelName];
                    lvl.currentState = Level.LevelState.Unlocked;
                    lvl.currentResult = (Level.ResultPassing)savedResult;
                }

                playerLevelList.Add(lvl);
            }

            foreach (Level level in playerLevelList)
            {
                DrawButton(level);
            }
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
        /// Draw UI level button
        /// </summary>
        /// <param name="level"></param>
        private void DrawButton(Level level)
        {
            GameObject newButtonGO = Instantiate(levelButton, levelBox.transform, false) as GameObject;
            newButtonGO.name = level.levelName;
            Text btnText = newButtonGO.GetComponentInChildren<Text>();
            Button newButton = newButtonGO.GetComponent<Button>();
            btnText.text = level.levelName + " - " + level.currentState + " - " + level.currentResult;

            switch (level.currentState)
            {
                case Level.LevelState.Locked:
                    newButton.interactable = false;
                    //Debug.Log("Locked");
                    break;
                case Level.LevelState.Unlocked:
                    newButton.onClick.AddListener(() => LoadSelectedLevel(level));
                    //Debug.Log("Unlocked");
                    break;
            }

            switch (level.currentResult)
            {
                case Level.ResultPassing.NotPassed:
                    //Debug.Log("NotPassed");
                    break;
                case Level.ResultPassing.Low:
                    //Debug.Log("Low");
                    break;
                case Level.ResultPassing.Middle:
                    //Debug.Log("Middle");
                    break;
                case Level.ResultPassing.High:
                    //Debug.Log("High");
                    break;
            }
        }

        /// <summary>
        /// Make the button playable
        /// </summary>
        /// <param name="level"></param>
        private void UnlockButton(Level level)
        {
            GameObject btnGO = levelBox.Find(level.levelName).gameObject;

            Button btn = btnGO.GetComponent<Button>();
            btn.interactable = true;
            btn.onClick.AddListener(() => LoadSelectedLevel(level));

            ChangeButtonUIResult(btnGO, level.currentResult);
        }

        /// <summary>
        /// Change result of level (number of stars) //TODO: add stars for button
        /// </summary>
        /// <param name="btnGO"></param>
        /// <param name="resultPassing"></param>
        private void ChangeButtonUIResult(GameObject btnGO, Level.ResultPassing resultPassing)
        {
            Text btnText = btnGO.GetComponentInChildren<Text>();
            btnText.text = resultPassing.ToString();
        }
        #endregion

        #region Helpers
        public void SetCurrentLevelFromSave()
        {
            string savedCurrentLevelName = PlayerData.Instance.currentLevel;
            Debug.Log("Saved current level name: " + savedCurrentLevelName);
            currentLevel = FindLevelByName(savedCurrentLevelName);
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

        /// <summary>
        /// check the player's parameters and determine the result by them
        /// </summary>
        /// <returns></returns>
        private Level.ResultPassing GetResult()
        {
            Level.ResultPassing result = Level.ResultPassing.Low;

            if (PlayerController.Instance.Steps >= 2)
            {
                result = Level.ResultPassing.High;
            }
            else if (PlayerController.Instance.Steps == 1)
            {
                result = Level.ResultPassing.Middle;
            }

            return result;
        }
        #endregion
    }
}
