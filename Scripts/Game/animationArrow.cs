using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class animationArrow : MonoBehaviour
{
    bool runDown = true;
    public float y1 = 5f;
    public float y2 = 6f;
    public float speed = 0.1f;

    private void FixedUpdate()
    {
        Vector3 x = transform.position;
        if (runDown) 
        {
            x.y -= speed;
        }
        else
        {
            x.y += speed;
        }
        if (x.y < y1 && runDown)
        {
            runDown = !runDown;
        }
        if (x.y > y2 && !runDown)
        {
            runDown = !runDown;
        }
        transform.position = x;
    }
}
