using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class simplemove : MonoBehaviour
{
    public bool isReady = false;
    Rigidbody2D rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.D))
        {
            transform.Translate(Vector2.right * 8f * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.Translate(Vector2.left * 8f * Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.name == ("Frog"))
        {
            isReady = true;
            Debug.Log("yes");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.name == ("Frog"))
        {
            isReady = false;
            Debug.Log("no");
        }
    }
}
