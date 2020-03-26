﻿using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.Serialization;
[RequireComponent(typeof(Rigidbody))]
public class TankController : MonoBehaviour
{
    [Header("Camera")] 
    public Transform m_cameraTarget;
    public Transform m_zoomTarget;
    [HideInInspector] 
    public GameObject m_tankCamera;
    private Transform m_tankCameraTransform;
    private CameraFollow m_tankCameraFollow;


    [Header("Tank")] 
    [SerializeField] 
    private float m_tankSpeed = 30;
    [SerializeField] 
    private float m_tankRotSpeed = 60;
    [SerializeField] 
    private float m_maxSpeed = 10.0f;
    private Rigidbody m_rb;
    private LineRenderer m_lr;
    private ProjectilePrediction m_tankProjectilePrediction;

    [Header("Canon")] [SerializeField] [Tooltip("The complete turret that rotates 360 degrees")]
    private GameObject m_turret;

    [SerializeField] [Tooltip("The canon is the tube that moves vertically where the bullet comes out")]
    private GameObject m_canon;

    private GameObject m_canonRb;
    [SerializeField] 
    private float m_canonRotSpeed = 60;
    [SerializeField] 
    private float m_maxCanonRotAngle = 10.0f;
    [SerializeField] 
    private float m_fireRate = 1.0f;
    [SerializeField][Tooltip("The recoil percentage caused by the shot")][Range(0f,500f)]
    private float m_recoil;
    private float m_fireRateCounter = 0.0f;
    private bool m_isZooming;
    private bool m_isShooting;

    [Header("Wheels Animations")]
    [SerializeField]
    [Tooltip("Drag and drop all four wheels. Front are 0 and 1, back are 2 and 3")]
    private Transform[] m_wheels;

    [SerializeField] [Tooltip("It's only visual, for the wheel animation")]
    private float m_maxWheelTurnAngle = 25.0f;

    [SerializeField] private float m_wheelAnimationSpeed = 1.0f;

    


    // Start is called before the first frame update
    void Start()
    {
        m_rb = GetComponent<Rigidbody>();
        m_tankCameraTransform = m_tankCamera.GetComponentInChildren<Transform>();
        m_tankCameraFollow = m_tankCamera.GetComponent<CameraFollow>();
        m_lr = transform.GetComponent<LineRenderer>();
        m_tankProjectilePrediction = GetComponent<ProjectilePrediction>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        ControlTank();
        RotateTank();
        RotateTurret();
        AnimateWheels();
        SetZoom();
        Shoot();
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
        canonRotation = Quaternion.Euler(-m_tankCameraTransform.rotation.eulerAngles.x, canonRotation.eulerAngles.y, canonRotation.eulerAngles.z);
        
        //Clamp X rotation
        if (canonRotation.eulerAngles.x > m_maxCanonRotAngle && canonRotation.eulerAngles.x < 270.0f)
            canonRotation = Quaternion.Euler(m_maxCanonRotAngle, canonRotation.eulerAngles.y, canonRotation.eulerAngles.z);
        else if (canonRotation.eulerAngles.x < 360.0f - m_maxCanonRotAngle && canonRotation.eulerAngles.x > 270.0f )
            canonRotation = Quaternion.Euler(360.0f - m_maxCanonRotAngle, canonRotation.eulerAngles.y, canonRotation.eulerAngles.z);
        
        m_canon.transform.localRotation = canonRotation;
    }

    void SetZoom()
    {

        if (Input.GetKey(KeyCode.JoystickButton4) || Input.GetMouseButton(1))
            m_isZooming = true;
        else
            m_isZooming = false;
        
        m_tankCameraFollow.SetIsZooming(m_isZooming);
        m_lr.enabled = m_isZooming;
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

    void Shoot()
    {
        if (m_isShooting)
        {
            m_fireRateCounter += Time.deltaTime;

            if (m_fireRateCounter > m_fireRate)
            {
                m_isShooting = false;
                m_fireRateCounter = 0;
            }
        }
        else if ((Input.GetKeyDown(KeyCode.JoystickButton5) || Input.GetMouseButtonDown(0)) && !m_isShooting)
        {
            m_isShooting = true;
            m_tankProjectilePrediction.SetIsShooting(true);
            m_tankProjectilePrediction.Shoot();
            m_rb.AddForce(m_tankProjectilePrediction.m_bulletOrigin.forward * -1 * m_recoil);
            m_rb.AddForce(m_tankProjectilePrediction.m_bulletOrigin.up * -1 * m_recoil * 2.0f);
        }
        
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
