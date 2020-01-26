using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public StatText currencyText;
    public Menu[] menus;
    public GameObject inventory;
    public List<string> inventoryMenus = new List<string>();

    // a dictionary that allows us to refer to a menu by it's tag.
    // Eg. we can load the main menu from the tag 'mainMenu' rather than the number '0'.
    // this makes it easier to work with large amounts of menus.
    protected Dictionary<string, Menu> menuDict = new Dictionary<string, Menu>();

    // the current menu's ID.
    protected string currentMenu = "";
    // the previous menu's ID.
    protected string lastMenu;

    private void Awake()
    {
        // before we do anything we need to populate our dictionary.
        for (int i = 0; i < menus.Length; i++)
        {
            // for every menu we add it to the dictionary with it's ID as the key.
            menuDict.Add(menus[i].ID, menus[i]);
        }
    }

    public void SetCurrentMenu(string ID)
    {
        Menu a = null;
        if (menuDict.TryGetValue(currentMenu, out a))
        {
            // set the current menu's last element
            a.lastElement = a.currentElement;
        }
        // set the last menu to our current menu
        lastMenu = currentMenu;
        // set the current menu to the new ID
        currentMenu = ID;

        foreach (KeyValuePair<string, Menu> menu in menuDict)
        {
            // the current menu's ID
            string ourID = menu.Key;
            // get the current menu
            Menu m = menu.Value;

            m.isActive = (ID == ourID);
            // graphically disable / enable the menu if need be
            if (m.disableWhenInactive)
            {
                m.gameObject.SetActive(ID == ourID);
            }
        }

        if (inventory != null)
        {
            inventory.SetActive(inventoryMenus.Contains(currentMenu));
        }

        // check to see if the new menu exists
        if (ID != "")
        {
            // set the new menu's element to the last element.
            menuDict[ID].SetElement(menuDict[ID].resetElement ? 0 : menuDict[ID].lastElement);
            // update the current menu's visuals
            menuDict[ID].UpdateMenuVisuals();
        }
    }
    public void ReturnToPreviousMenu()
    {
        // set the current menu to the last menu.
        SetCurrentMenu(lastMenu);
    }
}
