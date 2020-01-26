using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour
{
    public static Shop instance;

    public List<string> itemsForSale = new List<string>();

    private void Awake()
    {
        instance = this;    
    }

    public bool BuyItem(string key)
    {
        if (Global.instance.currency >= Inventory.instance.allItems[key].value)
        {
            Inventory.instance.AddItem(key, 1);
            Global.instance.currency -= Inventory.instance.allItems[key].value;
            ShopBuyUI.instance.UpdateInventory();
        }
        return true;
    }

    public void SellItem (string key)
    {
        // get the item
        Item item = Inventory.instance.allItems[key];
        // if there is only 1, don't sell it if it is equipped
        if (item.amount == 1)
        {
            if (item is Armour)
            {
                Armour a = (Armour)item;
                if (a.isEquipped) return;
            }
            else if (item is Weapon)
            {
                Weapon a = (Weapon)item;
                if (a.isEquipped) return;
            }
            else if (item is SpellBook)
            {
                SpellBook a = (SpellBook)item;
                if (a.isEquipped) return;
            }
        }

        // add currency
        Global.instance.currency += Mathf.CeilToInt(Inventory.instance.allItems[key].value * 0.6F);
        // remove item
        Inventory.instance.allItems[key].amount--;
        // update inventory
        InventoryUI.instance.UpdateInventory();
        // update shop
        ShopSellUI.instance.UpdateInventory();
    }
}