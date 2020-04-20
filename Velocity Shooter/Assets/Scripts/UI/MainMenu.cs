using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class MainMenu : MonoBehaviour
{

    [SerializeField] private string m_sceneName;
    [SerializeField] Slider m_playerSlider;

    private void StartGame()
    {
        SceneManager.LoadScene(m_sceneName);
        GameManager.m_instance.m_playersToAdd = (int) m_playerSlider.value;
    }

    private void Update()
    {
        m_playerSlider.GetComponentInChildren<TextMeshProUGUI>().text = "Number of players : " + (int)m_playerSlider.value;
    }

    private void Exit()
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
