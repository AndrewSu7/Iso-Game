using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapManager : MonoBehaviour {
    private static MapManager _instance;
    public static MapManager Instance { get { return _instance; } }

    public GameObject overlayPrefab;
    public GameObject overlayContainer;

    public Dictionary<Vector2Int, OverlayTile> map;
    public bool ignoreBottomTiles;

    private void Awake() {
        if (_instance != null && _instance != this) {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
    }

    // Start is called before the first frame update
    void Start() {
        var tileMap = gameObject.transform.GetComponentsInChildren<Tilemap>().OrderByDescending(x => x.GetComponent<TilemapRenderer>().sortingOrder);
        map = new Dictionary<Vector2Int, OverlayTile>();

         foreach (var tm in tileMap) {
            BoundsInt bounds = tm.cellBounds;
            for (int z = bounds.max.z; z >= bounds.min.z; z--) {
                for (int y = bounds.min.y; y < bounds.max.y; y++) {
                    for (int x = bounds.min.x; x < bounds.max.x; x++) {
                        if (z == 0 && ignoreBottomTiles)
                            return;

                        var tileLocation = new Vector3Int(x, y, z);
                        var tileKey = new Vector2Int(x, y);
                        if (tm.HasTile(tileLocation) && !map.ContainsKey(tileKey)) {
                            var overlayTile = Instantiate(overlayPrefab, overlayContainer.transform);
                            var cellWorldPosition = tm.GetCellCenterWorld(tileLocation);

                            overlayTile.transform.position = new Vector3(cellWorldPosition.x, cellWorldPosition.y, cellWorldPosition.z + 1);
                            overlayTile.GetComponentInChildren<SpriteRenderer>().sortingOrder = tm.GetComponent<TilemapRenderer>().sortingOrder;
                            overlayTile.gameObject.GetComponentInChildren<OverlayTile>().gridLocation = tileLocation;
                            map.Add(tileKey, overlayTile.gameObject.GetComponentInChildren<OverlayTile>());
                        }
                    }
                }
            }
        }
    }
    public List<OverlayTile> GetNeighbourTiles(OverlayTile currentOverlayTile) {
        List<OverlayTile> neighbours = new List<OverlayTile>();

        //top
        Vector2Int locationToCheck = new Vector2Int(currentOverlayTile.gridLocation.x, currentOverlayTile.gridLocation.y + 1);
        if (map.ContainsKey(locationToCheck)) {
            if (Mathf.Abs(currentOverlayTile.gridLocation.z - map[locationToCheck].gridLocation.z) <= 2){ 
            neighbours.Add(map[locationToCheck]);
            }
        }

        //botom
        locationToCheck = new Vector2Int(currentOverlayTile.gridLocation.x, currentOverlayTile.gridLocation.y - 1);
        if (map.ContainsKey(locationToCheck)) {
            if (Mathf.Abs(currentOverlayTile.gridLocation.z - map[locationToCheck].gridLocation.z) <= 2){
                neighbours.Add(map[locationToCheck]);
            }
        }

        //right
        locationToCheck = new Vector2Int(currentOverlayTile.gridLocation.x + 1, currentOverlayTile.gridLocation.y);
        if (map.ContainsKey(locationToCheck)) {
            if (Mathf.Abs(currentOverlayTile.gridLocation.z - map[locationToCheck].gridLocation.z) <= 2){
                neighbours.Add(map[locationToCheck]);
            }
        }

        //left
        locationToCheck = new Vector2Int(currentOverlayTile.gridLocation.x - 1, currentOverlayTile.gridLocation.y);
        if (map.ContainsKey(locationToCheck)) {
            if (Mathf.Abs(currentOverlayTile.gridLocation.z - map[locationToCheck].gridLocation.z) <= 2) {
                neighbours.Add(map[locationToCheck]);
            }
        }
        return neighbours;
    }

}
