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
    private float m_rotSpeed = 30;
    
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
        Debug.Log(Input.GetAxis("Horizontal2"));

        Vector3 newPos = transform.position + (transform.forward * Input.GetAxis("Vertical") * m_tankSpeed * Time.deltaTime);  
        
        Quaternion newRot = transform.rotation * Quaternion.Euler(Vector3.up * Input.GetAxis("Horizontal") * m_rotSpeed * Time.deltaTime); 
        Quaternion canonRot = m_canon.transform.rotation * Quaternion.Euler(Vector3.up * Input.GetAxis("Horizontal2") * m_rotSpeed * Time.deltaTime); 

        newRot.z = 0;
        m_rb.MovePosition(newPos);
        m_rb.MoveRotation(newRot);
        m_canonRb.MoveRotation(canonRot);
    }
}
