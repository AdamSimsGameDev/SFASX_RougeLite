using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AbilityElement : ConditionalElement
{
    public string abilityTag;

    public override void UpdateVisuals()
    {
        Ability a = null;
        if (Ability.abilities.TryGetValue(abilityTag, out a))
        {
            if (Game.character.abilities.TryGetValue(abilityTag, out a))
            {
                // the player has the ability.
                // this means that we can check to see if we need to hide it or not.
                if (a.isCooldown || Game.character.stamina < a.staminaCost)
                {
                    // we can't use the ability.
                    switch(hiddenType)
                    {
                        case HiddenType.Hidden:
                            gameObject.SetActive(false);
                            break;
                        case HiddenType.Stricken:
                            strickenObject.SetActive(true);
                            break;
                    }
                    // set interactability to false
                    isInteractable = false;
                }
                else
                {
                    // we can use the ability.
                    gameObject.SetActive(true);
                    strickenObject.SetActive(false);
                    // set interactability to true
                    isInteractable = true;
                }
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
            Debug.LogError("AbilityElement " + transform.name + " has an invalid ability tag of '" + abilityTag + "'!");
            gameObject.SetActive(false);
        }
    }
}
