using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WorldMapUI : MenuManager
{
    private void Start()
    {
        SetCurrentMenu("actionsMenu");
    }
    private void Update()
    {
        currencyText.value = Global.instance.currency;
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
