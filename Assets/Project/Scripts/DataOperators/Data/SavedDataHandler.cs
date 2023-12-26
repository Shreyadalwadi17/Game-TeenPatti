using BaseFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


namespace Game.BaseFramework
{
    public class SavedDataHandler : Singleton<SavedDataHandler>
    {
        [Header("Data Credentials")] public string password;

        [Header("Current Save Data")] public SaveData _saveData;

        [Header("Default Data")] public SaveData _DefaultSaveData;


        public override void OnAwake()
        {
            base.OnAwake();
            print(Application.persistentDataPath);
            if (!File.Exists(SaveGameData.filePath))
            {
                print("does not exist");
                SetFirstLaunch();
            }
            else
            {
                _saveData = SaveGameData.Load(_DefaultSaveData, password);
            }
            //_saveData = SaveGameData.Load(_DefaultSaveData, password);
        }


        private void OnApplicationFocus(bool hasFocus)
        {
            if (!hasFocus)
            {
                //SaveGameData.Save(_saveData, password);
            }
            else
            {
                //_saveData = SaveGameData.Load(_DefaultSaveData, password);
            }
        }

        public void ResetToDefault()
        {
            _saveData = SaveGameData.Clear(_DefaultSaveData, password);
        }

        public void SetFirstLaunch()
        {
            if (!_saveData.isFirstLaunch)
            {
                _saveData.isFirstLaunch = true;
            }

            SaveGameData.Save(_saveData, password);
        }


        public void ResetData()
        {
            _saveData.isFirstLaunch = false;
            SetFirstLaunch();
        }
    }

    [Serializable]
    public class SaveData
    {
        public bool isFirstLaunch;
    }
}