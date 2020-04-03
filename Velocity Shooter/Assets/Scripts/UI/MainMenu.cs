using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class MainMenu : MonoBehaviour
{
    
    [SerializeField]
    private string m_sceneName;
    [SerializeField]
    Slider m_playerSlider;

    public void StartGame()
    {
        SceneManager.LoadScene(m_sceneName);
        GameManager.m_instance.m_playersToAdd = (int)m_playerSlider.value;
    }

    public void Exit()
    {
        if (Application.isPlaying)
        {
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }
    }
}
