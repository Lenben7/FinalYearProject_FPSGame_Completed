using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public static Player Instance {get; set;}
    [Header("血量设置")]
    public float maxHealth = 100f;
    public float currentHealth;

    [Header("UI 引用")]
    public Image healthboundImage;
    public Image healthFillImage;
    public GameObject bloodyScreen;
    public TextMeshProUGUI playerHealthUI;
    public GameObject gameOverUI;
    public TextMeshProUGUI gameOverScore;

    [Header("玩家状态")]
    public bool isDead;

    [Header("颜色设置")]
    public Color healthyColor = Color.green;
    public Color warningColor = Color.yellow;
    public Color dangerColor = Color.red;

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        currentHealth = maxHealth;

        GetComponent<ScreenBlackout>().StartFadeIn();

        // playerHealthUI.text = $"Health: {currentHealth}"; 数字UI，测试用
    }
    private void Update()
    {
        UpdateColor();

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    void UpdateColor()
    {
        if (healthFillImage == null) return;
        
        float percent = currentHealth / maxHealth;
        
        if (percent > 0.6f)
            healthFillImage.color = healthyColor;
        else if (percent > 0.3f)
            healthFillImage.color = warningColor;
        else
            healthFillImage.color = dangerColor;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("EnemyHand"))
        {
            if(isDead == false)
            {
                TakeDamage(other.gameObject.GetComponent<OrdinarySoldierHand>().damage);
            }
        }
    }

    public void TakeDamage(int damageAmount)
    {
        // currentHealth -= damageAmount; 数字UI，测试用

        currentHealth = Mathf.Clamp(currentHealth - damageAmount, 0, maxHealth);

        if(currentHealth <= 0)
        {
            print("Player Dead");
            PlayerDead();
            GameObject.Find("PlayMenu").SetActive(false);
            healthFillImage.fillAmount = 0;
            isDead = true;
        }
        else
        {
            print("Player Hit");
            StartCoroutine(BloodyScreenEffect());
            healthFillImage.fillAmount = currentHealth / maxHealth;

            // playerHealthUI.text = $"Health: {currentHealth}"; 数字UI, 测试用

            SoundManager.Instance.playerChannel.PlayOneShot(SoundManager.Instance.playerHurt);
        } 

        UpdateColor();
    }

    private IEnumerator BloodyScreenEffect()
    {
        if(bloodyScreen.activeInHierarchy == false)
        {
            bloodyScreen.SetActive(true);
        }

        var image = bloodyScreen.GetComponentInChildren<Image>();

        // Set the initial alpha value to 1 (fully visible).
        Color startColor = image.color;
        startColor.a = 1f;
        image.color = startColor;

        float duration = 2f;
        float elapsedTime = 0f;

        while(elapsedTime < duration)
        {
            // Calculate the new alpha value using Lerp;
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / duration);

            // Update the color with the new alpha value.
            Color newColor = image.color;
            newColor.a = alpha;
            image.color = newColor;

            // Increment the elapsed time
            elapsedTime += Time.deltaTime;

            yield return null; // Wait for the next frame;
        }

        if(bloodyScreen.activeInHierarchy == false)
        {
            bloodyScreen.SetActive(false);
        }
    }

    public void Heal(float amount)
   {
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
       
        if (healthFillImage != null)
            healthFillImage.fillAmount = currentHealth / maxHealth;
        SoundManager.Instance.playerChannel.PlayOneShot(SoundManager.Instance.playerHeal);
        UpdateColor();
   }

    private void PlayerDead()
    {
        SoundManager.Instance.playerChannel.PlayOneShot(SoundManager.Instance.playerDeath);

        SoundManager.Instance.playerChannel.clip = SoundManager.Instance.gameOverMusic;

        SoundManager.Instance.playerChannel.PlayDelayed(2f);

        GetComponent<MouseMovement>().enabled = false;
        GetComponent<PlayerMovemont>().enabled = false;

        // Dying Animation
        GetComponentInChildren<Animator>().enabled = true;
        playerHealthUI.gameObject.SetActive(false);

        GetComponent<ScreenBlackout>().StartFade();
        StartCoroutine(ShowGameOverUI());
    }

     private IEnumerator ShowGameOverUI()
    {
        yield return new WaitForSeconds(1f);

        gameOverUI.gameObject.SetActive(true);

        int scoreFinal = GlobalReferences.Instance.scoreNumber;

        gameOverScore.text = $"Score: {scoreFinal}";

        if(scoreFinal > SaveLoadManager.Instance.LoadHighScore())
        {
            SaveLoadManager.Instance.SaveHighScore(scoreFinal);
        }

        StartCoroutine(ReturnToMainMenu());
    }

    private IEnumerator ReturnToMainMenu()
    {
        yield return new WaitForSeconds(5F);

        SceneManager.LoadScene("MainMenu");
    }
}
