using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public TMP_Text highScoreUI;
    string newGameScene = "MainScene";

    public AudioClip bg_music;
    public AudioSource main_channel;

    void Start()
    {
        main_channel.PlayOneShot(bg_music);
        Cursor.lockState = CursorLockMode.None;

        // Set the high score text
        int highScore = SaveLoadManager.Instance.LoadHighScore();
        highScoreUI.text = $"Top Score: {highScore}";
    }

    public void SatrtNewGame()
    {
        main_channel.Stop();
        
        SceneManager.LoadScene(newGameScene);
    }

    public void ExitApplication() 
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
    
}
