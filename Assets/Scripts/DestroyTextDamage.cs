using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyTextDamage : MonoBehaviour
{
    public float timeDestroy = 1f;
    void Start()
    {
        Destroy(gameObject, timeDestroy);
    }


}
