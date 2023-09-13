using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionEffect : MonoBehaviour {

    public float cubeSize = 0.2f;
    public int cubesInRow = 5;

    public Sprite texture;
    float cubesPivotDistance;
    public Vector3 cubesPivot;
    GameObject pieces;
    Component[] piececom;
    public LayerMask pieceLayer;
    public Transform bomb;
    public Collider2D[] colliders;
    public float explosionForce = 200f;
    public float explosionRadiusMulti = 4f;
    public float explosionUpward = 0.5f;
    public float pieceMass = 5f;
    public float pieceGravity = 2f;
    public float cleanDelayStart = 2f;
    public float cleanDelayEnd = 4f;

    // Use this for initialization
    void Start() {

        
        //calculate pivot distance
        cubesPivotDistance = cubeSize * cubesInRow / 2;
        //use this value to create pivot vector)
        cubesPivot = new Vector3(cubesPivotDistance, cubesPivotDistance, cubesPivotDistance);

    }

    // Update is called once per frame
    private void Update() {

        //手動觸發解體
        if(Input.GetKeyDown(KeyCode.M)){
            Disassemble();
        }
    }
    public void Disassemble() {
        
        //創造小碎塊的父類別，方便處理
        pieces = new GameObject("pieces");

        //loop 3 times to create 5x5 pieces in x,y coordinates
        for (int x = 0; x < cubesInRow; x++) {
            for (int y = 0; y < cubesInRow; y++) {
                CreatePiece(x, y);
            }
        }
        //關閉本體
        gameObject.SetActive(false);
        //get explosion position
        //Vector2 explosionPos = transform.position;
        //get colliders in that position and radius
        //colliders = Physics2D.OverlapCircleAll(explosionPos, explosionRadius);
        //add explosion force to all colliders in that overlap sphere
        /*
        foreach (Collider2D hit in colliders) {
            //get rigidbody from collider object
            Rigidbody2D rb = hit.GetComponent<Rigidbody2D>();
            if (rb != null) {
                //add explosion force to this body with given parameters
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius, explosionUpward);
            }
        }
        */

        //取得所有碎塊的component

        
        piececom = pieces.GetComponentsInChildren(typeof(Collider2D));
        /*

        Vector2 distanceVector = transform.position - bomb.transform.position;

        foreach(Collider2D piec in piececom)
        {
            piec.GetComponent<Rigidbody2D>().AddRelativeForce(distanceVector.normalized * explosionForce , ForceMode2D.Impulse);
        }
        */

        //清除碎塊
        Clean();
    }

    void CreatePiece(int x, int y) {

        //create piece
        GameObject piece;
        piece = new GameObject("piece");

        //將創造出來的碎塊放入父類別
        piece.transform.parent = pieces.transform;

        //set piece position and scale
        piece.transform.position = transform.position + new Vector3(cubeSize * x, cubeSize * y) - cubesPivot;
        piece.transform.localScale = new Vector3(cubeSize, cubeSize, cubeSize);

        //給碎塊剛體、碰撞、材質、物理、碰撞圖層
        piece.AddComponent<Rigidbody2D>();
        piece.AddComponent<BoxCollider2D>();
        piece.AddComponent<SpriteRenderer>();
        piece.GetComponent<SpriteRenderer>().sprite = texture;
        piece.GetComponent<Rigidbody2D>().mass = pieceMass;
        piece.GetComponent<Rigidbody2D>().gravityScale = pieceGravity;
        piece.layer = LayerMask.NameToLayer("ParticleLayer");

        Vector2 distanceVector = piece.transform.position - bomb.transform.position;

        piece.GetComponent<Rigidbody2D>().AddRelativeForce(distanceVector.normalized * explosionForce , ForceMode2D.Impulse);
        
    }

    public void Clean()
    {
        foreach (Collider2D pieceColl in piececom) {
            Destroy(pieceColl.gameObject , UnityEngine.Random.Range(cleanDelayStart , cleanDelayEnd));
        }
        Invoke("KillParent" , cleanDelayEnd);
    }
    public void KillParent()
    {
        Destroy(pieces);
    }
    
}
