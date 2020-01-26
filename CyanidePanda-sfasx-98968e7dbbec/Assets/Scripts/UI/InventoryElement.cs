using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class InventoryElement : ConditionalElement
{
    public string inventoryTag;
    public TextMeshProUGUI text;
    public TextMeshProUGUI amount;
    public bool forceHide;

    public override void UpdateVisuals()
    {
        if (forceHide)
        {
            return;
        }

        Item item = null;
        if (Inventory.instance.allItems.TryGetValue(inventoryTag, out item))
        {
            bool isEquipped = false;
            if (item is Armour)
            {
                // check to see if the armour is the armour that is currently equipped.
                Armour armour = (Armour)item;
                isEquipped = armour.isEquipped;
            }
            else if (item is Weapon)
            {
                Weapon weapon = (Weapon)item;
                isEquipped = weapon.isEquipped;
            }
            else if (item is SpellBook)
            {
                SpellBook book = (SpellBook)item;
                isEquipped = book.isEquipped;
            }

            text.text = (isEquipped ? "(E)" : "") + item.name.ToUpper();
            amount.text = item.amount.ToString();

            if (item.amount != 0)
            {
                // the player has the item.
                // this means that we can check to see if we need to hide it or not.
                // we can use the ability.
                gameObject.SetActive(true);
                strickenObject.SetActive(false);

                // set interactability to true
                isInteractable = true;
            }
            else
            {
                // the player does not have the ability
                // this means that regardless of the 'hiddenType', it will be set to hidden.
                gameObject.SetActive(false);
            }
        }
        else
        {
            Debug.LogError("InventoryElement " + transform.name + " has an invalid item tag of '" + inventoryTag + "'!");
            gameObject.SetActive(false);
        }
    }
}
