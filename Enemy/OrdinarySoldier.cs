using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrdinarySoldier : MonoBehaviour
{
    public OrdinarySoldierHand ordinarySoldierHand;

    public int ordinarySoldierDamage;

    private void Start() 
    {
        ordinarySoldierHand.damage = ordinarySoldierDamage;    
    }
}
