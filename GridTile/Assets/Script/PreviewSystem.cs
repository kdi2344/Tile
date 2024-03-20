using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewSystem : MonoBehaviour
{
    [SerializeField] private float previewYOffset = 0.06f;

    [SerializeField] private GameObject cellIndicatorTile;
    [SerializeField] private GameObject cellIndicatorFurn;
    private GameObject previewObject;

    [SerializeField] private Material previewMaterialPrefab;
    private Material previewMaterialnstance;

    private Renderer[] cellIndicatorRendererTile;
    private Renderer[] cellIndicatorRendererFurn;

    private bool isTile = false;

    public Vector3 offset;
    private int rot = 0;

    private void Start()
    {
        previewMaterialnstance = new Material(previewMaterialPrefab);
        cellIndicatorTile.SetActive(false);
        cellIndicatorFurn.SetActive(false);
        cellIndicatorRendererTile = cellIndicatorTile.GetComponentsInChildren<Renderer>();
        cellIndicatorRendererFurn = cellIndicatorFurn.GetComponentsInChildren<Renderer>();
    }

    public void StartShowingPlacementPreview(GameObject prefab, Vector2Int size, int ID)
    {
        rot = 0;
        previewObject = Instantiate(prefab);
        PreparePreview(previewObject);
        PrepareCursor(size);
        if (ID == 0)
        {
            cellIndicatorTile.SetActive(true);
            cellIndicatorFurn.SetActive(false);
            isTile = true;
        }
        else
        {
            cellIndicatorTile.SetActive(false);
            cellIndicatorFurn.SetActive(true);
            isTile = false;
        }
    }

    private void PrepareCursor(Vector2Int size)
    {
        if (size.x > 0 || size.y > 0)
        {
            GameObject cellIndicator;
            if (isTile)
            {
                cellIndicator = cellIndicatorTile;
                foreach (Renderer a in cellIndicatorRendererTile)
                {
                    a.material.mainTextureScale = size;
                }
            }
            else
            {
                cellIndicator = cellIndicatorFurn;
                foreach (Renderer a in cellIndicatorRendererFurn)
                {
                    a.material.mainTextureScale = size;
                }
            }
            cellIndicator.transform.localScale = new Vector3(size.x, 1, size.y);
        }
    }

    private void PreparePreview(GameObject preview)
    {
        foreach(Renderer renderer in previewObject.GetComponentsInChildren<Renderer>())
        {
            Material[] materials = renderer.materials;
            for (int i = 0; i < materials.Length; i++)
            {
                materials[i] = previewMaterialnstance;
            }
            renderer.materials = materials;
        }
    }

    public void StopShowingPreview()
    {
        Destroy(previewObject);
        cellIndicatorTile.SetActive(false);
        cellIndicatorFurn.SetActive(false);
    }

    public void UpdatePosition(Vector3 pos, bool validity)
    {
        MovePreview(pos);
        MoveCursor(pos);
        ApplyFeedback(validity);
    }

    private void ApplyFeedback(bool validity)
    {
        if (isTile)
        {
            Color c = validity ? Color.white : Color.red;
            foreach (Renderer r in cellIndicatorRendererTile)
            {
                r.material.color = c;
            }
            c.a = 0.5f;
            previewMaterialnstance.color = c;
        }
        else
        {
            Color c = validity ? Color.white : Color.red;
            foreach (Renderer r in cellIndicatorRendererFurn)
            {
                r.material.color = c;
            }
            c.a = 0.5f;
            previewMaterialnstance.color = c;
        }
    }

    private void MoveCursor(Vector3 pos)
    {
        Vector3 offset = Vector3.zero;
        if (rot == 1)
        {
            offset = new Vector3(0, 0, 0);
        }
        else if (rot == 2)
        {
            offset = new Vector3(-1, 0, 0);
        }
        else if (rot == 3)
        {
            offset = new Vector3(0, 0, -1);
        }
        if (isTile)
        {
            cellIndicatorTile.transform.position = pos;
        }
        else
        {
            cellIndicatorFurn.transform.position = pos + offset;
        }
    }

    private void MovePreview(Vector3 pos)
    {
        previewObject.transform.position = new Vector3(pos.x, pos.y + previewYOffset, pos.z) + offset;
    }

    public void RotatePreview(int rot)
    {
        Vector3 store = cellIndicatorFurn.transform.localScale;
        cellIndicatorFurn.transform.localScale = new Vector3(store.z, 1, store.x);
        previewObject.transform.rotation = Quaternion.Euler(0, 90 * rot, 0);
        this.rot = rot;
    }
}
