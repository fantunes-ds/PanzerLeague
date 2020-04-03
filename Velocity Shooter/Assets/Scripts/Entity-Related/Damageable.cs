using System.Collections;
using UnityEngine;

public class Damageable : MonoBehaviour
{
    private int m_lastHitId;
    public float m_maxHealth = 100.0f;

    [SerializeField] 
    private float m_timeToDestruction = 1.0f;
    public float m_health { get; private set; }
    
    public float m_maxArmor = 100.0f;
    
    [SerializeField]
    private float m_defaultArmor = 0.0f;
    public float m_armor { get; private set; }
    private TankController m_tank;

    [SerializeField]
    private int m_pointsGivenOnDestruction;
    
    // Start is called before the first frame update
    void Start()
    {
        ResetValuesToDefault();
        if (GetComponent<TankController>() != null)
        {
            m_tank = GetComponent<TankController>();
        }
    }

    public void ResetValuesToDefault()
    {
        m_health = m_maxHealth;
        m_armor = m_defaultArmor;
    }

    public void TakeDamage(float p_damage, int p_originId)
    {
        float remainingDamage = p_damage;
        while (m_armor > 0 && remainingDamage > 0)
        {
            m_armor--;
            remainingDamage--;
        }
        m_health -= remainingDamage;
        
        m_lastHitId = p_originId;
        CheckDestruction();
    }
    
    public void ReceiveHealth(float p_health)
    {
        m_health += p_health;
        if (m_health > m_maxHealth)
            m_health = m_maxHealth;
    }
    
    public void ReceiveArmor(float p_armor)
    {
        m_armor += p_armor;
        if (m_armor > m_maxArmor)
            m_armor = m_maxArmor;
    }

    void CheckDestruction()
    {
        if (m_health > 0) 
            return;
        
        //Play any death animation/fire if needed here
        if (m_tank == null) 
            return;

        if (m_tank.m_isDead)
            return;
            
        GameManager.m_instance.GetComponent<ScoreManager>().AddScore(m_pointsGivenOnDestruction, m_lastHitId);
        m_tank.SetCanMove(false);
        m_tank.SetIsDead(true);
        m_tank.StartCoroutine(m_tank.EnableRespawnDelayed(m_timeToDestruction));
    }
}
