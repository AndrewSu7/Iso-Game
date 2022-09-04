using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MouseController : MonoBehaviour {

    public int movementRange;


    public float speed;
    public GameObject cursor;
    public GameObject characterPrefab;
    private CharacterInfo _characterInfo;


    private PathFinder _pathFinder;
    private RangeFinder _rangeFinder;
    private List<OverlayTile> path = new List<OverlayTile>();
    private List<OverlayTile> inRangeTiles = new List<OverlayTile>();

    private void Start() {
        _pathFinder = new PathFinder();
        _rangeFinder = new RangeFinder();
    }

    void LateUpdate() {
        RaycastHit2D? hit = GetFocusedOnTile();

        if (hit.HasValue) {
            OverlayTile overlayTile = hit.Value.collider.gameObject.GetComponentInChildren<OverlayTile>();
            cursor.transform.position = overlayTile.transform.position;
            cursor.GetComponent<SpriteRenderer>().sortingOrder = overlayTile.GetComponentInChildren<SpriteRenderer>().sortingOrder;

            if (Input.GetMouseButtonDown(0)) {
                overlayTile.gameObject.GetComponentInChildren<OverlayTile>().ShowTile();
                overlayTile.gameObject.GetComponentInChildren<OverlayTile>().HideTile();

                if (_characterInfo == null) {
                    _characterInfo = Instantiate(characterPrefab).GetComponent<CharacterInfo>();
                    PositionCharacterOnTile(overlayTile);
                    GetInRangeTiles();
                   // _characterInfo.activeTile = overlayTile;
                } else {
                    path = _pathFinder.FindPath(_characterInfo.activeTile, overlayTile, inRangeTiles);
                 
                }
            }
        }
        if (path.Count > 0) {
            MoveAlongPath();
        }
    }

    private void GetInRangeTiles() {
        foreach (var item in inRangeTiles) {
            item.HideTile();
        }
        inRangeTiles = _rangeFinder.GetTilesInRange(_characterInfo.activeTile, movementRange);

        foreach (var item in inRangeTiles) {
            item.ShowTile();
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

        if (path.Count == 0) { 
        GetInRangeTiles();
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
