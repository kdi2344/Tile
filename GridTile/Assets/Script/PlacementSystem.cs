using System;
using System.Collections.Generic;
using UnityEngine;

public class PlacementSystem : MonoBehaviour
{
    [SerializeField] private GameObject mouseIndicator;
    [SerializeField] private InputManager inputManager;
    [SerializeField] private Grid gridTile;
    [SerializeField] private Grid gridFurniture;
    [SerializeField] private ObjectsDatabase database;
    private int selectedObjectIndex = -1;
    [SerializeField] private GameObject gridTileVisualization;
    [SerializeField] private GameObject gridFurnVisualization;
    private GridData floorData, furnitureData;
    [SerializeField] private Renderer[] previewRendererTile, previewRendererFurn;

    public List<GameObject> placedGameObject;
    public List<GameObject> placedTile;

    [SerializeField] private Transform tileParent;
    [SerializeField] private Transform furnParent;

    [SerializeField] private PreviewSystem previewSystem;

    private Vector3Int lastDetectedPosition = Vector3Int.zero;

    private int rot = 0;
    private bool isTile =false;
    private bool isRemove = false;

    private int start = 0;

    private void Start()
    {
        start = 0;
        StopPlacement();
        floorData = new();
        furnitureData = new();

        floorData.AddObjectAt(new Vector3Int(4, 0, 4), Vector2Int.one, 0, 0, 0);
        floorData.AddObjectAt(new Vector3Int(4, 0, 3), Vector2Int.one, 0, 1, 0);
        floorData.AddObjectAt(new Vector3Int(3, 0, 4), Vector2Int.one, 0, 2, 0);
        floorData.AddObjectAt(new Vector3Int(3, 0, 3), Vector2Int.one, 0, 3, 0);
        furnitureData.AddObjectAt(new Vector3Int(6, 0, 6), new Vector2Int(4, 4), 999, placedGameObject.Count-1, 0);
    }
    private void Update()
    {
        Vector3 mousePos = inputManager.GetSelectedMapPosition();
        Vector3Int gridPos;
        if (isRemove)
        {
            gridPos = gridFurniture.WorldToCell(mousePos);
            mouseIndicator.transform.position = mousePos;
            previewSystem.UpdateRemovePosition(gridFurniture.CellToWorld(gridPos));
            return;
        }
        if (selectedObjectIndex < 0)
        {
            return;
        }
        if (isTile)
        {
            gridPos = gridTile.WorldToCell(mousePos);
            if (lastDetectedPosition != gridPos)
            {
                bool placementValidity = CheckPlacementValidity(gridPos, selectedObjectIndex, rot);
                foreach (Renderer render in previewRendererTile)
                {
                    render.material.color = placementValidity ? Color.white : Color.red;
                }
                previewSystem.UpdatePosition(gridTile.CellToWorld(gridPos), placementValidity);
                lastDetectedPosition = gridPos;
            }
        }
        else
        {
            gridPos = gridFurniture.WorldToCell(mousePos);
            if (lastDetectedPosition != gridPos)
            {
                bool placementValidity = CheckPlacementValidity(gridPos, selectedObjectIndex, rot);
                foreach (Renderer render in previewRendererFurn)
                {
                    render.material.color = placementValidity ? Color.white : Color.red;
                }
                previewSystem.UpdatePosition(gridFurniture.CellToWorld(gridPos), placementValidity);
                lastDetectedPosition = gridPos;
            }
        }
        mouseIndicator.transform.position = mousePos;
    }

    public void StartPlacement(int ID) //놓기 시작
    {
        rot = 0;
        selectedObjectIndex = database.objectData.FindIndex(data => data.ID == ID);
        if (selectedObjectIndex < 0)
        {
            Debug.Log("읎음");
            return;
        }
        if (ID == 0)
        {
            gridTileVisualization.SetActive(true);
            gridFurnVisualization.SetActive(false);
            isTile = true;
            previewSystem.StartShowingPlacementPreview(database.objectData[selectedObjectIndex].Prefab, database.objectData[selectedObjectIndex].Size, ID);
        }
        else
        {
            gridTileVisualization.SetActive(false);
            gridFurnVisualization.SetActive(true);
            isTile = false;
            previewSystem.StartShowingPlacementPreview(database.objectData[selectedObjectIndex].Prefab, database.objectData[selectedObjectIndex].Size, ID);
        }
        if (start == 0)
        {
            inputManager.OnClicked += PlaceStructure;
            inputManager.OnExit += StopPlacement;
            inputManager.OnRightClicked += RotateStructure;
            start++;
        }

    }

