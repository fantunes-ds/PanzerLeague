using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.Serialization;
[RequireComponent(typeof(Rigidbody))]
public class TankController : MonoBehaviour
{
    [SerializeField]
    private GameObject m_canon;
    [SerializeField]
    private Transform m_tankCamera;

    [SerializeField]
    private Transform[] m_wheels;

    [SerializeField]
    private float m_tankSpeed = 30;
    [SerializeField]
    private float m_tankRotSpeed = 60;
    [SerializeField]
    private float m_maxSpeed = 10.0f;
    [SerializeField]
    private float m_canonRotSpeed = 60;
    [SerializeField]
    private float m_maxCanonRotAngle = 10.0f;

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
        AnimateWheels();
    }

    void RotateCanon()
    {
        Vector3 canonRotationAxis = Vector3.right * Input.GetAxis("Vertical2") + Vector3.up * Input.GetAxis("Horizontal2");
//        Quaternion canonRotation = m_canon.transform.rotation * Quaternion.Euler(canonRotationAxis); 

        Quaternion canonRotation = m_tankCamera.transform.rotation;
        //Clamp X rotation
        if (canonRotation.eulerAngles.x > m_maxCanonRotAngle && canonRotation.eulerAngles.x < 270.0f)
            canonRotation = Quaternion.Euler(m_maxCanonRotAngle, canonRotation.eulerAngles.y, canonRotation.eulerAngles.z);
        else if (canonRotation.eulerAngles.x < 360.0f - m_maxCanonRotAngle && canonRotation.eulerAngles.x > 270.0f )
            canonRotation = Quaternion.Euler(360.0f - m_maxCanonRotAngle, canonRotation.eulerAngles.y, canonRotation.eulerAngles.z);
        
        Quaternion q = canonRotation;
        q.eulerAngles = Vector3.Slerp(q.eulerAngles, new Vector3(q.eulerAngles.x, q.eulerAngles.y, 0), Time.deltaTime * m_canonRotSpeed);
        m_canon.transform.rotation = q;
    }

    void ControlTank()
    {
        if (GetSpeed() < m_maxSpeed)
        {m_rb.AddForce(transform.forward * Input.GetAxis("Throttle") * m_tankSpeed);}
    }

    void RotateTank()
    {
        Quaternion tankRotation = transform.rotation * Quaternion.Euler(Vector3.up * Input.GetAxis("Horizontal") * m_tankRotSpeed * Time.deltaTime);
        m_rb.MoveRotation(tankRotation);
    }

    private void AnimateWheels()
    {
        float speed = GetSpeed();
        for (int i = 0; i < m_wheels.Length; i++)
        {
            m_wheels[i].Rotate(new Vector3(-speed, 0, 0));
        }
    }

    float GetSpeed()
    {
        return Mathf.Abs(m_rb.velocity.x) + Mathf.Abs(m_rb.velocity.y) + Mathf.Abs(m_rb.velocity.z);
    }
}
