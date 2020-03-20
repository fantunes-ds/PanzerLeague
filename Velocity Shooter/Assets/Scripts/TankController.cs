using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
[RequireComponent(typeof(Rigidbody))]
public class TankController : MonoBehaviour
{
    [SerializeField]
    private GameObject m_canon;
    [SerializeField]
    private Camera m_tankCamera;

    [SerializeField]
    private float m_tankSpeed = 30;
    [SerializeField]
    private float m_tankRotSpeed = 60;
    [SerializeField]
    private float m_maxSpeed = 10.0f;
    [SerializeField]
    private float m_canonRotSpeed = 60;

    private Rigidbody m_rb;
    private GameObject m_canonRb;
    // Start is called before the first frame update
    void Start()
    {
        m_rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        ControlTank();
        RotateTank();
        RotateCanon();
    }

    void RotateCanon()
    {
        Vector3 canonRotationAxis = Vector3.right * Input.GetAxis("Vertical2") + Vector3.up * Input.GetAxis("Horizontal2");
        Quaternion canonRotation = m_canon.transform.rotation * Quaternion.Euler(canonRotationAxis); 

        //Clamp X rotation
        if (canonRotation.eulerAngles.x > 10.0f && canonRotation.eulerAngles.x < 270.0f)
            canonRotation = Quaternion.Euler(9.0f, canonRotation.eulerAngles.y, canonRotation.eulerAngles.z);
        else if (canonRotation.eulerAngles.x < 350.0f && canonRotation.eulerAngles.x > 270.0f )
            canonRotation = Quaternion.Euler(350.0f, canonRotation.eulerAngles.y, canonRotation.eulerAngles.z);
        
        

        
        Quaternion q = canonRotation;
        q.eulerAngles = new Vector3(q.eulerAngles.x, q.eulerAngles.y, 0);
        m_canon.transform.rotation = q;
    }

    void ControlTank()
    {
        if (Mathf.Abs(m_rb.velocity.x) + Mathf.Abs(m_rb.velocity.y) + Mathf.Abs(m_rb.velocity.z) < m_maxSpeed)
        {m_rb.AddForce(transform.forward * Input.GetAxis("Trigger") * m_tankSpeed);}
    }

    void RotateTank()
    {
        Quaternion tankRotation = transform.rotation * Quaternion.Euler(Vector3.up * Input.GetAxis("Horizontal") * m_tankRotSpeed * Time.deltaTime);
        m_rb.MoveRotation(tankRotation);
    }
}
