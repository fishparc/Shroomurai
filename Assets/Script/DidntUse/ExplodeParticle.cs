using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodeParticle : MonoBehaviour
{
    public float pieceSize = 0.2f;
    public int pieceInRow = 5;
    float piecePivotDistance;
    Vector3 piecePivot;

    private void Start() {
        piecePivotDistance = pieceSize * pieceInRow / 2;

        piecePivot = new Vector3(piecePivotDistance , piecePivotDistance);
    }


    private void Update() {
         if(Input.GetKeyDown(KeyCode.M)){
            explode();
        }
    }


    public void explode()
    {
        gameObject.SetActive(false);
        
        for(int x = 0 ; x < pieceInRow ; x ++)
        {
            for(int y = 0 ; y < pieceInRow ; y ++)
            {
                createPiece(x , y);
            }
        }
    }

    void createPiece(int x , int y)
    {
        GameObject piece;

        piece = GameObject.CreatePrimitive(PrimitiveType.Cube);

        piece.transform.position = transform.position + new Vector3(pieceSize *x , pieceSize * y) - piecePivot;
        piece.transform.localScale = new Vector3(pieceSize , pieceSize);

        piece.AddComponent<Rigidbody2D>();
        piece.GetComponent<Rigidbody2D>().mass = pieceSize;
    }
}
