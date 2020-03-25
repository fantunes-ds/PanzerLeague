using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{    

    public static GameManager m_instance;

    [SerializeField] 
    private GameObject[] m_playerPrefabs;
    public List<GameObject> m_playerList { private set; get; }
    private GameObject m_playerContainer;
    [SerializeField]
    private Transform[] m_spawnPoints;

    private void Awake()
    {
        if (m_instance == null)
        {
            m_instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (m_instance != null)
            Destroy(gameObject);
        m_playerList = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
    }

    public GameObject AddPlayer(int tankIndex)
    {
        if (m_playerContainer == null)
            m_playerContainer = new GameObject("Players");

        if (m_playerContainer == null)
            return new GameObject("ERROR GAME MANAGER ADD PLAYER");

        GameObject newPlayer = Instantiate(m_playerPrefabs[tankIndex], m_spawnPoints[m_playerList.Count].position, Quaternion.identity, m_playerContainer.transform);

        newPlayer.name = "P" + (m_playerList.Count + 1);
        m_playerList.Add(newPlayer);


        return newPlayer;
    }

    public Scene GetScene()
    {
        return SceneManager.GetActiveScene();
    }

    public GameObject GetPlayerFromList(int p_index)
    {
        return m_playerList[p_index];
    }
}