    private void PlaceStructure() //클릭으로 놓기
    {
        if (inputManager.IsPointerOverUI())
        {
            return;
        }
        Vector3 mousePos = inputManager.GetSelectedMapPosition();
        Vector3Int gridPos;
        if (isTile)
        {
            gridPos = gridTile.WorldToCell(mousePos);
        }
        else
        {
            gridPos = gridFurniture.WorldToCell(mousePos);
        }

        bool placementValidity = CheckPlacementValidity(gridPos, selectedObjectIndex, rot);
        if (placementValidity == false)
        {
            return;
        }

        GameObject newObject = Instantiate(database.objectData[selectedObjectIndex].Prefab);
        if (isTile)
        {
            newObject.transform.parent = tileParent;
        }
        else
        {
            newObject.transform.parent = furnParent;
        }
        newObject.transform.rotation = Quaternion.Euler(0, 90 * rot, 0);
        GridData selectedData = database.objectData[selectedObjectIndex].ID == 0 ? floorData : furnitureData;
        if (isTile)
        {
            newObject.transform.position = gridTile.CellToWorld(gridPos) + new Vector3(0, 0.001f, 0) + previewSystem.offset;
            placedTile.Add(newObject);
            selectedData.AddObjectAt(gridPos, database.objectData[selectedObjectIndex].Size, database.objectData[selectedObjectIndex].ID, placedTile.Count - 1, 0);
            Vector3Int tiles = new Vector3Int(gridPos.x * 2, 0, gridPos.z * 2);
            furnitureData.AddObjectAt(tiles, new Vector2Int(2, 2), 999, placedGameObject.Count - 1, 0);
            previewSystem.UpdatePosition(gridTile.CellToWorld(gridPos), false);
        }
        else
        {
            newObject.transform.position = gridFurniture.CellToWorld(gridPos) + new Vector3(0, 0.001f, 0) + previewSystem.offset;
            if (rot == 1)
            {
                newObject.transform.position += new Vector3(0, 0, 1);
            }
            else if (rot == 2)
            {
                newObject.transform.position += new Vector3(0, 0, 0);
            }
            else if (rot == 3)
            {
                newObject.transform.position += new Vector3(0, 0, -1);
            }
            placedGameObject.Add(newObject);
            selectedData.AddObjectAt(gridPos, database.objectData[selectedObjectIndex].Size, database.objectData[selectedObjectIndex].ID, placedGameObject.Count - 1, rot);
            previewSystem.UpdatePosition(gridFurniture.CellToWorld(gridPos), false);
        }
    }

    private bool CheckPlacementValidity(Vector3Int gridPos, int selectedObjectIndex, int rot)
    {
        GridData selectedData = database.objectData[selectedObjectIndex].ID == 0 ? floorData : furnitureData;
        if (selectedData == floorData) //고른게 tile이면
        {
            return selectedData.CanPlaceTileAt(gridPos);
        }
        else //고른게 가구라면
        {
            if (furnitureData.placedTiles.ContainsKey(gridPos))
            {
                return selectedData.CanPlaceObjectAt(gridPos, database.objectData[selectedObjectIndex].Size, rot);
            }
            else
            {
                return false;
            }
        }
    }

    public void StopPlacement() //설치 멈추기
    {
        start = 0;
        previewSystem.StopShowingPreview();
        lastDetectedPosition = Vector3Int.zero;
        selectedObjectIndex = -1;
        gridTileVisualization.SetActive(false);
        gridFurnVisualization.SetActive(false);
        inputManager.OnClicked -= PlaceStructure;
        inputManager.OnExit -= StopPlacement;
        inputManager.OnRightClicked -= RotateStructure;
        rot = 0;
    }

    public void RotateStructure()
    {
        rot++;
        rot = rot % 4; //0~3까지만
        previewSystem.RotatePreview(rot);
    }

    public void StartRemove()
    {
        StopPlacement();
        gridFurnVisualization.SetActive(true);
        isTile = false;
        isRemove = true;
        inputManager.OnClicked += RemoveFurn;
    }

    public void RemoveFurn()
    {
        Vector3 mousePos = inputManager.GetSelectedMapPosition();
        Vector3Int gridPos = gridFurniture.WorldToCell(mousePos);
        if (furnitureData.placedObjects.ContainsKey(gridPos))
        {
            int i = furnitureData.RemoveFurniture(gridPos);
            Destroy(furnParent.GetChild(i).gameObject);
        }
    }


}
