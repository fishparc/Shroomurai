using System;
using UnityEngine;
using UnityEngine.UIElements;

public class Inventory : MonoBehaviour
{
    VisualElement rootVisualElement;
    void Awake()
    {
        rootVisualElement = GetComponent<UIDocument>().rootVisualElement;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.I))
        {
            rootVisualElement.style.display =
                rootVisualElement.style.display == DisplayStyle.Flex ?
                    DisplayStyle.None :
                    DisplayStyle.Flex;
        }
    }
}
