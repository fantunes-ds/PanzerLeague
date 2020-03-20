using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField]
    private float m_moveSpeed = 120.0f;
    [SerializeField]
    private GameObject m_CameraFollowObject;

    private Vector3 m_followPos;

    [SerializeField]
    private float m_clampAngle = 80.0f;
    [SerializeField]
    private float m_inputSensitivity = 150.0f;
    [SerializeField] 
    private GameObject m_cameraObject;    
    [SerializeField] 
    private GameObject m_playerObject;    
    [SerializeField] 
    private Vector3 m_cameraDistanceToPlayer;
    [SerializeField] 
    private Vector2 m_input;
    [SerializeField] 
    private Vector2 m_finalInput;    
    [SerializeField] 
    private Vector2 m_smooth;
    [SerializeField] 
    private Vector2 m_rot;

    
    
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
        float xInput = Input.GetAxis("Horizontal2");
        float yInput = Input.GetAxis("Vertical2");

        m_input.x = Input.GetAxis("Mouse X");
        m_input.y = Input.GetAxis("Mouse Y");
        m_finalInput.x = xInput + m_input.x;
        m_finalInput.y = yInput + m_input.y;

        m_rot.y += m_finalInput.x * m_inputSensitivity * Time.deltaTime;
        m_rot.x += m_finalInput.y * m_inputSensitivity * Time.deltaTime;

        m_rot.x = Mathf.Clamp(m_rot.x, -m_clampAngle, m_clampAngle);

        Quaternion localRotation = Quaternion.Euler(m_rot.x, m_rot.y, 0.0f);
        transform.rotation = localRotation;
    }

    private void LateUpdate()
    {
        UpdateCamera();
    }

    void UpdateCamera()
    {
        Transform target = m_CameraFollowObject.transform;

        float step = m_moveSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, target.position, step);
    }
}
