
using Unity.Cinemachine;
using UnityEngine;

public class Pickups : MonoBehaviour
{
    //Only Adds to inventory!
    public Item item;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerInventory inventory = GetComponent<PlayerInventory>();
        if (inventory != null )
        {
            inventory.GetItem(1);
            Destroy(gameObject);
        }
    }


}
