using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUI : MonoBehaviour
{
    public StatBar healthBar;
    public StatBar staminaBar;
    [Space]
    public Menu[] menus;

    // a dictionary that allows us to refer to a menu by it's tag.
    // Eg. we can load the main menu from the tag 'mainMenu' rather than the number '0'.
    // this makes it easier to work with large amounts of menus.
    private Dictionary<string, Menu> menuDict = new Dictionary<string, Menu>();

    // the current menu's ID.
    private string currentMenu;
    // the previous menu's ID.
    private string lastMenu;

    private void Awake()
    {
        // before we do anything we need to populate our dictionary.
        for(int i = 0; i< menus.Length; i++)
        {
            // for every menu we add it to the dictionary with it's ID as the key.
            menuDict.Add(menus[i].ID, menus[i]);
        }
    }
    private void Start()
    {
        // set the current menu to the main menu.
        SetCurrentMenu("mainMenu");
    }
    private void Update()
    {
        healthBar.value = Game.character.health;
        healthBar.maxValue = Game.character.maxHealth;

        staminaBar.value = Game.character.stamina;
        staminaBar.maxValue = Game.character.maxStamina;
    }

    public void EndTurn()
    {
        menuDict[currentMenu].UpdateMenuVisuals();
    }

    public void SetCurrentMenu(string ID)
    {
        // set the last menu to our current menu
        lastMenu = currentMenu;
        // set the current menu to the new ID
        currentMenu = ID;

        foreach(KeyValuePair<string, Menu> menu in menuDict)
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

        // set the new menu's element to 0.
        menuDict[ID].SetElement(0);
        // update the current menu's visuals
        menuDict[ID].UpdateMenuVisuals();
    }
    public void ReturnToPreviousMenu()
    {
        // set the current menu to the last menu.
        SetCurrentMenu(lastMenu);
    }
}
