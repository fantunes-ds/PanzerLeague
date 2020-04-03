using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    private List<TextMeshProUGUI> m_scoreText;

    [SerializeField]
    private TextMeshProUGUI m_prefab;
    
    private Transform m_uiTargetTransform;
    [SerializeField]
    private List<Color> m_textColors;

    private List<int> m_scores;

    private bool m_isInitialized;

    private int m_nbOfPlayers = 0;
    private int m_nbOfPointsToWin = 10;

    [SerializeField]
    private GameObject m_victoryText;

    public void Initialize()
    {
        m_uiTargetTransform = GameManager.m_instance.m_uiTargetContainer;

        m_scores = new List<int>();
        m_scoreText = new List<TextMeshProUGUI>();

        m_nbOfPlayers = GameManager.m_instance.m_numberOfInstancedPlayers;
        
        
        for (int i = 0; i < m_nbOfPlayers; ++i)
        {
            TextMeshProUGUI newText = Instantiate(m_prefab, Vector3.zero, Quaternion.identity, m_uiTargetTransform);
            
            newText.color = m_textColors[i];
            newText.name = "ScoreP" + (i + 1);
            newText.fontSize = 36;

            for (int j = 0; j < m_nbOfPlayers; ++j)
            {
                if (j == i)
                {
                    m_scoreText.Add(newText);
                    continue;
                }         

                TextMeshProUGUI newText2 = Instantiate(m_prefab, Vector3.zero, Quaternion.identity, m_uiTargetTransform);
                newText2.color = m_textColors[(j % 4)];
                newText2.name = "ScoreP" + (i+1) + " (others" + (j + 1 % 4) + ")";
                newText2.fontSize = 25;
                m_scoreText.Add(newText2);
            }

            m_scores.Add(0);
        }

        switch (m_nbOfPlayers)
        {
            case 2:
                m_scoreText[0].transform.localPosition = new Vector3(-860, 490, 0); 
                m_scoreText[1].transform.localPosition = new Vector3(-80, -490, 0); 
                m_scoreText[m_nbOfPlayers + 1].transform.localPosition = new Vector3(100, 490, 0); 
                m_scoreText[m_nbOfPlayers + 0].transform.localPosition = new Vector3(860, -490, 0); 
                break;
            case 3:
                m_scoreText[(0 * m_nbOfPlayers) + 0].transform.localPosition = new Vector3(-860, 490, 0); 
                m_scoreText[(0 * m_nbOfPlayers) + 1].transform.localPosition = new Vector3(-80, 80, 0); 
                m_scoreText[(0 * m_nbOfPlayers) + 2].transform.localPosition = new Vector3(-80, 55, 0); 
                m_scoreText[(1 * m_nbOfPlayers) + 1].transform.localPosition = new Vector3(100, 490, 0); 
                m_scoreText[(1 * m_nbOfPlayers) + 0].transform.localPosition = new Vector3(860, 80, 0); 
                m_scoreText[(1 * m_nbOfPlayers) + 2].transform.localPosition = new Vector3(880, 55, 0); 
                m_scoreText[(2 * m_nbOfPlayers) + 2].transform.localPosition = new Vector3(-380, -55, 0); 
                m_scoreText[(2 * m_nbOfPlayers) + 1].transform.localPosition = new Vector3(400, -495, 0); 
                m_scoreText[(2 * m_nbOfPlayers) + 0].transform.localPosition = new Vector3(400, -470, 0); 
                break;
            case 4:
                m_scoreText[(0 * m_nbOfPlayers) + 0].transform.localPosition = new Vector3(-860, 490, 0); 
                m_scoreText[(0 * m_nbOfPlayers) + 1].transform.localPosition = new Vector3(-80, 105, 0); 
                m_scoreText[(0 * m_nbOfPlayers) + 2].transform.localPosition = new Vector3(-80, 80, 0); 
                m_scoreText[(0 * m_nbOfPlayers) + 3].transform.localPosition = new Vector3(-80, 55, 0); 
                m_scoreText[(1 * m_nbOfPlayers) + 1].transform.localPosition = new Vector3(100, 490, 0); 
                m_scoreText[(1 * m_nbOfPlayers) + 0].transform.localPosition = new Vector3(880, 105, 0); 
                m_scoreText[(1 * m_nbOfPlayers) + 2].transform.localPosition = new Vector3(880, 80, 0); 
                m_scoreText[(1 * m_nbOfPlayers) + 3].transform.localPosition = new Vector3(880, 55, 0); 
                m_scoreText[(2 * m_nbOfPlayers) + 2].transform.localPosition = new Vector3(-860, -55, 0); 
                m_scoreText[(2 * m_nbOfPlayers) + 0].transform.localPosition = new Vector3(-80, -445,  0); 
                m_scoreText[(2 * m_nbOfPlayers) + 1].transform.localPosition = new Vector3(-80, -470, 0); 
                m_scoreText[(2 * m_nbOfPlayers) + 3].transform.localPosition = new Vector3(-80, -495,  0);
                m_scoreText[(3 * m_nbOfPlayers) + 3].transform.localPosition = new Vector3(100,-55, 0);
                m_scoreText[(3 * m_nbOfPlayers) + 0].transform.localPosition = new Vector3(880,-445, 0); 
                m_scoreText[(3 * m_nbOfPlayers) + 1].transform.localPosition = new Vector3(880,-470, 0); 
                m_scoreText[(3 * m_nbOfPlayers) + 2].transform.localPosition = new Vector3(880, -495, 0); 
                break;
        }
        
        m_isInitialized = true;
    }


    public void AddScore(int p_score, int p_playerNum)
    {
        p_playerNum--;
        if (p_playerNum < 0 || p_playerNum > m_nbOfPlayers)
            return;
        
        m_scores[p_playerNum] += p_score;
        CheckForVictory(p_playerNum);
    }

    private void DisplayScore()
    {
        for (int i = 0; i < m_nbOfPlayers; ++i)
        {
            for (int j = 0; j < m_nbOfPlayers; ++j)
            {
                if (j == i)
                {
                    m_scoreText[(i * m_nbOfPlayers) + j].text = "P" + (i + 1) + " kills = " + m_scores[i] + " ";
                    continue;
                }

                m_scoreText[i * m_nbOfPlayers + (j % m_nbOfPlayers)].text = "P" + ((j % m_nbOfPlayers)+1) + " kills = " + m_scores[j] + " ";
            }
        }
    }

    private void CheckForVictory(int p_playerNum)
    {
        if (m_scores[p_playerNum] >= m_nbOfPointsToWin)
        {
            GameObject vt = Instantiate(m_victoryText, m_uiTargetTransform);
            vt.GetComponent<TextMeshProUGUI>().text = "P" + (p_playerNum + 1) + " Won the game !";

            for (int i = 0; i < m_nbOfPlayers; ++i)
            {
                GameManager.m_instance.m_playerList[i].GetComponent<TankController>().SetCanMove(false);
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.O) && Input.GetKey(KeyCode.LeftShift))
            Initialize();
        
        if (!m_isInitialized)
            return;
        
        DisplayScore();
    }
}
