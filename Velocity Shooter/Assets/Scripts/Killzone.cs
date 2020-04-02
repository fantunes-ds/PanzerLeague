using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Killzone : MonoBehaviour
{
    [SerializeField]
    private float m_killzoneGravityForce = 0.3f;

    private TankController m_tank;
    private Rigidbody m_otherRb;

    [SerializeField]
    private float m_timeToRespawn = 2.0f;

    private void OnTriggerStay(Collider other)
    {
        if (m_tank && m_otherRb)
        {
            m_otherRb.AddForce(Vector3.down * m_killzoneGravityForce);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<TankController>())
        {
            m_tank = other.GetComponent<TankController>();

            if (m_tank.m_isDead)
                return;

            m_otherRb = m_tank.GetComponent<Rigidbody>();
            m_otherRb.useGravity = false;
            m_otherRb.drag = 2f;
            m_tank.StartCoroutine(m_tank.EnableRespawnDelayed(m_timeToRespawn));
            m_tank.SetCanMove(false);
            m_tank.SetIsDead(true);
        }
    }
}
