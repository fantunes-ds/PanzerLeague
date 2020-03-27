using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField]
    private float m_moveSpeed = 120.0f;
    public Transform m_cameraTarget;
    [SerializeField]
    public Transform m_zoomTarget;

    private Vector3 m_followPos;

    [SerializeField]
    private float m_clampAngleLow = 25.0f;
    [SerializeField]
    private float m_clampAngleHigh = 70.0f;
    [SerializeField]
    private float m_inputSensitivity = 150.0f;
    [SerializeField] 
    private CameraSpringArm m_cameraObject;
    [SerializeField] 
    private Vector2 m_input;
    [SerializeField] 
    private Vector2 m_finalInput;    
    [SerializeField] 
    private Vector2 m_rot;

    private bool m_isZooming;


    // Start is called before the first frame update
    void Start()
    {
        m_rot.x = transform.localRotation.eulerAngles.x;
        m_rot.y = transform.localRotation.eulerAngles.y;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        float xInput = Input.GetAxis(gameObject.name + "Horizontal2");
        float yInput = Input.GetAxis(gameObject.name + "Vertical2");

        m_input.x = Input.GetAxis("Mouse X");
        m_input.y = Input.GetAxis("Mouse Y");
        m_finalInput.x = xInput + m_input.x;
        m_finalInput.y = yInput + m_input.y;

        m_rot.y += m_finalInput.x * m_inputSensitivity * Time.deltaTime;
        m_rot.x += m_finalInput.y * m_inputSensitivity * Time.deltaTime;

        m_rot.x = Mathf.Clamp(m_rot.x, -m_clampAngleLow, m_clampAngleHigh);

        Quaternion localRotation = Quaternion.Euler(m_rot.x, m_rot.y, 0.0f);
        transform.rotation = Quaternion.Slerp(transform.rotation, localRotation, Time.deltaTime * m_moveSpeed);
    }

    private void LateUpdate()
    {
        UpdateCamera();
    }

    public void SetIsZooming(bool p_isZooming)
    {
        m_isZooming = p_isZooming;
    }

    void UpdateCamera()
    {
        Transform target = m_cameraTarget.transform;
        if (m_isZooming)
        {
            target = m_zoomTarget.transform;
            m_cameraObject.SetZoom();
        }
        else
        {
            m_cameraObject.RestoreView();
        }

            
        float step = m_moveSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, target.position, step);
    }
}
