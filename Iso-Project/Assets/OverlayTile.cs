using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverlayTile : MonoBehaviour {

    public int S; //start dist
    public int E; //end dist
    public int T { get { return S + E; } } // total dist

    public bool isBlocked = false;
    public OverlayTile previous;

    public Vector3Int gridLocation;


    /*private void Update() {
        if (Input.GetMouseButtonDown(0)) {
            HideTile();
        }
    }*/

    public void HideTile() {
        gameObject.GetComponentInChildren<SpriteRenderer>().color = new Color(1, 1, 1, 0);
    }
    public void ShowTile() {
        gameObject.GetComponentInChildren<SpriteRenderer>().color = new Color(1, 1, 1, 1);
    }

}
