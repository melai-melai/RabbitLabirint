using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace RabbitLabirint
{
    /// <summary>
    /// Save data for the game. This is stored locally in this case
    /// </summary>
    public class PlayerData
    {
        static protected PlayerData _instance;
        static public PlayerData Instance { get { return _instance; } }

        protected string saveFile = "";

        // Data is for a save
        public string currentLevel;
        public float masterVolume = float.MinValue;
        public float musicVolume = float.MinValue;
        public float masterSFXVolume = float.MinValue;

        static int version = 1;

        #region File Management
        /// <summary>
        /// Create a save instance (read saved or create new save)
        /// </summary>
        static public void Create()
        {
            if (_instance == null)
            {
                _instance = new PlayerData();

                // database loaders
            }

            _instance.saveFile = Application.persistentDataPath + "/save.bin";

            if (File.Exists(_instance.saveFile))
            {
                // If we have a save, we read it
                _instance.Read();
                Debug.Log("Read saved game data");
            }
            else
            {
                // If not we create one with default data
                CreateNewSave();
                Debug.Log("Create new game data");
            }

            Debug.Log(Application.persistentDataPath);
        }

        /// <summary>
        /// Read saved data
        /// </summary>
        public void Read()
        {
            BinaryReader r = new BinaryReader(new FileStream(saveFile, FileMode.Open));

            // ver - variable "version" of saved game; for compability with new version a game
            int ver = r.ReadInt32();

            currentLevel = r.ReadString();

            // Example:
            // Save contains the version they were written with. If data are added bump the version & test for that version before loading that data.
            if (ver >= 1)
            {
                masterVolume = r.ReadSingle();
                musicVolume = r.ReadSingle();
                masterSFXVolume = r.ReadSingle();
            }

            r.Close();
        }

        /// <summary>
        /// Create a new game save
        /// </summary>
        static public void CreateNewSave()
        {
            _instance.currentLevel = LevelManager.Instance.DefaultLevelName;

            _instance.Save();
        }

        /// <summary>
        /// Save game data
        /// </summary>
        public void Save()
        {
            BinaryWriter w = new BinaryWriter(new FileStream(saveFile, FileMode.OpenOrCreate));

            w.Write(version);

            w.Write(currentLevel);

            w.Write(masterVolume);
            w.Write(musicVolume);
            w.Write(masterSFXVolume);

            w.Close();
        }
        #endregion        
    }
}
