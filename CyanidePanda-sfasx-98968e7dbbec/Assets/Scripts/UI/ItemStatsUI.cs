using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ItemStatsUI : MonoBehaviour
{
    public TextMeshProUGUI damageText;
    public TextMeshProUGUI defenseText;
    public TextMeshProUGUI typeText;
    public TextMeshProUGUI rangeText;
    public TextMeshProUGUI valueText;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI manaText;
    [Space]
    public int damage;
    public int defense;
    public DamageType element;
    public int range;
    public int value;
    public int healthRegen;
    public int manaRegen;

    public void SetWeaponValues(int damage, DamageType type, int range, int value)
    {
        this.damage = damage;
        this.defense = 0;
        this.element = type;
        this.range = range;
        this.value = value;
        this.healthRegen = 0;
        this.manaRegen = 0;

        UpdateVisuals();
    }
    public void SetArmourValues(int defense, DamageType type, int value)
    {
        this.damage = 0;
        this.defense = defense;
        this.element = type;
        this.range = 0;
        this.value = value;
        this.healthRegen = 0;
        this.manaRegen = 0;

        UpdateVisuals();
    }
    public void SetConsumableValues(int healthRegen, int manaRegen, int value)
    {
        this.damage = 0;
        this.defense = 0;
        this.range = 0;
        this.value = value;
        this.healthRegen = healthRegen;
        this.manaRegen = manaRegen;

        UpdateVisuals();
    }
    public void SetMisc(int value)
    {
        this.damage = 0;
        this.defense = 0;
        this.range = 0;
        this.value = value;
        this.healthRegen = 0;
        this.manaRegen = 0;

        UpdateVisuals();
    }
    public void SetEmpty()
    {
        this.damage = 0;
        this.defense = 0;
        this.range = 0;
        this.value = 0;
        this.healthRegen = 0;
        this.manaRegen = 0;

        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        // first we work out whether to set things to active or not
        damageText.gameObject.SetActive(damage != 0);
        defenseText.gameObject.SetActive(defense != 0);
        typeText.gameObject.SetActive(damage != 0 || defense != 0);
        rangeText.gameObject.SetActive(range != 0);
        valueText.gameObject.SetActive(value != 0);
        healthText.gameObject.SetActive(healthRegen != 0);
        manaText.gameObject.SetActive(manaRegen != 0);

        // we get the type string
        string typeString = "";
        switch (element)
        {
            case DamageType.Normal:
                typeString = "NORMAL";
                break;
            case DamageType.Fire:
                typeString = "FIRE";
                break;
            case DamageType.Water:
                typeString = "WATER";
                break;
            case DamageType.Grass:
                typeString = "GRASS";
                break;
            case DamageType.Light:
                typeString = "LIGHT";
                break;
            case DamageType.Dark:
                typeString = "DARK";
                break;
        }

        // we then set the text
        damageText.text = "DAMAGE: " + damage.ToString("n0");
        defenseText.text = "DEFENSE: " + defense.ToString("n0");
        typeText.text = "TYPE: " + typeString;
        rangeText.text = "RANGE: " + range.ToString("n0");
        valueText.text = "VALUE: " + value.ToString("n0");
        healthText.text = "HEALTH: +" + healthRegen.ToString("n0");
        manaText.text = "MANA: +" + manaRegen.ToString("n0");
    }
}
