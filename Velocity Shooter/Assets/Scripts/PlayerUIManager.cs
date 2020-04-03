using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerUIManager : MonoBehaviour
{
    [SerializeField]
    private GameObject m_healthUIPrefab;
    [SerializeField]
    private GameObject m_armorUIPrefab;
    [SerializeField]
    private GameObject m_superBulletUIPrefab;
    
    [SerializeField]
    private Transform m_uiTargetTransform;

    private List<GameObject> m_tanks;
    private List<GameObject> m_healthUI;
    private List<GameObject> m_armorUI;
    private List<GameObject> m_superBulletUI;

    private bool m_isInitialized;
    
    private int m_nbOfPlayers = 0;
    
    private void Initialize()
    {
        m_tanks = GameManager.m_instance.m_playerList;

        m_healthUI = new List<GameObject>();
        m_armorUI = new List<GameObject>();
        m_superBulletUI = new List<GameObject>();
        
        m_nbOfPlayers = m_tanks.Count;
        
        for (int i = 0; i < m_tanks.Count; ++i)
        {
            GameObject newHealth = Instantiate(m_healthUIPrefab, Vector3.zero, Quaternion.identity, m_uiTargetTransform);
            newHealth.name = "HealthP" + (i + 1);
            GameObject newArmor = Instantiate(m_armorUIPrefab, Vector3.zero, Quaternion.identity, m_uiTargetTransform);
            newArmor.name = "ArmorP" + (i + 1);
            GameObject newSuperBullet = Instantiate(m_superBulletUIPrefab, Vector3.zero, Quaternion.identity, m_uiTargetTransform);
            newSuperBullet.name = "SuperBulletP" + (i + 1);
            m_healthUI.Add(newHealth);
            m_armorUI.Add(newArmor);
            m_superBulletUI.Add(newSuperBullet);
        }
        
        switch (m_nbOfPlayers)
        {
            case 2:
                m_healthUI[0].transform.localPosition = new Vector3(-900, 220, 0); 
                m_healthUI[1].transform.localPosition = new Vector3(65, 220, 0);
                m_armorUI[0].transform.localPosition = new Vector3(-910, 130, 0); 
                m_armorUI[1].transform.localPosition = new Vector3(60, 130, 0);
                m_superBulletUI[0].transform.localPosition = new Vector3(-910, 40, 0); 
                m_superBulletUI[1].transform.localPosition = new Vector3(60, 40, 0);
                break;
            case 3:
                m_healthUI[0].transform.localPosition = new Vector3(-900, 220, 0); 
                m_healthUI[1].transform.localPosition = new Vector3(65, 220, 0); 
                m_healthUI[2].transform.localPosition = new Vector3(-410, -315, 0);
                m_armorUI[0].transform.localPosition = new Vector3(-910, 130, 0); 
                m_armorUI[1].transform.localPosition = new Vector3(60, 130, 0); 
                m_armorUI[2].transform.localPosition = new Vector3(-420, -400, 0); 
                m_superBulletUI[0].transform.localPosition = new Vector3(-910, 40, 0); 
                m_superBulletUI[1].transform.localPosition = new Vector3(60, 40, 0); 
                m_superBulletUI[2].transform.localPosition = new Vector3(-420, -490, 0); 
                break;
            case 4:
                m_healthUI[0].transform.localPosition = new Vector3(-900, 220, 0); 
                m_healthUI[1].transform.localPosition = new Vector3(65, 220, 0); 
                m_healthUI[2].transform.localPosition = new Vector3(-900, -315, 0); 
                m_healthUI[3].transform.localPosition = new Vector3(65,-315, 0);
                m_armorUI[0].transform.localPosition = new Vector3(-910, 130, 0); 
                m_armorUI[1].transform.localPosition = new Vector3(60, 130, 0); 
                m_armorUI[2].transform.localPosition = new Vector3(-910, -400, 0); 
                m_armorUI[3].transform.localPosition = new Vector3(60,-400, 0);
                m_superBulletUI[0].transform.localPosition = new Vector3(-910, 40, 0); 
                m_superBulletUI[1].transform.localPosition = new Vector3(60, 40, 0); 
                m_superBulletUI[2].transform.localPosition = new Vector3(-910, -490, 0); 
                m_superBulletUI[3].transform.localPosition = new Vector3(60,-490, 0);
                break;
        }
    }

    void UpdateUI()
    {
        for (int i = 0; i < m_nbOfPlayers; ++i)
        {
//            m_healthUI[i].GetComponentInChildren<Image>();
        }
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
            Initialize();
        
        if (!m_isInitialized)
            return;
        
        UpdateUI();
    }
}
