using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour
{
    public void OnStartGame()
    {
        SceneManager.LoadScene("miniGame");
    }

    public void OnResetGame()
    {
        SceneManager.LoadScene("miniGame");
        Time.timeScale = 1;
    }

    public void OnExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
#endif
        Application.Quit();
    }
}
