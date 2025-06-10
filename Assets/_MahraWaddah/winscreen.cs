using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class winscreenScript : MonoBehaviour
{
    public GameObject WinscreenObj;



    public void RestartGame()
    {
        Debug.Log("Restart");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        WinscreenObj.SetActive(false);
    }
    
    public void ShowWinScreen(){
        WinscreenObj.SetActive(true);
    }
}