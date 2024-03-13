using System;
using System.Collections.Generic;
using UnityEngine;

public class GridData
{
    public Dictionary<Vector3Int, PlacementData> placedObjects = new(){};

    public void AddObjectAt(Vector3Int gridPos, Vector2Int objectSize, int ID, int placedObjectIndex, int rotationIndex)
    {
        List<Vector3Int> positionToOccupy = CalculatePositions(gridPos, objectSize);
        PlacementData data = new PlacementData(positionToOccupy, ID, placedObjectIndex, rotationIndex);
        foreach (var pos in positionToOccupy)
        {
            if (placedObjects.ContainsKey(pos))
            {
                throw new Exception($"Dictionary already contains this cell position {pos}");
            }
            placedObjects[pos] = data;
        }
    }

    private List<Vector3Int> CalculatePositions(Vector3Int gridPos, Vector2Int objectSize)
    {
        List<Vector3Int> returnVal = new();
        for (int x = 0; x < objectSize.x; x++)
        {
            for (int y = 0; y < objectSize.y; y++)
            {
                returnVal.Add(gridPos + new Vector3Int(x, 0, y));
            }
        }
        return returnVal;
    }
    public bool CanPlaceObjectAt(Vector3Int gridPos, Vector2Int objectSize)
    {

        List<Vector3Int> positionToOccupy = CalculatePositions(gridPos, objectSize);
        foreach (var pos in positionToOccupy)
        {
            if (placedObjects.ContainsKey(pos))
            {
                return false;
            }
        }
        return true;
    }
    public bool CanPlaceTileAt(Vector3Int gridPos)
    {
        List<Vector3Int> positionToOccupy = new List<Vector3Int>() { gridPos };
        foreach (var pos in positionToOccupy)
        {
            if (placedObjects.ContainsKey(pos))
            {
                return false;
            }
        }
        if (placedObjects.ContainsKey(gridPos + new Vector3Int(-1, 0, 0)) || placedObjects.ContainsKey(gridPos + new Vector3Int(1, 0, 0)) || placedObjects.ContainsKey(gridPos + new Vector3Int(0, 0, -1)) || placedObjects.ContainsKey(gridPos + new Vector3Int(0, 0, 1))){
            return true;
        }
        else
        {
            return false;
        }
    }
}


public class PlacementData
{
    public List<Vector3Int> occupiedPositions;
    public int ID { get; private set; }
    public int PlacedObjectIndex { get; private set; }
    public int PlacedRotation { get; private set; }

    public PlacementData(List<Vector3Int> occupiedPositions, int iD, int placedObjectIndex, int rotationIndex)
    {
        this.occupiedPositions = occupiedPositions;
        ID = iD;
        PlacedObjectIndex = placedObjectIndex;
        PlacedRotation = rotationIndex;
    }

    public PlacementData(){}
}
