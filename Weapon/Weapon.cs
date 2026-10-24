using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Header("Status")]
    public bool isActiveWeapon;
    [Header("WeaponDamage")]
    public int weaponDamage;

    // Shooting
    // 开火
    [Header("Shooting")]
    public bool isShooting, readyToShoot;
    bool allowReset = true;
    public float shootingDelay = 2f;

    // Burst
    // 连发
    [Header("Burst")]
    public int bulletsPerBurst = 3;
    public int burstBulletsLeft;

    // Spread
    // 射程
    [Header("Spread")]
    public float spreadIntensity;
    public float hipSpreadIntensity;
    public float adsSpreadIntensity;

    // Bullet
    // 子弹
    [Header("Bullet")]
    public GameObject bulletPrefab;
    public Transform bulletSpawn;
    public float bulletVelocity = 30f;
    public float bulletPrefabLifeTime = 3f; // seconds (时间单位：秒)

    // effect
    //射击特效
    public GameObject muzzleEffect;
    //animator
    //武器动画机
    internal Animator animator;

    // Loading
    // 装弹
    [Header("Loading")]
    public float reloadTime;
    public int magazineSize, bulletsLeft;
    public bool isReloading;

    [Header("WeaponSlot")]
    public Vector3 spawnPosition;
    public Vector3 spawnRotation;

    //ADS status
    //瞄准状态
    bool isADS;

    //Weapon Type
    public enum WeaponModel
    {
        Pistol1911,
        M16A4,
        MP5
    }

    public WeaponModel thisWeaponModel;

    //Shooting Type
    public enum ShootingMode
    {
        Single,
        Burst,
        Auto
    }

    public ShootingMode currentShootingMode;

    //Preset Mode
    private void Awake()
    {
        readyToShoot = true;
        burstBulletsLeft = bulletsPerBurst;
        animator = GetComponent<Animator>();

        bulletsLeft = magazineSize;

        spreadIntensity = hipSpreadIntensity;
    }

    void Update()
    {
        if (isActiveWeapon)
        {
            Transform[] allTransform = GetComponentsInChildren<Transform>(true);
            foreach (Transform child in allTransform)
            {
                child.gameObject.layer = LayerMask.NameToLayer("WeaponRender");
            }

            #region || ---- ADS Behavior ---- ||
            if (Input.GetMouseButtonDown(1))
            {
                EnterADS();
            }

            if(Input.GetMouseButtonUp(1))
            {
                ExitADS();
            }
            #endregion || ---- ADS Behavior ---- ||

            //Outline check
            GetComponent<Outline>().enabled = false;
            
            #region || ---- emptyShootingSound ---- ||
            if(bulletsLeft == 0 && isShooting)
            {
                SoundManager.Instance.emptyManagizeSound.Play();
            }
            #endregion || ---- emptyShootingSound ---- ||

            #region || ---- ShootingModel Behavior ---- ||
            if(currentShootingMode == ShootingMode.Auto)
            {
                // Holding Down Left Mouse Button
                // 按住鼠标左键连续发射
                isShooting = Input.GetKey(KeyCode.Mouse0);
            }
            else if(currentShootingMode == ShootingMode.Single || 
                    currentShootingMode ==ShootingMode.Burst)
            {
                // Clicking Left Mouse Once
                // 单击鼠标左键发射
                isShooting = Input.GetKeyDown(KeyCode.Mouse0);
            }
            #endregion || ---- ShootingModel Behavior ---- ||

            #region || ---- Reload Behavior ---- ||
            if(Input.GetKey(KeyCode.R) && bulletsLeft < magazineSize && 
            !isReloading && WeaponManager.Instance.CheckAmmoLeftFor(thisWeaponModel) > 0)
            {
                Reload();
            }

            // If you want to automatically reload when magazine is empty
            // 弹匣为空自动装填
            if(readyToShoot && !isShooting && !isReloading && bulletsLeft <= 0)
            {
                // Reload();
            }
            #endregion || ---- Reload Behavior ---- ||

            #region || ---- Confirm fire ---- ||
            if(readyToShoot && isShooting && bulletsLeft > 0)
            {
                burstBulletsLeft = bulletsPerBurst;
                FireWeapon();
            }
            #endregion || ---- Confirm fire ---- ||
        }
        else
        {
            Transform[] allTransform = GetComponentsInChildren<Transform>(true);
            foreach (Transform child in allTransform)
            {
                child.gameObject.layer = LayerMask.NameToLayer("Default");
            }
        }
    }

    #region || ---- ADS Logic ---- ||
    private void EnterADS() 
    {
        animator.SetTrigger("enterADS");
        isADS = true;
        HUDManager.Instance.middleDot.SetActive(false);
        spreadIntensity = adsSpreadIntensity;
    }

     private void ExitADS() 
    {
        animator.SetTrigger("exitADS");
        isADS = false;
        HUDManager.Instance.middleDot.SetActive(true);
        spreadIntensity = hipSpreadIntensity;
    }
    #endregion || ---- ADS Logic ---- ||

    #region || ---- Fire Logic ---- ||
    private void FireWeapon()
    {
        bulletsLeft--;

        muzzleEffect.GetComponent<ParticleSystem>().Play();

        if (isADS)
        {
            animator.SetTrigger("RECOIL_ADS");
        }
        else
        {
            animator.SetTrigger("RECOIL");
        }
        
        SoundManager.Instance.PlayShootingSound(thisWeaponModel);

        readyToShoot = false;

        Vector3 shootingDirection = CalculateDirectionAndSpreed().normalized;

        // Instantiate the bullet
        // 实例化弹药
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, Quaternion.identity);

        Bullet bul = bullet.GetComponent<Bullet>();
        bul.bulletDamage = weaponDamage;

        // Poting the bullet to face the shooting direction
        // 将子弹面向射击方向
        bullet.transform.forward = shootingDirection;

        // Shoot the bullet
        // 发射弹药
        bullet.GetComponent<Rigidbody>().AddForce(shootingDirection * bulletVelocity, ForceMode.Impulse);

        // Destroy the bullet after some time
        // 稍后摧毁弹药
        StartCoroutine(DestroyBulletAfterTime(bullet, bulletPrefabLifeTime));

        // Checking if we are done shooting
        // 检测是否射击完成
        if (allowReset)
        {
            Invoke("ResetShot", shootingDelay);
            allowReset = false;
        }

        // Burst Mode
        // 连发模式
        if(currentShootingMode == ShootingMode.Burst && burstBulletsLeft > 1)// we already shoot once before 之前已经发射一次了
        {
            burstBulletsLeft--;
            Invoke("FireWeapon", shootingDelay);
        }
    }

        private void ResetShot() 
    {
        readyToShoot = true;
        allowReset = true;
    }
    #endregion || ---- Fire Logic ---- ||

    #region || ---- Reload Logic ---- ||
    private void Reload() 
    {
        SoundManager.Instance.PlayReloadSound(thisWeaponModel);
        animator.SetTrigger("RELOAD");

        isReloading = true;
        Invoke("ReloadCompleted", reloadTime);
    }

    private void ReloadCompleted()
    {
        int bulletsNeeded = magazineSize - bulletsLeft;
        int availableAmmo = WeaponManager.Instance.CheckAmmoLeftFor(thisWeaponModel);
        int bulletsToReload = Math.Min(bulletsNeeded, availableAmmo);

        bulletsLeft += bulletsToReload;

        WeaponManager.Instance.DecreaseTotalAmmo(bulletsToReload, thisWeaponModel);

        isReloading = false;
    }
    #endregion || ---- Reload Logic ---- ||

    #region || ---- Bullet Logic ---- ||
    public Vector3 CalculateDirectionAndSpreed() 
    {
        // Shooting from the middle of the screen to check where are we pointing at
        // 从屏幕中间射击以检查我们指向的位置(整体为射线检测)
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        Vector3 targetPoint;
        if(Physics.Raycast(ray, out hit))
        {
            // Hit Something
            // 击中某物
            targetPoint = hit.point;
        }
        else
        {
            // Shooting at the air
            targetPoint = ray.GetPoint(100);
        }

        Vector3 direction = targetPoint - bulletSpawn.position;

        float z = UnityEngine.Random.Range(-spreadIntensity, spreadIntensity);
        float y = UnityEngine.Random.Range(-spreadIntensity, spreadIntensity);

        // Returning the shooting direction and spread
        return direction+new Vector3(0, y, z);
    }

    private IEnumerator DestroyBulletAfterTime(GameObject bullet, float bulletPrefabLifeTime)
    {
        yield return new WaitForSeconds(bulletPrefabLifeTime);
        Destroy(bullet);
    }
    #endregion || ---- Bullet Logic ---- ||
}