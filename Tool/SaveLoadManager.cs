using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveLoadManager : MonoBehaviour
{
    public static SaveLoadManager Instance { get; set;}

    public string highScoreKey = "BestScoreSavedValue";

    public void Awake() 
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        // 销毁所有非 DontDestroyOnLoad 的光源，或特定标记的旧光源
        Light[] lights = FindObjectsOfType<Light>();

        foreach (Light light in lights)
        {
            // 如果光源在 DontDestroyOnLoad 场景中且是旧场景的，销毁它
            if (light.gameObject.scene.name == "DontDestroyOnLoad")
            {
                Destroy(light.gameObject);
            }
        }

        DontDestroyOnLoad(this);
    }

    public void SaveHighScore(int score)
    {
        PlayerPrefs.SetInt(highScoreKey, score);
    }

    public int LoadHighScore()
    {
        if (PlayerPrefs.HasKey(highScoreKey))
        {
            return PlayerPrefs.GetInt(highScoreKey);
        }
        else
        {
            return 0;
        }
    }
}
