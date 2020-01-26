using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class ShopElement : ConditionalElement
{
    public string inventoryTag;
    public TextMeshProUGUI text;
    public TextMeshProUGUI amount;
    public bool forceHide;

    public override void UpdateVisuals()
    {
        text.text = Inventory.instance.allItems[inventoryTag].name.ToUpper();
        amount.text = Inventory.instance.allItems[inventoryTag].amount.ToString();
        gameObject.SetActive(!forceHide);
    }
}
