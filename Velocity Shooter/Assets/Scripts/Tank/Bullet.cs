using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class Bullet : MonoBehaviour
{
    [HideInInspector]
    public int m_fireOriginId;
    [HideInInspector]
    public float m_speed;
    private Vector3 m_bulletVelocity;
    [SerializeField]
    private float m_damage = 40.0f;

    [HideInInspector]
    public float m_additionalDamage = 0.0f;

    [SerializeField]
    private int m_destroyDelay;
    
    private bool m_canMove;
    
    private void Start()
    {
        m_canMove = true;
        StartCoroutine(AutoDestroy(m_destroyDelay));
        m_bulletVelocity = transform.forward * m_speed;
    }

    IEnumerator AutoDestroy(float p_seconds)
    {
        yield return new WaitForSeconds(p_seconds);
        Destroy(gameObject);
    }
    
    // Update is called once per frame
    void Update()
    {
        if(m_canMove)
            MoveBullet();
    }

    void MoveBullet()
    {
        Vector3 point1 = transform.position;
        float stepSize = 1.0f / 20.0f;

        for (float i = 0; i < 1; i += stepSize)
        {
            m_bulletVelocity += Physics.gravity * stepSize * Time.deltaTime;
            Vector3 point2 = point1 + m_bulletVelocity * stepSize * Time.deltaTime;

            RaycastHit hit;
            if (Physics.Raycast(point1, (point2 - point1), out hit, (point2-point1).magnitude))
            {
                gameObject.transform.parent = hit.transform;
                DealDamage(hit);
                //Play any animation/FX here
                m_canMove = false;
            }

            point1 = point2;
        }

        transform.position = point1;
    }

    void DealDamage(RaycastHit p_hit)
    {
        if (p_hit.transform == null)
            return;
                
        if (p_hit.transform.gameObject.GetComponent<Damageable>() != null)
        {
            p_hit.transform.gameObject.GetComponent<Damageable>().TakeDamage(m_damage + m_additionalDamage, m_fireOriginId);
        }
    }
}
