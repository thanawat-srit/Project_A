using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuButton : MonoBehaviour
{
    public void ButtonPlay(){
        SceneManager.LoadScene(1);
    }
    public void ButtonExit(){
        Application.Quit();
    }
}
