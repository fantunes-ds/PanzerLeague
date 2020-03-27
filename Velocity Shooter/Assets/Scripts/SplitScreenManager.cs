using System.Collections.Generic;
using UnityEngine;

public class SplitScreenManager : MonoBehaviour
{
    public static SplitScreenManager m_instance;

    [SerializeField] 
    private GameObject m_cameraPrefab;
    private List<GameObject> m_splitCameras;

    private int m_currentNumberOfCameras;

    private GameObject m_cameraContainer;

    void Start()
    {
        if (m_instance == null)
        {
            m_instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (m_instance != null)
            Destroy(gameObject);

        m_splitCameras = new List<GameObject>();

        if (!GameManager.m_instance.GetScene().name.Equals("Main Menu"))
            UpdateSplitScreen(m_currentNumberOfCameras);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
            UpdateSplitScreen(++m_currentNumberOfCameras);
    }

    private void UpdateSplitScreen(int p_numberOfScreens)
    {
        switch (p_numberOfScreens)
        {
            case 1:
                CheckCameraAvailability(p_numberOfScreens);
                m_splitCameras[0].GetComponentInChildren<Camera>().rect = new Rect(0, 0, 1, 1);
                break;
            case 2:
                CheckCameraAvailability(p_numberOfScreens);
                m_splitCameras[0].GetComponentInChildren<Camera>().rect = new Rect(0f, 0, 0.499f, 1);
                m_splitCameras[1].GetComponentInChildren<Camera>().rect = new Rect(0.5f, 0, 0.5f, 1);
                break;
            case 3:
                CheckCameraAvailability(p_numberOfScreens);
                m_splitCameras[0].GetComponentInChildren<Camera>().rect = new Rect(0f, 0.5f, 0.499f, 0.5f);
                m_splitCameras[1].GetComponentInChildren<Camera>().rect = new Rect(0.5f, 0.5f, 0.5f, 0.5f);
                m_splitCameras[2].GetComponentInChildren<Camera>().rect = new Rect(0.25f, 0, 0.5f, 0.5f);
                break;
            case 4:
                CheckCameraAvailability(p_numberOfScreens);
                m_splitCameras[0].GetComponentInChildren<Camera>().rect = new Rect(0f, 0.5f, 0.499f, 0.5f);
                m_splitCameras[1].GetComponentInChildren<Camera>().rect = new Rect(0.5f, 0.5f, 0.5f, 0.5f);
                m_splitCameras[2].GetComponentInChildren<Camera>().rect = new Rect(0, 0, 0.499f, 0.495f);
                m_splitCameras[3].GetComponentInChildren<Camera>().rect = new Rect(0.5f, 0, 0.5f, 0.495f);
                break;
            default:
                Debug.Log("[SPLIT SCREEN] Format not supported : " + p_numberOfScreens + " screens is not acceptable");
                break;
        }
    }

    void CheckCameraAvailability(int p_numberOfCameras)
    {
        if(GameManager.m_instance == null)
            Debug.Log("GameManager was not set !");
        
        if (m_splitCameras.Count < p_numberOfCameras)
        {
            for (int i = m_splitCameras.Count; i < p_numberOfCameras; ++i)
            {
                if (m_cameraContainer == null)
                {
                    m_cameraContainer = new GameObject("Cameras");
                    m_cameraContainer.transform.parent = transform;
                }
                
                GameObject newCamera = Instantiate(m_cameraPrefab, Vector3.zero, Quaternion.identity, m_cameraContainer.transform);
                newCamera.name = "P" + (GameManager.m_instance.m_playerList.Count + 1);
                GameObject newPlayer = GameManager.m_instance.AddPlayer(GameManager.m_instance.m_playerList.Count);
                newCamera.GetComponent<CameraFollow>().m_cameraTarget = newPlayer.GetComponent<TankController>().m_cameraTarget;
                newCamera.GetComponent<CameraFollow>().m_zoomTarget = newPlayer.GetComponent<TankController>().m_zoomTarget;
                newPlayer.GetComponent<TankController>().m_tankCamera = newCamera;
                newPlayer.GetComponent<ProjectilePrediction>().m_tankCamera = newCamera.GetComponentInChildren<Camera>();
                m_splitCameras.Add(newCamera);
            }
        }
        else
            for (int i = p_numberOfCameras; i < m_splitCameras.Count - 1; i++)
                m_splitCameras[i].gameObject.SetActive(false);
    }

    public void Refresh()
    {
        UpdateSplitScreen(m_currentNumberOfCameras);
    }

    public void SetCameraNumber(int p_nbOfCameras)
    {
        m_currentNumberOfCameras = p_nbOfCameras;
    }
}

