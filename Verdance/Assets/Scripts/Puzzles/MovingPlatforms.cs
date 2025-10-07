using UnityEngine;

public class MovingPlatforms : MonoBehaviour
{
    public Transform posA, posB;
    public float speed;
    Vector3 targetPos;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        targetPos = posB.position;
    }

    // Update is called once per frame
    void Update()
    {
        // When the platform gets close to pos A the new target is pos B
        if(Vector2.Distance(transform.position, posA.position) < 0.05f)
        {
            targetPos = posB.position;
        }

        //When the platform gets close to pos B the new target pos A
        if(Vector2.Distance(transform.position, posB.position) < 0.05f)
        {
            targetPos = posA.position;
        }

        transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
    }

}
