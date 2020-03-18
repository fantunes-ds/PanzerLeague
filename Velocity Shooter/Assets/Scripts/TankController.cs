using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class TankController : MonoBehaviour
{
    [SerializeField]
    private GameObject m_canon;
    [SerializeField]
    private Camera m_tankCamera;

    [SerializeField]
    private float m_tankSpeed = 3;
    [SerializeField]
    private float m_tankRotSpeed = 30;
    [SerializeField]
    private float m_maxSpeed = 10.0f;
    [SerializeField]
    private float m_canonRotSpeed = 30;

    private Rigidbody m_rb;
    private Rigidbody m_canonRb;
    // Start is called before the first frame update
    void Start()
    {
        m_rb = GetComponent<Rigidbody>();
        m_canonRb = m_canon.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Quaternion tankRotation = transform.rotation * Quaternion.Euler(Vector3.up * Input.GetAxis("Horizontal") * m_tankRotSpeed * Time.deltaTime);

        Vector3 canonRotationAxis = Vector3.up * Input.GetAxis("Horizontal2") + Vector3.right * Input.GetAxis("Vertical2");
        
        Quaternion canonRotation = Quaternion.Euler(canonRotationAxis) *  m_canon.transform.rotation; 

        if (canonRotation.eulerAngles.x > 10.0f && canonRotation.eulerAngles.x < 270.0f)
            canonRotation = Quaternion.Euler(9.0f, canonRotation.eulerAngles.y, canonRotation.eulerAngles.z);
        else if (canonRotation.eulerAngles.x < 350.0f && canonRotation.eulerAngles.x > 270.0f )
            canonRotation = Quaternion.Euler(350.0f, canonRotation.eulerAngles.y, canonRotation.eulerAngles.z);

        canonRotation.z = 0;
        
        if (Mathf.Abs(m_rb.velocity.x) + Mathf.Abs(m_rb.velocity.y) + Mathf.Abs(m_rb.velocity.z) < m_maxSpeed)
        {m_rb.AddForce(transform.forward * Input.GetAxis("Trigger") * m_tankSpeed);}
        
        m_rb.MoveRotation(tankRotation);
        m_canonRb.MoveRotation(canonRotation);
    }
}
