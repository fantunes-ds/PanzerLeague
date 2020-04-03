using System.Collections.Generic;
using UnityEngine;

public class Collectible : MonoBehaviour
{
    public enum CollectibleType
    {
        Armor,
        Life,
        SuperBullet,
        Boost
    }

    [SerializeField]
    private CollectibleType m_type;

    [SerializeField]
    private float m_healthReceived = 50.0f;
    [SerializeField]
    private float m_armorReceived = 50.0f;
    [SerializeField]
    private int m_maxSuperBulletUsage = 2;
    [SerializeField]
    private int m_superBulletDamage = 15;
    [SerializeField]
    private int m_boostForce = 500;
    [SerializeField]
    private int m_rotationSpeed = 2;

    [SerializeField]
    private GameObject[] m_meshesPrefabs;
    
    void Start()
    {
        switch (m_type)
        {
            case CollectibleType.Armor:
                GetComponent<MeshFilter>().mesh = m_meshesPrefabs[0].GetComponent<MeshFilter>().sharedMesh;
                GetComponent<MeshRenderer>().material = m_meshesPrefabs[0].GetComponent<MeshRenderer>().sharedMaterial;
                break;
            case CollectibleType.Life:
                GetComponent<MeshFilter>().mesh = m_meshesPrefabs[1].GetComponent<MeshFilter>().sharedMesh;
                GetComponent<MeshRenderer>().material = m_meshesPrefabs[1].GetComponent<MeshRenderer>().sharedMaterial;
                break;
            case CollectibleType.SuperBullet:
                GetComponent<MeshFilter>().mesh = m_meshesPrefabs[2].GetComponent<MeshFilter>().sharedMesh;
                GetComponent<MeshRenderer>().material = m_meshesPrefabs[2].GetComponent<MeshRenderer>().sharedMaterial;
                break;
            case CollectibleType.Boost:
                GetComponent<MeshFilter>().mesh = m_meshesPrefabs[3].GetComponent<MeshFilter>().sharedMesh;
                GetComponent<MeshRenderer>().material = m_meshesPrefabs[3].GetComponent<MeshRenderer>().sharedMaterial;
                break;
        }

        BoxCollider newBC = gameObject.AddComponent<BoxCollider>();
        newBC.isTrigger = true;
    }
    private void OnTriggerEnter(Collider other)
    {
        switch (m_type)
        {
            case CollectibleType.Armor:
                if (other.gameObject.GetComponent<Damageable>() != null && other.gameObject.GetComponent<TankController>())
                {
                    Damageable dmRef = other.gameObject.GetComponent<Damageable>();
                    if (dmRef.m_armor < dmRef.m_maxArmor)
                        dmRef.ReceiveArmor(m_armorReceived);
                    Destroy(gameObject);
                }
                break;
            case CollectibleType.Life:
                if (other.gameObject.GetComponent<Damageable>() != null && other.gameObject.GetComponent<TankController>())
                {
                    Damageable dmRef = other.gameObject.GetComponent<Damageable>();
                    if (dmRef.m_health < dmRef.m_maxHealth)
                        dmRef.ReceiveHealth(m_healthReceived);
                    Destroy(gameObject);
                }
                break;
            case CollectibleType.SuperBullet:
                if (other.gameObject.GetComponent<TankController>())
                {
                    other.gameObject.GetComponent<TankController>().m_powerUpUsableTimes = m_maxSuperBulletUsage;
                    other.gameObject.GetComponent<TankController>().m_superBulletDamage = m_superBulletDamage;
                    Destroy(gameObject);
                }
                break;
            case CollectibleType.Boost:
                if (other.gameObject.GetComponent<TankController>())
                {
                    other.gameObject.GetComponent<Rigidbody>().AddForce(other.gameObject.transform.forward * m_boostForce) ;
                    Destroy(gameObject);
                }
                break;
        }
    }

    private void Update()
    {
        transform.Rotate(0, m_rotationSpeed,0);
    }
}
