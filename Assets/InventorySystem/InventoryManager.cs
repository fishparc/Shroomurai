using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    #region Singleton
    public static InventoryManager Instance;
    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
        DontDestroyOnLoad(this);
    }
    #endregion
    public GameObject shadow;
    private bool Inventoryison;
    public GameObject Inventory;
    public List<Item> ItemList;
    public delegate void onInventoryChange();
    public onInventoryChange onInventoryCallBack;

    private void Start()
    {
        Inventoryison = false;
        Inventory.SetActive(false);
        shadow.SetActive(false);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.I))
        {
            if(!Inventoryison)
            {
                Inventoryison = true;
                Inventory.SetActive(true);
                shadow.SetActive(true);
            }
            else
            {
                Inventoryison = false;
                Inventory.SetActive(false);
                shadow.SetActive(false);
            }
        }
    }

    public void Add(Item newItem)
    {
        ItemList.Add(newItem);
    }

    public void Remove(Item oldItem)
    {
        ItemList.Remove(oldItem);
    }
}
            
