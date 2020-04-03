using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{    

    public static GameManager m_instance;

    [SerializeField] 
    private GameObject[] m_playerPrefabs;
    [SerializeField] 
    private GameObject m_uiTargetPrefab;
    public List<GameObject> m_playerList { private set; get; }
    private GameObject m_playerContainer;
    public Transform m_uiTargetContainer;
    public List<Transform> m_spawnPoints;

    public int m_numberOfInstancedPlayers { private set; get; }

    public int m_playersToAdd;
    public bool m_isReadyToAddPlayers { private set; get; }
    
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

        if (GetScene().name != "MainMenu" && m_spawnPoints.Count == 0)
        {
            m_uiTargetContainer = GameObject.Find("Canvas").transform;
            for (int i = 0; i < GameObject.Find("SpawnPoints").transform.GetChildCount() - 1; ++i)
            {
                m_spawnPoints.Add(GameObject.Find("SpawnPoints").transform.GetChild(i).transform);
            }

            m_isReadyToAddPlayers = true;
        }
    }

    public GameObject AddPlayer(int tankIndex)
    {
        if (m_playerContainer == null)
        {
            m_playerContainer = new GameObject("Players");
            m_playerContainer.transform.parent = transform;
        }

        if (m_playerContainer == null)
            return new GameObject("ERROR GAME MANAGER ADD PLAYER");

        GameObject newPlayer = Instantiate(m_playerPrefabs[tankIndex], m_spawnPoints[m_playerList.Count].position, Quaternion.identity, m_playerContainer.transform);
        GameObject newUiTarget = Instantiate(m_uiTargetPrefab, Vector3.zero, Quaternion.identity, m_uiTargetContainer);
        newPlayer.GetComponent<ProjectilePrediction>().m_uiTarget = newUiTarget;
        
        newPlayer.name = "P" + (m_playerList.Count + 1);
        m_playerList.Add(newPlayer);

        m_numberOfInstancedPlayers++;
        
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
