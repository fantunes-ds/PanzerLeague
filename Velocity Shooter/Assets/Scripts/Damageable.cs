using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Damageable : MonoBehaviour
{
    public float m_maxHealth = 100.0f;

    [SerializeField] 
    private float timeToDestruction = 1.0f;
    public float m_health { get; private set; }
    
    public float m_maxArmor = 100.0f;
    
    [SerializeField]
    private float m_defaultArmor = 0.0f;
    public float m_armor { get; private set; }
    private TankController m_tank;
    
    // Start is called before the first frame update
    void Start()
    {
        m_health = m_maxHealth;
        m_armor = m_defaultArmor;
        if (GetComponent<TankController>() != null)
        {
            m_tank = GetComponent<TankController>();
        }
    }

    public void TakeDamage(float p_damage)
    {
        float remainingDamage = p_damage;
        while (m_armor > 0 && remainingDamage > 0)
        {
            m_armor--;
            remainingDamage--;
        }
        m_health -= remainingDamage;
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
            
        m_tank.SetCanMove(false);
        m_tank.SetIsDead(true);
        StartCoroutine(DestroyDelayed(1.0f));
    }

    IEnumerator DestroyDelayed(float p_time)
    {
        yield return new WaitForSeconds(p_time);
        Destroy(gameObject);
    }
}
