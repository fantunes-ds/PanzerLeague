using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damageable : MonoBehaviour
{
    
    [SerializeField] 
    private float m_maxHealth;

    private float m_health;
    private TankController m_tank;
    
    // Start is called before the first frame update
    void Start()
    {
        m_health = m_maxHealth;
        if (GetComponent<TankController>() != null)
        {
            m_tank = GetComponent<TankController>();
        }
    }

    public void TakeDamage(float p_damage)
    {
        m_health -= p_damage;
        CheckDestruction();
    }

    void CheckDestruction()
    {
        if (m_health <= 0)
        {
            //Play any death animation/fire if needed here
            if (m_tank != null)
            {
                m_tank.SetCanMove(false);
                m_tank.SetIsDead(true);
                StartCoroutine(DestroyDelayed(3.0f));
            }
        }
    }

    IEnumerator DestroyDelayed(float p_time)
    {
        yield return new WaitForSeconds(p_time);
        Destroy(gameObject);
    }
}
