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

        private void Start()
        {
            currentLevel = levelList[levelList.Count - 1]; // TODO: get current level from player data

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
            Instantiate(currentLevel.levelPrefab);
            PlayerController.Instance.PreparePlayerForLevel();
            UIStep.Instance.SetValue(PlayerController.Instance.Steps);
            UICarrot.Instance.SetValue(0);

            if (GameManager.Instance.topState.GetName() == "Loadout")
            {
                GameManager.Instance.topState.LoadSelectedLevel();
            }
            
            ToggleLevelSelectPopup(false);
        }

        /// <summary>
        /// Show/Hidden level select popup (canvas)
        /// </summary>
        /// <param name="isHidden"></param>
        public void ToggleLevelSelectPopup(bool isHidden)
        {
            canvas.gameObject.SetActive(isHidden);
            //if (GameManager.Instance.topState.GetName() == "Game")
            //{
            //    if (canvas.gameObject.activeSelf)
            //    {
            //        GameManager.Instance.topState.Pause(false);
            //    }
            //    else
            //    {
            //        GameManager.Instance.topState.Resume();
            //    }
            //}
            
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
            Instantiate(currentLevel.levelPrefab);
            PlayerController.Instance.PreparePlayerForLevel();
            UIStep.Instance.SetValue(PlayerController.Instance.Steps);
            UICarrot.Instance.SetValue(0);
        }

        /// <summary>
        /// Load next level
        /// </summary>
        public void LoadNextLevel()
        {
            //Level nextLevel = levelList.Find(item => item.levelName == currentLevel.levelName)
            int currentLevelIndex = levelList.IndexOf(currentLevel);
            if (currentLevelIndex == (levelList.Count - 1)) // reach the last level
            {
                LoadLevel(levelList[0]);
            } else
            {
                Level nextLevel = levelList[currentLevelIndex + 1];
                LoadLevel(nextLevel);
            }            
        }
    }
}
