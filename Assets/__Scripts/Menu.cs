using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    void Start()
    {
    }

    void Update()
    {
    }

    public void BtnPlay() { SceneManager.LoadScene(1); }

    // Wy³¹cza grê
    public void BtnQuit() { Application.Quit(); }
}
