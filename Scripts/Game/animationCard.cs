using UnityEngine;

public class animationCard : MonoBehaviour
{
    public bool isMove = false;
    public float speed = 1f;
    public Vector3 vectorTaget = new Vector3(0,-7,8);
    private void FixedUpdate()
    {
        if (isMove)
        {
            if (transform.position != vectorTaget)
            {
                transform.position = Vector3.MoveTowards(transform.position, vectorTaget,speed);
            }
            else
            {
                isMove = false;
            }
        }
    }
}
