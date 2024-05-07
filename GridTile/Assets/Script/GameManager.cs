using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;

    [SerializeField] private Transform topParent;

    public enum Price
    {
        brick,
        concrete,
        wood,
        money
    }
    public Dictionary<Price, int> playerProperty;
    
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
        playerProperty = new Dictionary<Price, int>();
        playerProperty.Add(Price.brick, 5);
        playerProperty.Add(Price.concrete, 5);
        playerProperty.Add(Price.wood, 5);
        playerProperty.Add(Price.money, 5);
        SetTopText();
    }

    public void SetTopText()
    {
        topParent.GetChild(0).GetComponent<TextMeshProUGUI>().text = playerProperty[Price.brick].ToString();
        topParent.GetChild(1).GetComponent<TextMeshProUGUI>().text = playerProperty[Price.concrete].ToString();
        topParent.GetChild(2).GetComponent<TextMeshProUGUI>().text = playerProperty[Price.wood].ToString();
        topParent.GetChild(3).GetComponent<TextMeshProUGUI>().text = playerProperty[Price.money].ToString();
    }
}
