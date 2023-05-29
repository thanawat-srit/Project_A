using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicDoNotDestroy : MonoBehaviour
{
    void Awake(){
        GameObject[] musicObj = GameObject.FindGameObjectsWithTag("BGMusic");
        if(musicObj.Length>1){
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(this.gameObject);
    }
}
