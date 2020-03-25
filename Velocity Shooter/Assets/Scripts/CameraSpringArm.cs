using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraSpringArm : MonoBehaviour
{
    [SerializeField][Range(0f, 5f)]
    private float m_minDistance = 1.0f;
    [SerializeField][Range(1f, 6f)]
    private float m_maxDistance = 4.0f;

    private float m_oldMaxDistance;
    [SerializeField][Range(1f, 20f)]
    private float m_transitionSpeed = 10.0f;
    private Vector3 m_direction;
    [SerializeField]
    private Vector3 m_directionAdjusted;
    [SerializeField]
    private float m_distance;
    
    void Awake()
    {
        m_direction = transform.localPosition.normalized;
        m_distance = transform.localPosition.magnitude;
        m_oldMaxDistance = m_maxDistance;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 targetPos = transform.parent.TransformPoint(m_direction * m_maxDistance);

        RaycastHit hit;
        
        if (Physics.Linecast(transform.parent.position, targetPos, out hit))
        {   m_distance = Mathf.Clamp((hit.distance * 0.8f), m_minDistance, m_maxDistance);}
        else
        {m_distance = m_maxDistance;}

        transform.localPosition =
            Vector3.Lerp(transform.localPosition, m_direction * m_distance, Time.deltaTime * m_transitionSpeed);
    }

    public void SetZoom()
    {
        m_maxDistance = 1;
    }
    
    public void RestoreView()
    {
        m_maxDistance = m_oldMaxDistance;
    }
}
