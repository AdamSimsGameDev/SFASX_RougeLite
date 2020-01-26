using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InventoryData
{
    public string tag;
    public int amount;

    public InventoryData(string t, int a)
    {
        tag = t;
        amount = a;
    }
}
