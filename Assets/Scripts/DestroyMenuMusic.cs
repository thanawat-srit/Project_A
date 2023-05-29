using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyMenuMusic : MonoBehaviour
{
    void Awake(){
        GameObject[] musicObj = GameObject.FindGameObjectsWithTag("BGMusic");
        Destroy(musicObj[0]);
    }
}
