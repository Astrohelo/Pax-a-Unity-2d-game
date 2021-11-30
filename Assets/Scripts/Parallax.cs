using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    public Camera cam;
    public Transform subject;
    Vector2 startPosition;
    float startZ;
    
    Vector2 travel=>(Vector2)cam.transform.position-startPosition;
    float distanceFromSubject => transform.position.z - subject.position.z;
    float clippingPlane => (cam.transform.position.z+(distanceFromSubject)>0? cam.farClipPlane:cam.nearClipPlane);
    float parallaxFactor=> Mathf.Abs(distanceFromSubject)/ clippingPlane;
    void Start()
    {
        startPosition = transform.position;
        startZ = transform.position.z;
    }

    void FixedUpdate()
    {
        Vector2 newPos = startPosition+ travel * parallaxFactor;
        float newY = startPosition.y+ travel.y * parallaxFactor*1.40F;
        transform. position = new Vector3(newPos.x,newY,startZ);
    }
}
