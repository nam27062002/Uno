using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spin : MonoBehaviour
{
    public float speed = 4f;
    private void FixedUpdate()
    {
        transform.Rotate(0, 0, speed);
    }
}
