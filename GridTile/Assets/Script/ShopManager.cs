using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopManager : MonoBehaviour
{
    public static ShopManager instance = null;

    [SerializeField] private Sprite[] priceIcons;
    [SerializeField] private ObjectsDatabase database;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        SetBtns();
    }
    public void SetBtns()
    {
        Transform parent = transform.GetChild(0).GetChild(0).GetChild(0);
        for (int i = 0; i < parent.childCount; i++)
        {
            parent.GetChild(i).GetComponent<Image>().sprite = database.objectData[i].Thumbnail;
            parent.GetChild(i).GetChild(0).GetComponent<TextMeshProUGUI>().text = database.objectData[i].Name.ToString();
            parent.GetChild(i).GetChild(1).GetComponent<TextMeshProUGUI>().text = database.objectData[i].priceNum.ToString();
            parent.GetChild(i).GetChild(2).GetComponent<Image>().sprite = PriceTypeToSprite(database.objectData[i].priceType);
            parent.GetChild(i).GetChild(3).GetComponent<TextMeshProUGUI>().text = database.objectData[i].Size.x.ToString() + " x " + database.objectData[i].Size.y.ToString();
        }
    }

    private Sprite PriceTypeToSprite(GameManager.Price price)
    {
        if (price == GameManager.Price.brick)
        {
            return priceIcons[0];
        }
        else if (price == GameManager.Price.concrete)
        {
            return priceIcons[1];
        }
        else if (price == GameManager.Price.money)
        {
            return priceIcons[2];
        }
        else
        {
            return priceIcons[3];
        }
    }
}
