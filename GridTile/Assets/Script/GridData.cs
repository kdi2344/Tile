using System;
using System.Collections.Generic;
using UnityEngine;

public class GridData
{
    public Dictionary<Vector3Int, PlacementData> placedObjects = new(){};
    public Dictionary<Vector3Int, PlacementData> placedTiles = new(){};

    public void AddObjectAt(Vector3Int gridPos, Vector2Int objectSize, int ID, int placedObjectIndex, int rot)
    {
        List<Vector3Int> positionToOccupy = CalculatePositions(gridPos, objectSize, rot);
        PlacementData data = new PlacementData(positionToOccupy, ID, placedObjectIndex, rot);
        if (ID == 0) //타일 설치시
        {
            foreach (var pos in positionToOccupy)
            {
                if (placedTiles.ContainsKey(pos))
                {
                    throw new Exception($"Dictionary already contains this cell position {pos}");
                }
                placedTiles[pos] = data;
            }
        }
        else if (ID == 999) //타일 설치해서 작은 사이즈로 자동 설치
        {
            foreach (var pos in positionToOccupy)
            {
                if (placedTiles.ContainsKey(pos))
                {
                    throw new Exception($"Dictionary already contains this cell position {pos}");
                }
                placedTiles[pos] = data;
            }
        }
        else //가구 설치시
        {
            foreach (var pos in positionToOccupy)
            {
                if (placedObjects.ContainsKey(pos))
                {
                    throw new Exception($"Dictionary already contains this cell position {pos}");
                }
                placedObjects[pos] = data;
            }
        }
    }

    private List<Vector3Int> CalculatePositions(Vector3Int gridPos, Vector2Int objectSize, int rot)
    {
        List<Vector3Int> returnVal = new();
        if (rot == 0)
        {
            for (int x = 0; x < objectSize.x; x++)
            {
                for (int y = 0; y < objectSize.y; y++)
                {
                    returnVal.Add(gridPos + new Vector3Int(x, 0, y));
                }
            }
        }
        else if (rot == 1)
        {
            for (int y = 0; y < objectSize.y; y++)
            {
                for (int x = 0; x < objectSize.x; x++)
                {
                    returnVal.Add(gridPos + new Vector3Int(y, 0, x));
                }
            }
        }
        else if (rot == 2)
        {
            for (int x = 0; x < objectSize.x; x++)
            {
                for (int y = 0; y < objectSize.y; y++)
                {
                    returnVal.Add(gridPos - new Vector3Int(x, 0, y));
                }
            }
        }
        else
        {
            for (int y = 0; y < objectSize.y; y++)
            {
                for (int x = 0; x < objectSize.x; x++)
                {
                    returnVal.Add(gridPos - new Vector3Int(y, 0, x));
                }
            }
        }
        return returnVal;
    }

    public bool CanPlaceObjectAt(Vector3Int gridPos, Vector2Int objectSize, int rot)
    {
        List<Vector3Int> positionToOccupy = CalculatePositions(gridPos, objectSize, rot); //설치할 곳 전부
        foreach (var pos in positionToOccupy)
        {
            if (placedObjects.ContainsKey(pos))
            {
                return false;
            }
        }
        return true;
    }
    public bool CanPlaceTileAt(Vector3Int gridPos) //큰 타일 설치 가능한지 확인
    {
        List<Vector3Int> positionToOccupy = new List<Vector3Int>() { gridPos };
        foreach (var pos in positionToOccupy)
        {
            if (placedTiles.ContainsKey(pos))
            {
                return false;
            }
        }
        if (placedTiles.ContainsKey(gridPos + new Vector3Int(-1, 0, 0)) || placedTiles.ContainsKey(gridPos + new Vector3Int(1, 0, 0)) || placedTiles.ContainsKey(gridPos + new Vector3Int(0, 0, -1)) || placedTiles.ContainsKey(gridPos + new Vector3Int(0, 0, 1))){
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
