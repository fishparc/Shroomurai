using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindArea : MonoBehaviour
{
    [SerializeField] private float windSpeed;
    [SerializeField] private float windAccel;
    public Transform windSource; 
    public Transform windEnd;
    public float windDrag , airDrag;
    public float inWindMass = 1 , inWindGravity = 2;
    private bool inWind;
    private float distance , length;
    private Vector3 windDir;
    private float objMass , objGravity;
    public bool isHorizontal;
    private float angleRad;
    private Rigidbody2D RB;

    private void Start() {

        length = (windEnd.position - windSource.position).magnitude;
        angleRad = (transform.localEulerAngles.z + 90f) * Mathf.Deg2Rad;
        //Debug.Log(transform.localEulerAngles.z);
        if(transform.localEulerAngles.z == 0 || transform.localEulerAngles.z == 180)
            isHorizontal = false;
        else
            isHorizontal = true;
    }
    private void FixedUpdate() {

    }
    private void OnTriggerEnter2D(Collider2D collision) {
        if(collision.tag == "WindArea")
            return;
        RB = collision.GetComponent<Rigidbody2D>();

        if(RB != null)
        {
            objMass = RB.mass;
            objGravity = RB.gravityScale;

            if(collision.tag != "Player")
            {
                RB.mass = inWindMass;
                RB.gravityScale = inWindGravity;
            }
        }

    }
    private void OnTriggerStay2D(Collider2D collision) {

        if(collision.tag == "WindArea")
            return;
        RB = collision.GetComponentInParent<Rigidbody2D>();

        windDir = new Vector3(Mathf.Cos(angleRad) , Mathf.Sin(angleRad) , 0);
        Vector3 objDir = collision.GetComponent<Transform>().position - windEnd.position;
        Vector3 moveDir = Vector3.Dot(objDir , windDir) * windDir;
        distance = moveDir.magnitude;

        if(windDir + moveDir/moveDir.magnitude == Vector3.zero)
        {
            inWind = true;
        }
        else
        {
            inWind = false;

        }
        if(inWind)
        {
            SwitchState(RB , true);
        }
        else
        {
            SwitchState(RB , false);
        }
        if(inWind)
        {
            float windPercent;
            if(isHorizontal)
                windPercent = 1f;
            else
                windPercent = distance/length;

            if(windPercent < 0.6f && windPercent > 0.15f)
                windPercent = 0.6f;
            else if(windPercent <= 0.15f)
                windPercent = 0f;

            float speedDif = windSpeed*(windPercent) - RB.velocity.y;
		    float movement = speedDif * windAccel;

            RB.AddForce(Mathf.Abs(movement) * windDir);
        }
        /*

        float speedDif = windSpeed*(windPercent) - RB.velocity.y;
		float movement = speedDif * windAccel;

        //movement = Mathf.Clamp(movement, -Mathf.Abs(speedDif)  * (1 / Time.fixedDeltaTime), Mathf.Abs(speedDif) * (1 / Time.fixedDeltaTime));

        Debug.Log(windPercent);

        //RB.AddForce(movement * windDir);
        //RB.AddForce(windSpeed * windDir * distance);
        */
    }
    private void OnTriggerExit2D(Collider2D collision) {
        if(collision.tag == "WindArea")
            return;
        Rigidbody2D RB = collision.GetComponentInParent<Rigidbody2D>();
        inWind = false;
        if(RB != null)
        {
            if(RB.tag != "Player")
            {
                RB.mass = objMass;
                RB.gravityScale = objGravity;
            }
            RB.drag = 0;
        }
    }

    void SwitchState(Rigidbody2D RB , bool isInWind)
    {
        if(isInWind)
        {
            RB.drag = windDrag;
        }
        else
            RB.drag = airDrag;
    }
}
