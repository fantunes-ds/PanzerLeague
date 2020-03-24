using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.Serialization;
[RequireComponent(typeof(Rigidbody))]
public class TankController : MonoBehaviour
{
    [SerializeField]
    private Transform m_tankCamera;


    [Header("Tank")]
    [SerializeField]
    private float m_tankSpeed = 30;
    [SerializeField]
    private float m_tankRotSpeed = 60;
    [SerializeField]
    private float m_maxSpeed = 10.0f;

    [Header("Canon")]
    [SerializeField][Tooltip("The complete turret that rotates 360 degrees")]
    private GameObject m_turret;
    [SerializeField][Tooltip("The canon is the tube that moves vertically where the bullet comes out")]
    private GameObject m_canon;
    [SerializeField]
    private float m_canonRotSpeed = 60;
    [SerializeField]
    private float m_maxCanonRotAngle = 10.0f;

    [Header("Wheels Animations")]
    [SerializeField][Tooltip("Drag and drop all four wheels. Front are 0 and 1, back are 2 and 3")]
    private Transform[] m_wheels;
    [SerializeField][Tooltip("It's only visual, for the wheel animation")]
    private float m_maxWheelTurnAngle = 25.0f;
    [SerializeField]
    private float m_wheelAnimationSpeed = 1.0f;
    
    
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
        RotateTurret();
        AnimateWheels();
    }

    void RotateTurret()
    {
        if (Input.GetKey(KeyCode.C))
            return;
            
        Vector3 canonRotationAxis = Vector3.right * Input.GetAxis("Vertical2") + Vector3.up * Input.GetAxis("Horizontal2");

        Quaternion turretRotation = m_turret.transform.parent.rotation;
        m_turret.transform.rotation = Quaternion.Euler(turretRotation.eulerAngles.x, m_tankCamera.transform.rotation.eulerAngles.y, turretRotation.eulerAngles.z);
        m_turret.transform.localRotation = Quaternion.Euler(0.0f, m_turret.transform.localRotation.eulerAngles.y, 0.0f);

        Quaternion canonRotation = m_canon.transform.localRotation;
        canonRotation = Quaternion.Euler(-m_tankCamera.rotation.eulerAngles.x, canonRotation.eulerAngles.y, canonRotation.eulerAngles.z);
        //Clamp X rotation
        if (canonRotation.eulerAngles.x > m_maxCanonRotAngle && canonRotation.eulerAngles.x < 270.0f)
            canonRotation = Quaternion.Euler(m_maxCanonRotAngle, canonRotation.eulerAngles.y, canonRotation.eulerAngles.z);
        else if (canonRotation.eulerAngles.x < 360.0f - m_maxCanonRotAngle && canonRotation.eulerAngles.x > 270.0f )
            canonRotation = Quaternion.Euler(360.0f - m_maxCanonRotAngle, canonRotation.eulerAngles.y, canonRotation.eulerAngles.z);
        m_canon.transform.localRotation = canonRotation;
    }

    void ControlTank()
    {
        if (GetSpeed() < m_maxSpeed)
        {m_rb.AddForce(transform.forward * Input.GetAxis("Throttle") * m_tankSpeed);}
    }

    void RotateTank()
    {
        Quaternion tankRotation = transform.rotation * Quaternion.Euler(Vector3.up * Input.GetAxis("Horizontal") * Input.GetAxis("Throttle") * GetSpeed() * m_tankRotSpeed * Time.deltaTime);
        m_rb.MoveRotation(tankRotation);
    }

    private void AnimateWheels()
    {
        if (m_wheels.Length < 4)
            return;
        
        Quaternion q = m_wheels[2].rotation;
        float rotAngle = q.eulerAngles.x - GetSpeed() * Input.GetAxis("Throttle");
            
            if (rotAngle > 180 && rotAngle < 270)
                rotAngle = 0;
            else if (rotAngle < 180 && rotAngle > 90)
                rotAngle = 0;
        
        q.eulerAngles = new Vector3(rotAngle, m_wheels[2].rotation.eulerAngles.y + Input.GetAxis("Horizontal") * m_maxWheelTurnAngle, 0.1f);
        m_wheels[0].rotation = q;
        m_wheels[0].localRotation = Quaternion.Euler(m_wheels[0].localRotation.eulerAngles.x, m_wheels[0].localRotation.eulerAngles.y, 0.0f);

        q = m_wheels[1].rotation;
        q.eulerAngles = new Vector3(rotAngle,m_wheels[2].rotation.eulerAngles.y + (Input.GetAxis("Horizontal") * m_maxWheelTurnAngle), 0.1f);
        m_wheels[1].rotation = q;
        m_wheels[1].localRotation = Quaternion.Euler(m_wheels[1].localRotation.eulerAngles.x, m_wheels[1].localRotation.eulerAngles.y, 0.0f);
        
        
        q = m_wheels[2].localRotation;
        q.eulerAngles = new Vector3(rotAngle, 0.1f, 0.1f);
        m_wheels[2].localRotation = q;
        
        q = m_wheels[3].localRotation;
        q.eulerAngles = new Vector3(rotAngle, 0.1f, 0.1f);
        m_wheels[3].localRotation = q;

    }

    float GetSpeed()
    {
        return Mathf.Abs(m_rb.velocity.x) + Mathf.Abs(m_rb.velocity.y) + Mathf.Abs(m_rb.velocity.z);
    }
}
