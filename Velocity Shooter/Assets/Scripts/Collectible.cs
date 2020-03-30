using UnityEngine;

public class Collectible : MonoBehaviour
{
    public enum CollectibleType
    {
        Armor,
        Life,
        SuperBullet
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
    
    void Start()
    {
        switch (m_type)
        {
            case CollectibleType.Armor:
                GetComponent<MeshRenderer>().materials[0].SetColor("_BaseColor", Color.cyan);
                break;
            case CollectibleType.Life:
                GetComponent<MeshRenderer>().materials[0].SetColor("_BaseColor", Color.red);
                break;
            case CollectibleType.SuperBullet:
                GetComponent<MeshRenderer>().materials[0].SetColor("_BaseColor", Color.yellow);
                break;
        }
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
        }
    }
}
