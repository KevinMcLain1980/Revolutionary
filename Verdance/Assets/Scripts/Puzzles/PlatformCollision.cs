using UnityEngine;

public class PlatformCollision : MonoBehaviour
{
    //When the player touches the platform it becomes a child of the platform
    //So the player moves along with it
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.transform.parent = this.transform;
        }
    }
    //On exit the player is no longer a child
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.transform.parent = null;
        }
    }
}
