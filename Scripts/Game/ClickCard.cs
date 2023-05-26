using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public class ClickCard : MonoBehaviour
{
    public List<Transform> listTransform;
    public int indexChoice = -1;
    protected void OnMouseDown()
    {   
        if (transform.position.y == -6f)
        {
            foreach (Transform i in listTransform)
            {
                if (i.position.y == -5f)
                {
                    Vector3 vector = i.position;
                    vector.y -= 1;
                    i.position = vector;
                    break;
                }
            }
            Vector3 vector1 = transform.position;
            vector1.y += 1;
            transform.position = vector1;
        }
        else if (transform.position.y == -5f && indexChoice == -1)
        {
            int count = 0;
            foreach (Transform i in listTransform)
            {
                if (i.position.y == -5f)
                {
                    indexChoice = count;
                }
                else
                {
                    Vector3 vector = i.position;
                    vector.y = -7f;
                    i.position = vector;
                }
                count++;
            }
        }
    }
}
