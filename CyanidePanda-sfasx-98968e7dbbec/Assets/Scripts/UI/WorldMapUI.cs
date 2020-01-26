using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WorldMapUI : MenuManager
{
    [Header("BuyScreen")]
    public GameObject buyScreen;
    public List<string> buyMenus = new List<string>();
    [Header("SellScreen")]
    public GameObject sellScreen;
    public List<string> sellMenus = new List<string>();
    [Space]
    public GameObject navigationText;

    private void Start()
    {
        SetCurrentMenu("actionsMenu");
    }
    private void Update()
    {
        navigationText.SetActive(FindObjectOfType<WorldMap>().canMove);

        currencyText.value = Global.instance.currency;

        if (buyScreen != null)
        {
            buyScreen.SetActive(buyMenus.Contains(currentMenu));
        }
        if (sellScreen != null)
        {
            sellScreen.SetActive(sellMenus.Contains(currentMenu));
        }
    }

    public void Save()
    {
        Global.instance.SaveGame();
    }

    public void ReturnToMenu()
    {
        Global.instance.ReturnToMenu();
    }
}
