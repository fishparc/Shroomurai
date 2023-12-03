using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemContainer : MonoBehaviour
{
    public Item thisItem;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag == "Player")
        {
            InventoryManager.Instance.Add(thisItem);
            InventoryManager.Instance.onInventoryCallBack();
            Destroy(gameObject);
        }
    }

    private void OnMouseDown()
    {
        InventoryManager.Instance.Add(thisItem);
        InventoryManager.Instance.onInventoryCallBack();
        Destroy(gameObject);
    }
}
