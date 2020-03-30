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

    [SerializeField][Tooltip("The bullet prefab you want to shoot")]
    private GameObject m_bulletPrefab;

    [Tooltip("The position you want the bullet to emerge from")]
    public Transform m_bulletOrigin;
    
    private LineRenderer m_lr;

    [SerializeField]
    private GameObject m_targetIndicator;
    public GameObject m_uiTarget;
    
    public Camera m_tankCamera;

    private bool m_isShooting;
    
    // Start is called before the first frame update
    void Start()
    {
        m_tankRigidbodyRef = GetComponent<Rigidbody>();
        m_lr = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        Rigidbody rb = m_tankRigidbodyRef;
        m_speed = rb.velocity.magnitude + m_initialSpeed;
        
        DrawTrajectory();
        Shoot(0.0f);
    }

    public void Shoot(float p_additionalDamage)
    {
        if (!m_isShooting)
            return;

        GameObject m_bullet = Instantiate(m_bulletPrefab, m_bulletOrigin.transform.position, m_bulletOrigin.rotation);
        m_bullet.GetComponent<Bullet>().m_speed = m_speed;
        m_bullet.GetComponent<Bullet>().m_additionalDamage = p_additionalDamage;
        m_isShooting = false;
    }
    private void DrawTrajectory()
    {
        float stepSize = 1.0f / m_curveAccuracy;
        List<Vector3> points = new List<Vector3>();

        Vector3 point1 = m_bulletOrigin.position;
        Vector3 predictedBulletVelocity = m_bulletOrigin.forward * m_speed;
        points.Add(point1);

        LayerMask lm = ~(1 << 8);
        for (float i = 0; i < m_maxCurveDistance * 4; i += stepSize)
        {
            predictedBulletVelocity += Physics.gravity * stepSize;
            Vector3 point2 = point1 +  predictedBulletVelocity * stepSize;
            points.Add(point2);
            RaycastHit hit;
            if (Physics.Raycast(point1, (point2 - point1), out hit, (point2 - point1).magnitude, lm))
            {
                m_targetIndicator.transform.position = hit.point;
                m_uiTarget.transform.position = m_tankCamera.WorldToScreenPoint(m_targetIndicator.transform.position);
                break;
            }
            point1 = point2;
        }
        
        m_lr.positionCount = points.Count;
        m_lr.SetPositions(points.ToArray());
    }
    
    // Works only on the scene camera, for debug !
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
    public void SetIsShooting(bool p_isShooting)
    {
        m_isShooting = p_isShooting;
    }
}
