using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DropTable
{
    public MinMax coins;
    [Space]
    public DropItem[] definiteItems;
    public DropItem[] chanceItems;

    public DropItem[] GetChanceItem()
    {
        List<DropItem> items = new List<DropItem>();

        for (int i = 0; i < chanceItems.Length; i++)
        {
            float r = Random.Range(0.0F, 100.0F);
            if (r < chanceItems[i].value)
            {
                items.Add(chanceItems[i]);
            }
        }

        return items.ToArray();
    }
}

[System.Serializable]
public class DropItem
{
    public string key;
    public MinMax amount;
    [Space]
    [Range(0.0F, 100.0F)] public float value;
}
