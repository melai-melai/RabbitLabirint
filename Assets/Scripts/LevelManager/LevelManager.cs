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
        public Text levelTitle;
        public GameObject levelButtonPrefab;
        public Transform levelBox;
        public List<Level> levelList = new List<Level>();               // full level list of game
        private List<Level> playerLevelList;                            // total player level list for drawing of level buttons

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
                return GetResult();
            }
        }

        public LevelProvider LevelData { get; private set; }

        private void Start()
        {
            playerLevelList = new List<Level>();

            FormPlayerList();

            SetCurrentLevel();

            DrawLevelList();
        }

        /// <summary>
        /// Form player's level list (possible game levels plus player's saved levels)
        /// </summary>
        public void FormPlayerList()
        {
            playerLevelList.Clear();

            // copy full level list of game
            foreach (Level lvl in levelList)
            {
                Level newLevel = new Level
                {
                    levelName = lvl.levelName,
                    levelPrefab = lvl.levelPrefab,
                    currentState = lvl.currentState,
                    currentResult = lvl.currentResult
                };

                playerLevelList.Add(newLevel);
            }

            Dictionary<string, int> savedLevels = PlayerData.Instance.levels;   // saved levels
            string nameLastSavedLevel = savedLevels.Keys.Last();
            bool setNextUnlocked = false;                                       // trigger for setting last available level            

            // form a summary list with levels for the player
            foreach (Level lvl in playerLevelList)
            {
                if (savedLevels.ContainsKey(lvl.levelName))
                {
                    lvl.currentState = Level.LevelState.Unlocked;
                    int savedResult = savedLevels[lvl.levelName];
                    lvl.currentResult = (Level.ResultPassing)savedResult;

                    if (lvl.currentResult != Level.ResultPassing.NotPassed && lvl.levelName == nameLastSavedLevel)
                    {
                        setNextUnlocked = true;
                    }
                }
                else if (setNextUnlocked)
                {
                    lvl.currentState = Level.LevelState.Unlocked;
                    lvl.currentResult = Level.ResultPassing.NotPassed;
                    setNextUnlocked = false;
                }
                else
                {
                    break;
                }
            }
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
            Level nextLevel = FindLevelByName(PlayerData.Instance.currentLevel);

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
            Level.ResultPassing oldResult = currentLevel.currentResult;
            Level.ResultPassing newResult = GetResult();

            if (oldResult < newResult) // the newest result is better than old
            {
                currentLevel.currentResult = newResult;
                GameObject btnGO = levelBox.Find(currentLevel.levelName).gameObject;
                ChangeLevelButton(btnGO, currentLevel.currentResult);
            }

            SaveLevel(currentLevel);

            Level nextLevel = FindNextLevel(currentLevel);
            if (nextLevel != null)
            {
                if (nextLevel.currentState == Level.LevelState.Locked)
                {
                    UnlockLevel(nextLevel);
                    UnlockButton(nextLevel);
                }

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

            return level;
        }

        /// <summary>
        /// Set current level
        /// </summary>
        public void SetCurrentLevel()
        {
            Dictionary<string, int> savedLevels = PlayerData.Instance.levels;

            // set current level
            if (savedLevels.Count == 1)
            {
                if (savedLevels[savedLevels.Keys.First()] == (int)Level.ResultPassing.NotPassed)
                {
                    currentLevel = FindLevelByName(PlayerData.Instance.currentLevel);
                }
                else
                {
                    currentLevel = FindNextLevel(PlayerData.Instance.currentLevel);
                }
            }
            else if (savedLevels.Count == levelList.Count)
            {
                currentLevel = FindLevelByName(PlayerData.Instance.currentLevel);
            }
            else
            {
                currentLevel = FindLevelByName(PlayerData.Instance.currentLevel);
            }

        }

        #region UI
        /// <summary>
        /// Add UI buttons of levels
        /// </summary>
        public void DrawLevelList()
        {
            // draw level UI buttons
            foreach (Level level in playerLevelList)
            {
                DrawButton(level);
            }
        }

        /// <summary>
        /// Destroy all level ui button with their game objects
        /// </summary>
        public void ClearLevelList()
        {
            foreach (Transform child in levelBox.transform)
            {
                Destroy(child.gameObject);
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
            GameObject newButtonGO = Instantiate(levelButtonPrefab, levelBox.transform, false) as GameObject;
            newButtonGO.name = level.levelName;

            Text btnText = newButtonGO.GetComponentInChildren<Text>();
            btnText.text = level.levelName;

            Button newButton = newButtonGO.GetComponent<Button>();            

            switch (level.currentState)
            {
                case Level.LevelState.Locked:
                    newButton.interactable = false;
                    //Debug.Log("Locked");
                    break;
                case Level.LevelState.Unlocked:
                    newButton.onClick.AddListener(() => LoadSelectedLevel(level));
                    newButton.onClick.AddListener(() => SoundManager.Instance.PlayUIButtonSound());
                    //Debug.Log("Unlocked");
                    break;
            }

            LevelButton levelButton = newButtonGO.GetComponent<LevelButton>();

            switch (level.currentResult)
            {
                case Level.ResultPassing.NotPassed:
                    //Debug.Log("NotPassed");
                    break;
                case Level.ResultPassing.Low:
                    levelButton.SetLevelStars(1);
                    //Debug.Log("Low");
                    break;
                case Level.ResultPassing.Middle:
                    levelButton.SetLevelStars(2);
                    //Debug.Log("Middle");
                    break;
                case Level.ResultPassing.High:
                    levelButton.SetLevelStars(3);
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

            ChangeLevelButton(btnGO, level.currentResult);
        }

        /// <summary>
        /// Change result of level (number of stars)
        /// </summary>
        /// <param name="btnGO"></param>
        /// <param name="resultPassing"></param>
        private void ChangeLevelButton(GameObject btnGO, Level.ResultPassing resultPassing)
        {
            //Text btnText = btnGO.GetComponentInChildren<Text>();
            //btnText.text = resultPassing.ToString();

            LevelButton levelButton = btnGO.GetComponent<LevelButton>();

            switch (resultPassing)
            {
                case Level.ResultPassing.Low:
                    levelButton.SetLevelStars(1);
                    //Debug.Log("Low");
                    break;
                case Level.ResultPassing.Middle:
                    levelButton.SetLevelStars(2);
                    //Debug.Log("Middle");
                    break;
                case Level.ResultPassing.High:
                    levelButton.SetLevelStars(3);
                    //Debug.Log("High");
                    break;
            }
        }

        /// <summary>
        /// Set level title
        /// </summary>
        public void SetLevelTitle()
        {
            levelTitle.text = currentLevel.levelName;
        }
        #endregion

        #region Helpers
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

            Level level = playerLevelList.Find(item => item.levelName == levelName);
            return level;
        }

        /// <summary>
        /// Searches for the next level after the level (parameter)
        /// </summary>
        /// <param name="level"></param>
        /// <returns>Next level</returns>
        private Level FindNextLevel(Level level)
        {
            int levelIndex = playerLevelList.IndexOf(level);
            if (levelIndex == (playerLevelList.Count - 1)) // reach the last level
            {
                return null;
            }
            else
            {
                Level nextLevel = playerLevelList[levelIndex + 1];
                return nextLevel;
            }
        }

        /// <summary>
        /// Searches for the next level by name of received level
        /// </summary>
        /// <param name="levelName">Name of received level</param>
        /// <returns>Next level</returns>
        private Level FindNextLevel(string levelName)
        {
            Level level = FindLevelByName(levelName);
            int levelIndex = playerLevelList.IndexOf(level);
            if (levelIndex == (playerLevelList.Count - 1)) // reach the last level
            {
                return null;
            }
            else
            {
                Level nextLevel = playerLevelList[levelIndex + 1];
                return nextLevel;
            }
        }

        /// <summary>
        /// Check the player's parameters and determine the current result by them
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
