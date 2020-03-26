using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [HideInInspector]
    public float m_speed;
    private Vector3 m_bulletVelocity;

    private void Start()
    {
        StartCoroutine(AutoDestroy(5.0f));
        m_bulletVelocity = transform.forward * m_speed;
    }

    IEnumerator AutoDestroy(float p_seconds)
    {
        yield return new WaitForSeconds(p_seconds);
        Destroy(gameObject);
    }
    
    // Update is called once per frame
    void Update()
    {

        Vector3 point1 = transform.position;
        float stepSize = 1.0f / 20.0f;

        for (float i = 0; i < 1; i += stepSize)
        {
            m_bulletVelocity += Physics.gravity * stepSize * Time.deltaTime;
            Vector3 point2 = point1 + m_bulletVelocity * stepSize * Time.deltaTime;

            Ray ray = new Ray(point1, point2 - point1);
            if (Physics.Raycast(ray, (point2 - point1).magnitude))
            {
                Destroy(gameObject);
            }

            point1 = point2;
        }

        transform.position = point1;
    }
}
