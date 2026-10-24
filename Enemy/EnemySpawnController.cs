using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnemySpawnController : MonoBehaviour
{
    public int initialEnemiesPerWave = 5;
    public int currentEnemiesPerWave;
    public int enemiesPerWaveIncrement = 2;

    public float spawnDelay = 0.5f; // Delay between spawining each enemy in a wave

    public int currentWave = 0;
    public int maxWave = 2;
    public float waveCooldown = 10.0f;// Time in seconds between waves;

    public bool inCooldown;
    public float cooldownCounter = 0; // this for testing and the UI

    public List<Enemy> currentEnemiesAlive;

    public GameObject enemyPrefab;

    public GameObject enemywaypointCluster;

    public TextMeshProUGUI waveOverUI;
    public TextMeshProUGUI cooldownCounterUI;
    public TextMeshProUGUI currentWaveUI;

    private void Start()
    {
        currentEnemiesPerWave = initialEnemiesPerWave;

        StartNextWave();
    }

    private void StartNextWave()
    {
        currentEnemiesAlive.Clear();

        currentWave++;

        GlobalReferences.Instance.scoreNumber += (currentWave - 1) * 100;

        currentWaveUI.text = "Wave :" + currentWave.ToString() + "";

        StartCoroutine(SpawnWave());
    }

    private IEnumerator SpawnWave()
    {
        for(int i=0; i < currentEnemiesPerWave; i++)
        {
            // Generate a random offset within a specified range
            Vector3 spawnOffset = new Vector3(Random.Range(-1f,1f), 0f, Random.Range(-1,1f));
            Vector3 spawnPosition = transform.position + spawnOffset;

            // Instantiate the Enemies
            var enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);

            // Get Enemy Script
            Enemy enemyScript = enemy.GetComponent<Enemy>();

            enemyScript.waypointCluster = enemywaypointCluster;

            // Track this enemy
            currentEnemiesAlive.Add(enemyScript);

            yield return new WaitForSeconds(spawnDelay);
        }
    }

    private void Update() 
    {
        // Get All dead enemies
        List<Enemy> enemiesToRemove = new List<Enemy>();
        foreach(Enemy enemy in currentEnemiesAlive)
        {
            if(enemy.isDead)
            {
                enemiesToRemove.Add(enemy);
            }
        }

        // Actually remove all dead enemies
        foreach(Enemy enemy in enemiesToRemove)
        {
            currentEnemiesAlive.Remove(enemy);
        }

        enemiesToRemove.Clear();

        // Start Cooldown if all enemies are dead
        if (currentEnemiesAlive.Count == 0 && inCooldown == false && currentWave < maxWave)
        {
            // Start cooldown for next wave
            StartCoroutine(WaveCooldown());
        }
        else if(currentEnemiesAlive.Count == 0 && currentWave >= maxWave )
        {
            SoundManager.Instance.playerChannel.PlayOneShot(SoundManager.Instance.gameFinishedMusic);

            GlobalReferences.Instance.scoreNumber += maxWave * 100;

            int scoreFinal = GlobalReferences.Instance.scoreNumber;

            if(scoreFinal > SaveLoadManager.Instance.LoadHighScore())
            {
                SaveLoadManager.Instance.SaveHighScore(scoreFinal);
            }

            TaskManager.Instance.Level4End();
        }

        // Run the cooldown counter
        if (inCooldown)
        {
            cooldownCounter -= Time.deltaTime;
        }
        else
        {   
            // Reset the counter
            cooldownCounter = waveCooldown;
        }

        cooldownCounterUI.text = cooldownCounter.ToString("F0");
    }

    private IEnumerator WaveCooldown()
    {
        inCooldown = true;

        waveOverUI.gameObject.SetActive(true);

        yield return new WaitForSeconds(waveCooldown);

        inCooldown = false;

        waveOverUI.gameObject.SetActive(false);

        currentEnemiesPerWave *= enemiesPerWaveIncrement;
        StartNextWave();
    }
}
