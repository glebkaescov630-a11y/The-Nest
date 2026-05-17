using UnityEngine;

public class QuickPickup : MonoBehaviour
{
    public int itemID = 1;

    void OnMouseDown()
    {
        if (itemID == 1) UseModeManager.Instance.pickedClockHand1 = true;
        if (itemID == 2) UseModeManager.Instance.pickedClockHand2 = true;
        if (itemID == 3) UseModeManager.Instance.pickedEmptyClock = true;

        Debug.Log($"Предмет с ID {itemID} сохранен в UseModeManager!");
        Destroy(gameObject); 
    }
}