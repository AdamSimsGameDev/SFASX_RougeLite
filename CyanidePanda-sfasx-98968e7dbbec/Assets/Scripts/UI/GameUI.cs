using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameUI : MenuManager
{
    [Header("GameUI")]
    public StatText healthText;
    public StatText manaText;
    [Space]
    public TextMeshProUGUI abilityDescription;

    [Header("Drops")]
    public Transform dropParent;
    public GameObject dropPrefab;

    [Header("Abilities")]
    public Transform abilityMenuParent;
    public GameObject abilityPrefab;

    private void Awake()
    {
        // before we do anything we need to populate our dictionary.
        for(int i = 0; i< menus.Length; i++)
        {
            // for every menu we add it to the dictionary with it's ID as the key.
            menuDict.Add(menus[i].ID, menus[i]);
        }
    }
    private void Update()
    {
        healthText.value = Game.character.health;
        healthText.maxValue = Game.character.maxHealth;

        manaText.value = Game.character.mana;
        manaText.maxValue = Game.character.maxMana;

        currencyText.value = Global.instance.currency;

        string ab = "";
        if (Game.character.currentAbility != "" && !Game.character.IsProcessingAbility)
        {
            ab = Game.character.currentAbility;
        }
        else
        {
            ab = Game.character.hoveredAbility;
        }

        abilityDescription.gameObject.SetActive(ab != "");

        if (ab != "")
        {
            Ability ability = Ability.abilities[ab];

            if (ability.isCooldown)
            {
                abilityDescription.text = (ability.name + "\n" + "on cooldown " + ability.currentCooldown + (ability.currentCooldown > 1 ? " turns" : " turn") + " remaining!").ToUpper();
            }
            else
            {
                int manaCost = ability.manaCost;
                if (ability.useWeapon)
                {
                    manaCost = ability.manaCost;
                }

                if (manaCost == 0)
                {
                    abilityDescription.text = (ability.name + "\n" + ability.description).ToUpper();
                }
                else
                {
                    abilityDescription.text = (ability.name + " - " + manaCost + " mana" + "\n" + ability.description).ToUpper();
                }
            }
        }
    }

    public void StartGame()
    {
        // populate ability menu
        for (int i = 0; i < 5; i++)
        {
            string tag = Global.instance.abilities[i];

            Ability ability = null;
            if (Ability.abilities.TryGetValue(tag, out ability))
            {
                GameObject go = Instantiate(abilityPrefab);
                go.transform.SetParent(abilityMenuParent, false);
                go.transform.SetSiblingIndex(abilityMenuParent.childCount - 2);

                AbilityElement element = go.GetComponent<AbilityElement>();
                element.onUse.AddListener(() => Game.instance.SetPlayerAbility(tag));
                element.onUse.AddListener(() => Game.ui.SetCurrentMenu("backMenu"));
                element.onHover.AddListener(() => Game.instance.SetPlayerHoveredAbility(tag));

                element.transform.Find("TextParent").Find("Text").GetComponent<TextMeshProUGUI>().text = ability.name.ToUpper();

                element.abilityTag = tag;
            }
        }
    }

    public void EndTurn()
    {
        SetCurrentMenu("actionsMenu");
        menuDict[currentMenu].SetElement(0);
        menuDict[currentMenu].UpdateMenuVisuals();
    }

    public void AddDropUI(string key, int amount)
    {
        Item item = Inventory.instance.allItems[key];

        GameObject go = Instantiate(dropPrefab);
        go.transform.SetParent(dropParent, false);

        go.GetComponent<TextMeshProUGUI>().text = "+ " + item.name.ToUpper() + " x " + amount;
    }
}
