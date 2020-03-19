using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

public class ProjectilePrediction : MonoBehaviour
{
    [SerializeField]
    private float m_speed = 30.0f;
    [SerializeField][Range(2,60)]
    private float m_curveAccuracy = 6;
    [SerializeField][Range(1,10)]
    private float m_maxCurveDistance = 1;
    [SerializeField]
    private Vector3 m_initialBulletVelocity;
    private Vector3 m_bulletVelocity;

    [SerializeField]
    private GameObject m_bulletPrefab;
    private GameObject m_bullet;
    
    // Start is called before the first frame update
    void Start()    
    {
        m_bulletVelocity = transform.forward * m_speed;
        m_bullet = Instantiate(m_bulletPrefab, transform.position, Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.JoystickButton2))
        {
            m_bullet.transform.position = transform.position;
            m_bulletVelocity = transform.forward * m_speed;
        }
        
        Vector3 point1 = m_bullet.transform.position;
        float stepSize = 1.0f / m_curveAccuracy;

        for (float i = 0; i < 1; i += stepSize)
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
    
    private void OnDrawGizmos()
    {
        float stepSize = 1.0f / m_curveAccuracy;
        
        Gizmos.color = Color.red;
        Vector3 point1 = transform.position;
        Vector3 predictedBulletVelocity = transform.forward * m_speed;
        
        for (float i = 0; i < m_maxCurveDistance; i += stepSize)
        {
            predictedBulletVelocity += Physics.gravity * stepSize;
            Vector3 point2 = point1 +  predictedBulletVelocity * stepSize;
            Gizmos.DrawLine(point1, point2);
            point1 = point2;
        }
    }
}
