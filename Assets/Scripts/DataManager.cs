using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance;
    public int highScore;
    public bool tutorial;
    public List<int> values = new List<int>();
    public int scoreMultiplier = 1;
    public int minSpawnValue = 2;
    public int maxSpawnValue = 64;
    public int currentLevel = 1;
    public int sliderValue;

    void Awake()
    {
        Instance = this;
#if UNITY_EDITOR
        // File.Delete(Application.persistentDataPath + "/ctadata.json"); // This line can be activated, If you want to delete save file.
#endif
    }

    [System.Serializable]
    class SaveData
    {
        public int highScore;
        public bool tutorial;
        public List<int> values = new List<int>();
        public int scoreMultiplier;
        public int minSpawnValue;
        public int maxSpawnValue;
        public int currentLevel;
        public int sliderValue;

    }

    public void Save()
    {
        SaveData data = new SaveData();
        data.highScore = highScore;
        data.tutorial = tutorial;
        data.values = values;
        data.scoreMultiplier = scoreMultiplier;
        data.minSpawnValue = minSpawnValue;
        data.maxSpawnValue = maxSpawnValue;
        data.currentLevel = currentLevel;
        data.sliderValue = sliderValue;

        string json = JsonUtility.ToJson(data);
        File.WriteAllText(Application.persistentDataPath + "/ctadata.json", json);
    }
    public void Load()
    {
        string path = Application.persistentDataPath + "/ctadata.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            SaveData data = JsonUtility.FromJson<SaveData>(json);
            highScore = data.highScore;
            tutorial = data.tutorial;
            values = data.values;
            scoreMultiplier = data.scoreMultiplier;
            minSpawnValue = data.minSpawnValue;
            maxSpawnValue = data.maxSpawnValue;
            currentLevel = data.currentLevel;
            sliderValue = data.sliderValue;

        }
    }
}