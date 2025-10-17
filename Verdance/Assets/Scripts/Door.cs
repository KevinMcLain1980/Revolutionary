using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private object nextLevel;

   

    public void OpenChest(PlayerInventory inventory)
    {
        if(!LevelManager.Instance.CheckLevelCompletion())
        {
            Debug.Log("You must defeat all enemies first");
            return;
        }

        if (inventory != null && inventory.HasKey())
        {
            LevelManager.Instance.ForceCompleteLevel();
        }
        else
        {
            Debug.Log("Find the key!");
        }
    }
}
