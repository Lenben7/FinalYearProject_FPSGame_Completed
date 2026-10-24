using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class InteractionManager : MonoBehaviour
{
    public static InteractionManager Instance {get; set;}

    public Weapon hoveredWeapon = null;
    public Ammobox hoveredAmmoBox = null;
    public Throwable hoveredThrowable = null;
    public HealthBox hoveredHealthBox = null;

    public float pickupRange = 10f;

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

    private void Update()
    {
        Ray ray= Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if(Physics.Raycast(ray, out hit, pickupRange))
        {
            GameObject obejctHitByRaycast = hit.transform.gameObject;

            #region || ---- Weapon PickUp ---- ||
            if (obejctHitByRaycast.GetComponent<Weapon>() && obejctHitByRaycast.GetComponent<Weapon>().isActiveWeapon == false)
            {
                // Disable the outline of previously selected item
                if (hoveredWeapon)
                {
                    hoveredWeapon.GetComponent<Outline>().enabled = false;
                }

                hoveredWeapon = obejctHitByRaycast.gameObject.GetComponent<Weapon>();
                hoveredWeapon.GetComponent<Outline>().enabled = true;

                if (Input.GetKeyDown(KeyCode.F))
                {
                    WeaponManager.Instance.PickupWeapon(obejctHitByRaycast.gameObject);
                }
            }
            else
            {
                if (hoveredWeapon)
                {
                    hoveredWeapon.GetComponent<Outline>().enabled = false;
                }
            }
            #endregion || ---- Weapon PickUp ---- ||

            #region || ---- Ammobox PickUp ---- ||
            if (obejctHitByRaycast.GetComponent<Ammobox>())
            {
                // Disable the outline of previously selected item
                if (hoveredAmmoBox)
                {
                    hoveredAmmoBox.GetComponent<Outline>().enabled = false;
                }

                hoveredAmmoBox = obejctHitByRaycast.gameObject.GetComponent<Ammobox>();
                hoveredAmmoBox.GetComponent<Outline>().enabled = true;

                if (Input.GetKeyDown(KeyCode.F))
                {
                    WeaponManager.Instance.PickupAmmo(hoveredAmmoBox);
                    Destroy(obejctHitByRaycast.gameObject);
                }
            }
            else
            {
                if (hoveredAmmoBox)
                {
                    hoveredAmmoBox.GetComponent<Outline>().enabled = false;
                }
            }
            #endregion || ---- Ammobox PickUp ---- ||

            #region || ---- HealthStone PickUp ---- ||
            if (obejctHitByRaycast.GetComponent<HealthBox>())
            {
                // Disable the outline of previously selected item
                if (hoveredHealthBox)
                {
                    hoveredHealthBox.GetComponent<Outline>().enabled = false;
                }

                hoveredHealthBox = obejctHitByRaycast.gameObject.GetComponent<HealthBox>();
                hoveredHealthBox.GetComponent<Outline>().enabled = true;

                if (Input.GetKeyDown(KeyCode.F))
                {
                    Player.Instance.Heal(hoveredHealthBox.healthaMount);
                    Destroy(obejctHitByRaycast.gameObject);
                }
            }
            else
            {
                if (hoveredHealthBox)
                {
                    hoveredHealthBox.GetComponent<Outline>().enabled = false;
                }
            }
            #endregion || ---- HealthStone PickUp ---- ||

            #region || ---- Throwable PickUp ---- ||
            if (obejctHitByRaycast.GetComponent<Throwable>())
            {
                // Disable the outline of previously selected item
                if (hoveredThrowable)
                {
                    hoveredThrowable.GetComponent<Outline>().enabled = false;
                }
                
                hoveredThrowable = obejctHitByRaycast.gameObject.GetComponent<Throwable>();
                hoveredThrowable.GetComponent<Outline>().enabled = true;

                if (Input.GetKeyDown(KeyCode.F))
                {
                    WeaponManager.Instance.PickupThrowable(hoveredThrowable);
                }
            }
            else
            {
                if (hoveredThrowable)
                {
                    hoveredThrowable.GetComponent<Outline>().enabled = false;
                }
            }
            #endregion || ---- Throwable PickUp ---- ||
        }
        else
        {
            // 超出距离时清除所有高亮
            ClearAllOutlines();
        }
    }

    // 辅助方法：清除所有高亮
    private void ClearAllOutlines()
    {
        if (hoveredWeapon) { hoveredWeapon.GetComponent<Outline>().enabled = false; hoveredWeapon = null; }
        if (hoveredAmmoBox) { hoveredAmmoBox.GetComponent<Outline>().enabled = false; hoveredAmmoBox = null; }
        if (hoveredHealthBox) { hoveredHealthBox.GetComponent<Outline>().enabled = false; hoveredHealthBox = null; }
        if (hoveredThrowable) { hoveredThrowable.GetComponent<Outline>().enabled = false; hoveredThrowable = null; }
    }
}