using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShopBuyUI : MonoBehaviour
{
    public static ShopBuyUI instance;

    public Transform[] inventories;
    public List<ShopElement> elements = new List<ShopElement>();
    [Space]
    public TextMeshProUGUI title;
    public TextMeshProUGUI description;
    public ItemStatsUI itemStats;
    [Space]
    public int currentInventory = -1;
    public int currentPage;

    public int[] pages = new int[4];
    private Item currentItem;

    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        Setup();
    }
    private void Update()
    {
        if (currentInventory != -1)
        {
            if (currentPage >= pages[currentInventory])
            {
                SetCurrentPage(pages[currentInventory] - 1);
            }

            if (Input.GetButtonDown("Previous") && currentPage > 0)
                SetCurrentPage(currentPage - 1);
            if (Input.GetButtonDown("Next") && currentPage < pages[currentInventory] - 1)
                SetCurrentPage(currentPage + 1);

            if (currentItem != null)
            {
                title.text = currentItem.name.ToUpper();
                description.text = currentItem.description.ToUpper();

                title.gameObject.SetActive(true);
                description.gameObject.SetActive(true);
            }
            else
            {
                title.gameObject.SetActive(false);
                description.gameObject.SetActive(false);
            }
        }
        else
        {
            title.gameObject.SetActive(false);
            description.gameObject.SetActive(false);
        }

    }

    public void SetCurrentInventory(int inventory)
    {
        currentInventory = inventory;

        for (int i = 0; i < 5; i++)
        {
            inventories[i].gameObject.SetActive(i == (currentInventory + 1));
        }

        UpdateInventory();
        SetCurrentPage(0);
    }

    /// <summary>
    /// Sets the current page of the inventory.
    /// </summary>
    /// <param name="value"></param>
    public void SetCurrentPage(int value)
    {
        currentPage = value;

        if (currentInventory != -1)
        {
            // finds all of the valid elements
            ShopElement[] elements = inventories[currentInventory + 1].GetComponentsInChildren<ShopElement>(true);
            List<ShopElement> validElements = new List<ShopElement>();
            // this is done by looping through all inactive and active elements and finding those who are either forced to be hidden or are active. 
            foreach (ShopElement e in elements)
            {
                if (e.forceHide || e.isInteractable)
                    validElements.Add(e);
            }
            
            // we can then get the min and max thresholds from the current page
            int min = (currentPage + 0) * 12;
            int max = (currentPage + 1) * 12;

            // and then we can set the elements to active or inactive based on whether they meet the requirements
            for (int i = 0; i < validElements.Count; i++)
            {
                bool isActive = (i >= min && i < max);
                validElements[i].forceHide = !isActive;
                validElements[i].gameObject.SetActive(isActive);
            }

            // we then update the menu
            Menu menu = inventories[currentInventory + 1].GetComponent<Menu>();
            menu.SetElement(0);
            menu.UpdateMenuVisuals();
        }
    }

    public void Setup()
    {
        // setup weapons
        foreach (string s in Shop.instance.itemsForSale)
        {
            Item i = Inventory.instance.allItems[s];
            if (i is Weapon)
            {
                CreateMenuItem(s, inventories[1]);
            }
            else if (i is Armour)
            {
                CreateMenuItem(s, inventories[2]);
            }
            else if (i is Consumable)
            {
                CreateMenuItem(s, inventories[3]);
            }
            else if (i is SpellBook)
            {
                CreateMenuItem(s, inventories[5]);
            }
            else
            {
                CreateMenuItem(s, inventories[4]);
            }
        }
    }
    public void UpdateInventory()
    {
        for (int i = 0; i < inventories.Length; i++)
        {
            inventories[i].GetComponent<Menu>().UpdateMenuVisuals();
        }

        foreach (ShopElement element in elements)
        {
            element.UpdateVisuals();
        }

        // get all active elements in each inventory
        for (int i = 0; i < 5; i++)
        {
            ShopElement[] elements = inventories[i + 1].GetComponentsInChildren<ShopElement>(true);
            List<ShopElement> validElements = new List<ShopElement>();
            foreach (ShopElement e in elements)
            {
                if (e.forceHide || e.isInteractable)
                    validElements.Add(e);
            }

            pages[i] = Mathf.CeilToInt(validElements.Count / 12.0F);
        }
    }

    public void CreateMenuItem (string tag, Transform parent)
    {
        GameObject prefab = (GameObject)Resources.Load("UI/ShopElement");
        GameObject go = Instantiate(prefab);

        go.transform.SetParent(parent, false);
        go.transform.SetSiblingIndex(parent.childCount - 2);

        ShopElement element = go.GetComponent<ShopElement>();
        element.inventoryTag = tag;
        element.UpdateVisuals();

        element.onUse.AddListener(() => Shop.instance.BuyItem(tag));
        element.onHover.AddListener(() => SetItem(tag));

        elements.Add(element);
    }

    public void SetItem(string tag)
    {
        if (tag == "null")
        {
            currentItem = null;
        }
        else
        {
            currentItem = Inventory.instance.allItems[tag];
        }

        // set the item UI
        if (currentItem != null)
        {
            itemStats.isShop = true;

            // find the item type
            if (currentItem is Weapon)
            {
                // cast the weapon
                Weapon weapon = (Weapon)currentItem;
                // display the weapon stats
                itemStats.SetWeaponValues(weapon.damage, weapon.element, weapon.range, weapon.value);
            }
            else if (currentItem is Armour)
            {
                // cast the weapon
                Armour armour = (Armour)currentItem;
                // display the weapon stats
                itemStats.SetArmourValues(armour.defense, armour.element, armour.value);
            }
            else if (currentItem is Consumable)
            {
                // cast the weapon
                Consumable consumable = (Consumable)currentItem;
                // display the weapon stats
                itemStats.SetConsumableValues(consumable.healthRegen, consumable.manaRegen, consumable.value);
            }
            else
            {
                // set misc
                itemStats.SetMisc(currentItem.value);
            }
        }
        else
        {
            // set the values to the null values
            itemStats.SetEmpty();
        }
    }
}
