using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestroy : MonoBehaviour
{
    public float timeForDestrution;

   private IEnumerator DestroySelf(float timeForDestrution)
    {
        yield return new WaitForSeconds(timeForDestrution);

        Destroy(gameObject);
    }
}
