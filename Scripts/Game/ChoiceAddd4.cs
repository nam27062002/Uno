using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChoiceAddd4 : MonoBehaviour
{
    public bool isZoom = false;
    public float speed = 0.3f;
    public bool isChoice = false;
    public void FixedUpdate()
    {
        if (isZoom)
        {
            if (transform.position.z > -1)
            {
                Vector3 vector = transform.position;
                vector.z -= speed;
                transform.position = vector;
            }
            else
            {
                isZoom = false;
            }
        }
    }
    private void OnMouseEnter()
    {
        isZoom = true;      
    }
    private void OnMouseDown()
    {
        isChoice = true;
    }
    private void OnMouseExit()
    {
        Vector3 vector = transform.position;
        vector.z = 0;
        transform.position = vector;
    }
}
