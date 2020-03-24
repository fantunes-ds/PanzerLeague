using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

[RequireComponent(typeof(LineRenderer))]
public class ProjectilePrediction : MonoBehaviour
{
    private Rigidbody m_tankRigidbodyRef;
    [SerializeField][Tooltip("The minimum speed of the overall shot")]
    private float m_initialSpeed = 5.0f;
    private float m_speed;
    [SerializeField][Range(2,60)][Tooltip("This setting makes the curve be smoother or sharper. It's the number of 'vertices'")]
    private float m_curveAccuracy = 20;
    [SerializeField][Range(1,10)][Tooltip("This setting makes the prediction go further or closer. WARNING : THIS MAY BE INTENSIVE")]
    private float m_maxCurveDistance = 1;
    [SerializeField][Tooltip("The velocity the bullet gets out from the canon at")]
    private Vector3 m_initialBulletVelocity;
    private Vector3 m_bulletVelocity;

    [SerializeField][Tooltip("The bullet prefab you want to shoot")]
    private GameObject m_bulletPrefab;
    private GameObject m_bullet;
    [SerializeField][Tooltip("The position you want the bullet to emerge from")]
    private Transform m_bulletOrigin;
    
    private LineRenderer m_lr;

    [SerializeField]
    private GameObject m_targetIndicator;
    
    // Start is called before the first frame update
    void Start()
    {
        m_tankRigidbodyRef = GetComponent<Rigidbody>();
        m_lr = GetComponent<LineRenderer>();
        m_bulletVelocity = m_bulletOrigin.forward * m_speed;
        m_bullet = Instantiate(m_bulletPrefab, new Vector3(-100, -100, -100), Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        Rigidbody rb = m_tankRigidbodyRef;
        m_speed = Mathf.Abs(rb.velocity.x) + Mathf.Abs(rb.velocity.y) + Mathf.Abs(rb.velocity.z) * 2 + m_initialSpeed;
        
        DrawTrajectory();
        
        if (Input.GetKeyDown(KeyCode.JoystickButton5) || Input.GetMouseButtonDown(0))
        {
            m_bullet.transform.position = m_bulletOrigin.position;
            m_bulletVelocity = m_bulletOrigin.forward * m_speed;
        }
        
        Vector3 point1 = m_bullet.transform.position;
        float stepSize = 1.0f / m_curveAccuracy;

        for (float i = 0; i < m_maxCurveDistance; i += stepSize)
        {
            m_bulletVelocity += Physics.gravity * stepSize * Time.deltaTime;
            Vector3 point2 = point1 + m_bulletVelocity * stepSize  * Time.deltaTime;

            Ray ray = new Ray(point1, point2 - point1);
            if (Physics.Raycast(ray, (point2 - point1).magnitude))
            {
                Debug.Log("HIT TARGET !");
            }
            point1 = point2;
        }
        
        m_bullet.transform.position = point1;
    }

    private void DrawTrajectory()
    {
        float stepSize = 1.0f / m_curveAccuracy;
        List<Vector3> points = new List<Vector3>();

        Vector3 point1 = m_bulletOrigin.position;
        Vector3 predictedBulletVelocity = m_bulletOrigin.forward * m_speed;
        points.Add(point1);

        for (float i = 0; i < m_maxCurveDistance * 4; i += stepSize)
        {
            predictedBulletVelocity += Physics.gravity * stepSize;
            Vector3 point2 = point1 +  predictedBulletVelocity * stepSize;
            points.Add(point2);
            RaycastHit hit;
            if (Physics.Raycast(point1, (point2 - point1), out hit, (point2 - point1).magnitude, ~8))
            {
                m_targetIndicator.transform.position = hit.point;
                break;
            }
            point1 = point2;
        }
        
        m_lr.positionCount = points.Count;
        m_lr.SetPositions(points.ToArray());
    }
    
    private void OnDrawGizmos()
    {
        float stepSize = 1.0f / m_curveAccuracy;
        
        Gizmos.color = Color.red;
        Vector3 point1 = m_bulletOrigin.position;
        Vector3 predictedBulletVelocity = m_bulletOrigin.forward * m_speed;
        
        for (float i = 0; i < m_maxCurveDistance * 4.0f; i += stepSize)
        {
            predictedBulletVelocity += Physics.gravity * stepSize;
            Vector3 point2 = point1 +  predictedBulletVelocity * stepSize;
            Gizmos.DrawLine(point1, point2);
            point1 = point2;
        }
    }
}
