using UnityEngine;

public class BackgroundController : MonoBehaviour
{
    private float startPos, startPosY, length;
    public GameObject cam;
    public float parallaxEffect;


    void Start()
    {
        startPos = transform.position.x;
        startPosY = transform.position.y;
        length = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    // Update is called once per frame
    void Update()
    {
        //Calculate dist based on cam movement
        float distance = cam.transform.position.x  * parallaxEffect;
        float distanceY = cam.transform.position.y * parallaxEffect;

        float movement = cam.transform.position.x * (1 - parallaxEffect);
        
        transform.position = new Vector3(startPos + distance, startPosY + distanceY, transform.position.z);

        //Adjust image for infinite scrolling
        if (movement > startPos + length)
        {
            startPos += length;
        }else if (movement < startPos - length)
        {
            startPos -= length;
        }
    }
}
