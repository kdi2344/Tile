using System;
using System.Collections.Generic;
using UnityEngine;

public class PlacementSystem : MonoBehaviour
{
    [SerializeField] private GameObject mouseIndicator, cellIndicator;
    [SerializeField] private InputManager inputManager;
    [SerializeField] private Grid grid;
    [SerializeField] private ObjectsDatabase database;
    private int selectedObjectIndex = -1;
    [SerializeField] private GameObject gridVisualization;
    private GridData floorData, furnitureData;
    private Renderer[] previewRenderer;
    public List<GameObject> placedGameObject;

    [SerializeField] private Transform tileParent;

    private void Start()
    {
        StopPlacement();
        floorData = new();
        furnitureData = new();
        previewRenderer = cellIndicator.GetComponentsInChildren<Renderer>();

        GridData startData = floorData;
        startData.AddObjectAt(new Vector3Int(4, 0, 4), Vector2Int.one, 0, 0, 0);
        startData.AddObjectAt(new Vector3Int(4, 0, 3), Vector2Int.one, 0, 1, 0);
        startData.AddObjectAt(new Vector3Int(3, 0, 4), Vector2Int.one, 0, 2, 0);
        startData.AddObjectAt(new Vector3Int(3, 0, 3), Vector2Int.one, 0, 3, 0);
    }
    public void StartPlacement(int ID)
    {
        selectedObjectIndex = database.objectData.FindIndex(data => data.ID == ID);
        if (selectedObjectIndex < 0)
        {
            Debug.Log("읎음");
            return;
        }
        gridVisualization.SetActive(true);
        cellIndicator.SetActive(true);
        inputManager.OnClicked += PlaceStructure;
        inputManager.OnExit += StopPlacement;
    }
    private void PlaceStructure()
    {
        if (inputManager.IsPointerOverUI())
        {
            return;
        }
        Vector3 mousePos = inputManager.GetSelectedMapPosition();
        Vector3Int gridPos = grid.WorldToCell(mousePos);

        bool placementValidity = CheckPlacementValidity(gridPos, selectedObjectIndex);
        if (placementValidity == false)
        {
            return;
        }

        GameObject newObject = Instantiate(database.objectData[selectedObjectIndex].Prefab);
        newObject.transform.parent = tileParent;
        newObject.transform.position = grid.CellToWorld(gridPos) + new Vector3(0, 0.001f, 0);
        placedGameObject.Add(newObject);
        GridData selectedData = database.objectData[selectedObjectIndex].ID == 0 ? floorData : furnitureData;
        selectedData.AddObjectAt(gridPos, database.objectData[selectedObjectIndex].Size, database.objectData[selectedObjectIndex].ID, placedGameObject.Count - 1, 0);
    }
    private bool CheckPlacementValidity(Vector3Int gridPos, int selectedObjectIndex)
    {
        GridData selectedData = database.objectData[selectedObjectIndex].ID == 0 ? floorData : furnitureData;
        if (selectedData == floorData) //고른게 tile이면
        {
            return selectedData.CanPlaceTileAt(gridPos);
        }
        else
        {
            if (floorData.placedObjects.ContainsKey(gridPos))
            {
                return selectedData.CanPlaceObjectAt(gridPos, database.objectData[selectedObjectIndex].Size);
            }
            else
            {
                return false;
            }
        }
    }
    public void StopPlacement()
    {
        selectedObjectIndex = -1;
        gridVisualization.SetActive(false);
        cellIndicator.SetActive(false);
        inputManager.OnClicked -= PlaceStructure;
        inputManager.OnExit -= StopPlacement;
    }

    private void Update()
    {
        if (selectedObjectIndex < 0)
        {
            return;
        }
        Vector3 mousePos = inputManager.GetSelectedMapPosition();
        Vector3Int gridPos = grid.WorldToCell(mousePos);

        bool placementValidity = CheckPlacementValidity(gridPos, selectedObjectIndex);
        foreach(Renderer render in previewRenderer)
        {
            render.material.color = placementValidity ? Color.white : Color.red;
        }

        mouseIndicator.transform.position = mousePos;
        cellIndicator.transform.position = grid.CellToWorld(gridPos);
    }
}
