using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameModeButton : MonoBehaviour
{
    public int energyRank = 0;

    public GameObject plateAds;
   
    public void ButtonStoryMode(){

    }
    public void ButtonPvpMode(){
        SceneManager.LoadScene("PvP");
    }
    public void ButtonRankMode(){
        if(energyRank>0){
            SceneManager.LoadScene("PvP");
            energyRank--;
        }else{
            plateAds.SetActive(true);
        }
    }
    public void AdsToEnergy(){
        
    }
    public void ButtonBackToMainMenu(){
        SceneManager.LoadScene("MainMenuScene");
    }
    public void ButtonExitPlateAds(){
        plateAds.SetActive(false);
    }

    public void SetEnergrRank(int en){
        energyRank = en;
    } 
}
