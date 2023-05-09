using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PvpButton : MonoBehaviour
{
    public GameObject plateStop;
    public GameObject gameObj;

    public void ButtonStop(){
        Game game = gameObj.GetComponent<Game>();
        List<GameObject> blues = game.GetPlayBlueList();
        List<GameObject> reds = game.GetPlayRedList();
        foreach(GameObject chessman in blues){
            SetActiveFalse(chessman);
        }
        foreach(GameObject chessman in reds){
            SetActiveFalse(chessman);
        }
        plateStop.SetActive(true);
    }

    void SetActiveFalse(GameObject chessman){
        chessman.GetComponent<Collider2D>().enabled = false;
    }
    void SetActiveTrue(GameObject chessman){
        chessman.GetComponent<Collider2D>().enabled = true;
    }

    public void ButtonBackToGame(){
        Game game = gameObj.GetComponent<Game>();
        List<GameObject> blues = game.GetPlayBlueList();
        List<GameObject> reds = game.GetPlayRedList();
        foreach(GameObject chessman in blues){
            SetActiveTrue(chessman);
        }
        foreach(GameObject chessman in reds){
            SetActiveTrue(chessman);
        }
        plateStop.SetActive(false);
    }

    public void ButtonExitGame(){
        SceneManager.LoadScene("GameModeScene");
    }
}
