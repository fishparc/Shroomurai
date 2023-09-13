using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class PlateOnPress : MonoBehaviour
{
    public Collider2D coll;
    public GameObject door;
    public GameObject alignTool;
    private bool isOpened = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnCollisionEnter2D(Collision2D collision) 
    {
        if (collision != null)
        {
            if (!isOpened)
            {
                if ((collision.gameObject.tag == "Player")||(collision.gameObject.tag == "Movable"))
                {
                    isOpened=true;
                    alignTool.transform.localScale = new Vector2(1f,0.2f);
                    AutomaticDoorTrigger automaticDoorTrigger = door.gameObject.GetComponent<AutomaticDoorTrigger>();
                    automaticDoorTrigger.Open();
                }
            }
        }
    }
}
