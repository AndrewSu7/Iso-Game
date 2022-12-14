using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathFinder {
    public List<OverlayTile> FindPath(OverlayTile start, OverlayTile end, List<OverlayTile> searchableTiles) {

        List<OverlayTile> openList = new List<OverlayTile>(); //list of tile we want to check next loop
        List<OverlayTile> closedList = new List<OverlayTile>(); // list of tiles we no longer want to check

        openList.Add(start);

        while (openList.Count > 0) {
            OverlayTile currentOverlayTile = openList.OrderBy(x => x.T).First();

            openList.Remove(currentOverlayTile);
            closedList.Add(currentOverlayTile);

            if (currentOverlayTile == end) {
                //finialize path
                return GetFinishedList(start, end);
            }

            var neighbourTiles = MapManager.Instance.GetNeighbourTiles(currentOverlayTile, searchableTiles);
            foreach (var neighbour in neighbourTiles) {                //jump height = 1
                if (neighbour.isBlocked || closedList.Contains(neighbour)) {
                    continue;
                }

                neighbour.S = GetManhattenDistance(start, neighbour);
                neighbour.E = GetManhattenDistance(end, neighbour);
                neighbour.previous = currentOverlayTile;

                if (!openList.Contains(neighbour)) {
                    openList.Add(neighbour);
                }
            }
        }
        return new List<OverlayTile>();
    }

    private List<OverlayTile> GetFinishedList(OverlayTile start, OverlayTile end) {
        List<OverlayTile> finishedList = new List<OverlayTile>();
        OverlayTile currentTile = end;

        while (currentTile != start) {
            finishedList.Add(currentTile);
            currentTile = currentTile.previous;
        }
        finishedList.Reverse();
        return finishedList;
    }

    private int GetManhattenDistance(OverlayTile start, OverlayTile neighbour) {
        return Mathf.Abs(start.gridLocation.x - neighbour.gridLocation.x) + Mathf.Abs(start.gridLocation.y - neighbour.gridLocation.y);
    }

    private List<OverlayTile> GetNeighbourTiles(OverlayTile currentOverlayTile) {
        var map = MapManager.Instance.map;

        List<OverlayTile> neighbours = new List<OverlayTile>();

        //top
        Vector2Int locationToCheck = new Vector2Int(currentOverlayTile.gridLocation.x, currentOverlayTile.gridLocation.y + 1);
        if (map.ContainsKey(locationToCheck)) {
            neighbours.Add(map[locationToCheck]);
        }

        //botom
         locationToCheck = new Vector2Int(currentOverlayTile.gridLocation.x, currentOverlayTile.gridLocation.y - 1);
        if (map.ContainsKey(locationToCheck)) {
            neighbours.Add(map[locationToCheck]);
        }

        //right
        locationToCheck = new Vector2Int(currentOverlayTile.gridLocation.x + 1, currentOverlayTile.gridLocation.y);
        if (map.ContainsKey(locationToCheck)) {
            neighbours.Add(map[locationToCheck]);
        }

        //left
        locationToCheck = new Vector2Int(currentOverlayTile.gridLocation.x - 1, currentOverlayTile.gridLocation.y);
        if (map.ContainsKey(locationToCheck)) {
            neighbours.Add(map[locationToCheck]);
        }
        return neighbours;
    }
}
