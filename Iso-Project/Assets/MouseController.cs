using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MouseController : MonoBehaviour {


    public GameObject cursor;
    public GameObject characterPrefab;
    public float speed;
    private CharacterInfo _characterInfo;


    private PathFinder _pathFinder;
    private List<OverlayTile> path;

    private void Start() {
        _pathFinder = new PathFinder();
        path = new List<OverlayTile>();
    }

    void LateUpdate() {
        RaycastHit2D? hit = GetFocusedOnTile();

        if (hit.HasValue) {
            OverlayTile overlayTile = hit.Value.collider.gameObject.GetComponentInChildren<OverlayTile>();
            cursor.transform.position = overlayTile.transform.position;
            cursor.GetComponent<SpriteRenderer>().sortingOrder = overlayTile.transform.GetComponentInChildren<SpriteRenderer>().sortingOrder;

            if (Input.GetMouseButtonDown(0)) {
                overlayTile.gameObject.GetComponentInChildren<OverlayTile>().ShowTile();

                if (_characterInfo == null) {
                    _characterInfo = Instantiate(characterPrefab).GetComponent<CharacterInfo>();
                    PositionCharacterOnTile(overlayTile);
                    _characterInfo.activeTile = overlayTile;
                } else {
                    path = _pathFinder.FindPath(_characterInfo.activeTile, overlayTile);
                   // overlayTile.gameObject.GetComponentInChildren<OverlayTile>().HideTile();
                }
            }
        }
        if (path.Count > 0) {
            MoveAlongPath();
        }
    }

    private void MoveAlongPath() {
        var step = speed * Time.deltaTime;

        var zIndex = path[0].transform.position.z;
        _characterInfo.transform.position = Vector2.MoveTowards(_characterInfo.transform.position, path[0].transform.position, step);
        _characterInfo.transform.position = new Vector3(_characterInfo.transform.position.x, _characterInfo.transform.position.y, zIndex);

        if (Vector2.Distance(_characterInfo.transform.position, path[0].transform.position) < 0.0001f) {
            PositionCharacterOnTile(path[0]);
            path.RemoveAt(0);
        }
    }

    public RaycastHit2D? GetFocusedOnTile() {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePos2d = new Vector2(mousePos.x, mousePos.y);

        RaycastHit2D[] hits = Physics2D.RaycastAll(mousePos2d, Vector2.zero);

        if (hits.Length > 0) {
            return hits.OrderByDescending(i => i.collider.transform.position.z).First();
        }

        return null;
    }

    private void PositionCharacterOnTile(OverlayTile tile) {
        _characterInfo.transform.position = new Vector3(tile.transform.position.x, tile.transform.position.y + 0.0001f, tile.transform.position.z);
        _characterInfo.GetComponent<SpriteRenderer>().sortingOrder = tile.GetComponent<SpriteRenderer>().sortingOrder;
        _characterInfo.activeTile = tile;
    }
}
