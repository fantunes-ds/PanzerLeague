using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    private Vector2 m_maxCanonRotAngle;
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

    private bool m_canMove = true;
    public bool m_isDead { private set; get; }
    public bool m_canRespawn { private set; get; }
    public int m_powerUpUsableTimes = 0;
    
    [HideInInspector]
    public float m_superBulletDamage = 0;


    // Start is called before the first frame update
    void Start()
    {
        m_rb = GetComponent<Rigidbody>();
        m_tankCameraTransform = m_tankCamera.GetComponentInChildren<Transform>();
        m_tankCameraFollow = m_tankCamera.GetComponent<CameraFollow>();
        m_lr = transform.GetComponent<LineRenderer>();
        m_tankProjectilePrediction = GetComponent<ProjectilePrediction>();
        m_canRespawn = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (m_canMove)
        {
            ControlTank();
            RotateTank();
            RotateTurret();
            AnimateWheels();
            SetZoom();
            Shoot();
            Suicide();
        }
        else if (m_canRespawn)
        {
            Respawn();
        }
    }

    void Suicide()
    {
        if (Input.GetAxis(gameObject.name + "Suicide") > 0.9f && !m_isDead )
        {
            SetCanMove(false);
            SetIsDead(true);
            StartCoroutine(EnableRespawnDelayed(1.0f));
            GameManager.m_instance.GetComponent<ScoreManager>().AddScore(-1, gameObject.name[1] - 48);
        }
    }

    void Respawn()
    {
        List<Transform> spp = GameManager.m_instance.m_spawnPoints;
        transform.position = spp[Random.Range(0, spp.Count)].position;
        
        SetCanRespawn(false);
        SetIsDead(false);
        SetCanMove(true);
        
        GetComponent<Damageable>().ResetValuesToDefault();
        
        m_rb.drag = 0.1f;
        
        if (m_rb.useGravity == false)
            m_rb.useGravity = true;
    }
    
    void RotateTurret()
    {
        if (Input.GetKey(KeyCode.C))
            return;
            
        Vector3 canonRotationAxis = Vector3.right * Input.GetAxis(gameObject.name + "Vertical2") + Vector3.up * Input.GetAxis(gameObject.name +"Horizontal2");

        Quaternion turretRotation = m_turret.transform.parent.rotation;
        m_turret.transform.rotation = Quaternion.Euler(turretRotation.eulerAngles.x, m_tankCamera.transform.rotation.eulerAngles.y, turretRotation.eulerAngles.z);
        m_turret.transform.localRotation = Quaternion.Euler(0.0f, m_turret.transform.localRotation.eulerAngles.y, 0.0f);

        Quaternion canonRotation = m_canon.transform.localRotation;
        canonRotation = Quaternion.Euler(m_tankCameraTransform.rotation.eulerAngles.x, canonRotation.eulerAngles.y, canonRotation.eulerAngles.z);
        
        //Clamp X rotation
        if (canonRotation.eulerAngles.x < m_maxCanonRotAngle.x && canonRotation.eulerAngles.x > 270)
            canonRotation = Quaternion.Euler(m_maxCanonRotAngle.x, canonRotation.eulerAngles.y, canonRotation.eulerAngles.z);
        else if (canonRotation.eulerAngles.x > m_maxCanonRotAngle.y && canonRotation.eulerAngles.x < 270)
            canonRotation = Quaternion.Euler(m_maxCanonRotAngle.y, canonRotation.eulerAngles.y, canonRotation.eulerAngles.z);
        
        m_canon.transform.localRotation = canonRotation;
    }

    void SetZoom()
    {

        if (Input.GetAxis(gameObject.name +"Zoom") > 0.9f  || Input.GetMouseButton(1))
            m_isZooming = true;
        else
            m_isZooming = false;
        
        m_tankCameraFollow.SetIsZooming(m_isZooming);
        m_lr.enabled = m_isZooming;
    }

    void ControlTank()
    {
        if (GetSpeed() < m_maxSpeed)
        {m_rb.AddForce(transform.forward * (Input.GetAxis(gameObject.name +"Throttle") + Input.GetAxis("Throttle")) * m_tankSpeed);}
    }

    void RotateTank()
    {
        Quaternion tankRotation = transform.rotation * Quaternion.Euler(Vector3.up * (Input.GetAxis(gameObject.name +"Horizontal") + Input.GetAxis("Horizontal")) * (Input.GetAxis(gameObject.name +"Throttle") + Input.GetAxis("Throttle")) * GetSpeed() * m_tankRotSpeed * Time.deltaTime);
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
        else if ((Input.GetAxis(gameObject.name +"Fire") > 0.5f || Input.GetMouseButtonDown(0)) && !m_isShooting)
        {
            m_isShooting = true;
            m_tankProjectilePrediction.SetIsShooting(true);

            if (m_powerUpUsableTimes > 0)
            {
                m_tankProjectilePrediction.Shoot(m_superBulletDamage);
                m_powerUpUsableTimes--;
            }
            else
            {
                m_tankProjectilePrediction.Shoot(0.0f);
            }

            m_rb.AddForce(m_tankProjectilePrediction.m_bulletOrigin.forward * -1 * m_recoil);
            m_rb.AddForce(m_tankProjectilePrediction.m_bulletOrigin.up * -1 * m_recoil * 2.0f);
        }
        
    }

    private void AnimateWheels()
    {
        if (m_wheels.Length < 4)
            return;

        Quaternion q = m_wheels[2].rotation;
        float rotAngle = q.eulerAngles.x + GetSpeed() * (Input.GetAxis(gameObject.name +"Throttle") + Input.GetAxis("Throttle"));

        if (rotAngle > 180 && rotAngle < 270)
            rotAngle = 0;
        else if (rotAngle < 180 && rotAngle > 90)
            rotAngle = 0;
        
        q.eulerAngles = new Vector3(rotAngle, m_wheels[2].rotation.eulerAngles.y + (Input.GetAxis("Horizontal") * m_maxWheelTurnAngle + Input.GetAxis(gameObject.name +"Horizontal") * m_maxWheelTurnAngle), 0.1f);
        m_wheels[0].rotation = q;
        m_wheels[0].localRotation = Quaternion.Euler(m_wheels[0].localRotation.eulerAngles.x, m_wheels[0].localRotation.eulerAngles.y, 0.0f);

        q = m_wheels[1].rotation;
        q.eulerAngles = new Vector3(rotAngle,m_wheels[2].rotation.eulerAngles.y + (Input.GetAxis("Horizontal") * m_maxWheelTurnAngle + Input.GetAxis(gameObject.name +"Horizontal") * m_maxWheelTurnAngle), 0.1f);
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

    public void SetCanMove(bool p_cm)
    {
        m_canMove = p_cm;
    }
    
    public void SetCanRespawn(bool p_cr)
    {
        m_canRespawn = p_cr;
    }
    
    public void SetIsDead(bool p_id)
    {
        m_isDead = p_id;
    }
    
    public IEnumerator EnableRespawnDelayed(float p_time)
    {
        yield return new WaitForSeconds(p_time);
        //Stop death animation/fire if needed here
        SetCanRespawn(true);
    }
}
