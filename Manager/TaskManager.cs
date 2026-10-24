using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TaskManager : MonoBehaviour
{
    // 此任务脚本非通用脚本，可依据个人需要进行更换 （仅服务于此游玩路线设计）

    public static TaskManager Instance { get; set;}

    public TextMeshProUGUI taskTitle;
    public TextMeshProUGUI taskDescript;

    public TextMeshProUGUI taskFinal;

    public bool targerFinished;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }
    void Start()
    {
        targerFinished = false;

        taskTitle.text = "Tutorial";

        taskDescript.text = "Look around and search for your gear";
    }

    public void Level1Tutorial()
    {
        taskTitle.text = "Awakening";

        taskDescript.text = "Follow the navigation point";
    }

    public void Level2Tutorial()
    {
        taskTitle.text = "Encounter";

        taskDescript.text = "Defeat special enemies";
    }

    public void Level3Tutorial()
    {
        GameObject.Find("EnemySpawner").GetComponent<EnemySpawnController>().enabled = true;

        taskTitle.text = "Survival";

        taskDescript.text = "Survive the ambush";
    }

     public void Level4End()
    {
        StartCoroutine(ReturnToMainMenu());
    }

    private IEnumerator ReturnToMainMenu()
    {   
        taskFinal.gameObject.SetActive(true);

        yield return new WaitForSeconds(5F);

        SceneManager.LoadScene("MainMenu");
    }
}
