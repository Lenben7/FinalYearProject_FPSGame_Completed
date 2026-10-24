using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Weapon;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; set; }

    [Header("ShootingChannel")]
    public AudioSource ShootingChannel;
   
    public AudioClip P1911Shot;
    public AudioClip M16A4Shot;
    public AudioClip MP5Shot;

    [Header("ReloadingChannel")]
    public AudioSource reloadingChannel;

    public AudioClip P1911Reloading;
    public AudioClip M16A4Reloading;
    public AudioClip MP5Rloading;

    [Header("EmptyManagizeChannel")]
    public AudioSource emptyManagizeSound;


    [Header("ThrowablesChannel")]
    public AudioSource throwablesChannel;

    public AudioClip grenadeSound;
    public AudioClip smokegrenadeSound;

    [Header("EnemyChannel")]
    public AudioSource enemyChannel;
    public AudioSource enemyChannel2;

    public AudioClip enemyWalking;
    public AudioClip enemyChase;
    public AudioClip enemyAttack;
    public AudioClip enemyHurt;
    public AudioClip enemyDeath;
    public AudioClip enemyTalk;
    
    [Header("PlayerChannel")]
    public AudioSource playerChannel;
    public AudioSource playerChannel2;
    public AudioClip playerHeal;
    public AudioClip playerHurt;
    public AudioClip playerDeath;
    public AudioClip playerMoving;
    public AudioClip playerRuning;
    public AudioClip playerJump;
    public AudioClip gameOverMusic;
    public AudioClip gameFinishedMusic;

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

    #region || ---- Play the firing sound effects of different weapons ---- ||
    public void PlayShootingSound(WeaponModel weapon)
    {
        switch (weapon)
        {
            case WeaponModel.Pistol1911:
                ShootingChannel.PlayOneShot(P1911Shot);
                break;
            case WeaponModel.M16A4:
                ShootingChannel.PlayOneShot(M16A4Shot);
                break;
            case WeaponModel.MP5:
                ShootingChannel.PlayOneShot(MP5Shot);
                break;
        }
    }
    #endregion || ---- Play the firing sound effects of different weapons ---- ||

    #region || ---- Play the reloading sound effects of different weapons ---- ||
    public void PlayReloadSound(WeaponModel weapon)
    {
        switch (weapon)
        {
            case WeaponModel.Pistol1911:
                reloadingChannel.PlayOneShot(P1911Reloading);
                break;
            case WeaponModel.M16A4:
                reloadingChannel.PlayOneShot(M16A4Reloading);
                break;
            case WeaponModel.MP5:
                ShootingChannel.PlayOneShot(MP5Rloading);
                break;
        }
    }
     #endregion || ---- Play the reloading sound effects of different weapons ---- ||
}