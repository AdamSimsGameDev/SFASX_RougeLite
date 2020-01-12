using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Menu : MonoBehaviour
{
    // the ID that this menu is referred to by
    public string ID;

    // an array of the current selectionElements.
    public MenuElement[] selectionElements;
    // the currently selected element
    public int currentElement = 0;

    // whether the menu is active
    public bool isActive = false;

    // whether input is allowed when an ability is active
    public bool allowInputDuringAbility = false;

    // whether the element has been switched without having released the analog stick, to avoid the change being too quick.
    public bool hasSwitched = false;

    // whether the menu will be disabled graphically when it isn't in use
    public bool disableWhenInactive = false;

    // the events that are ran when this menu is used with the back button.
    public UnityEvent onBack;

    private void Start()
    {
        // we update the elements immediately to let the menu update once.
        UpdateMenuVisuals();
        UpdateElements();
    }
    private void Update()
    {
        // if the menu isn't active (in the focus of the player), return
        if (isActive == false)
            return;

        // handle button input
        HandleButtonInput();

        /*
        // check to see if the player has an ability selected, if so we don't want them to have any further control of the menu.
        // the reason we do this after the 'jump' input is to allow for the use of back buttons
        if (Game.character.currentAbility != -1)
            return;
        */

        // get the vertical axis, either W and S or the right analog stick's up and down movement.
        float y = Input.GetAxisRaw("Vertical");

        // if this value is equal to 0, it means that we aren't trying to move it.
        if (y == 0.0F)
        {
            // this means we can reset the bool for the switch.
            hasSwitched = false;
        }
        // we then test to see if hasSwitched is false\
        if (hasSwitched == false)
        {
            // we test the input that we got.
            if (y > 0.0F)
            {
                // if the value is higher than 0 we need to move the selection upwards.
                // we can do this by lowering the currentElement
                // we also want to wrap it too, so if the value falls lower than 0, set it to the maximum - 1
                currentElement--;
                if (currentElement < 0)
                    currentElement = selectionElements.Length - 1;

                // we then update the elements to adjust to the new selection.
                UpdateElements();
            }
            else if (y < 0.0F)
            {
                // if the value is lower than 0 we need to move the selection down.
                // we can do this by raising the currentElement
                // we also want to wrap it too, so if the value is higher than or equal to the maximum, set it to 0
                currentElement++;
                if (currentElement >= selectionElements.Length)
                    currentElement = 0;

                // we then update the elements to adjust to the new selection.
                UpdateElements();
            }
        }
    }

    private void HandleButtonInput ()
    {
        // create a bool for whether this button can be used.
        bool canInteract = true;

        // test to see if the button is conditional.
        if (selectionElements[currentElement] is ConditionalElement)
        {
            // if it is we need to see if it is unusable.
            ConditionalElement conditional = selectionElements[currentElement] as ConditionalElement;
            canInteract = conditional.isInteractable;
        }

        // only allow the button to be used if it is active.
        if (canInteract)
        {
            // if the player presses the 'jump' button, activate the event on the current element
            if (Input.GetButtonDown("Use"))
                selectionElements[currentElement].onUse.Invoke();
            // if the player pushes the 'back' button, activate the event on the current element and this menu
            if (Input.GetButtonDown("Back"))
            {
                onBack.Invoke();
                selectionElements[currentElement].onBack.Invoke();
            }
        }
    }

    public void SetElement (int element)
    {
        currentElement = element;
        UpdateElements();
    }
    private void UpdateElements()
    {
        // here we can update the elements
        for (int i = 0; i < selectionElements.Length; i++)
        {
            MenuElement element = (MenuElement)selectionElements[i];
            // we set the element to active if it's position in the array is the same as the current element
            element.SetActive(currentElement == i);
        }

        // and set the switched state to true.
        hasSwitched = true;
    }

    public void UpdateMenuVisuals ()
    {
        // at the end of each turn we want to reset the current element
        currentElement = 0;

        // we then set all elements to inactive
        foreach (MenuElement element in selectionElements)
            element.SetActive(false);

        // we want to create a list of the new elements.
        List<MenuElement> elements = new List<MenuElement>();
        // we also want to get all of the elements that are childed to this object
        MenuElement[] allElements = GetComponentsInChildren<MenuElement>(true);

        // we can then look at all of the conditional elements that can be enabled / disabled
        for (int i = 0; i < allElements.Length; i++)
        {
            if (allElements[i] is ConditionalElement)
            {
                ConditionalElement e = (ConditionalElement)allElements[i];
                e.UpdateVisuals();
            }
        }

        // after this we loop through all elements AGAIN, this time adding them to the list of elements.
        // we only do this if they are active
        for (int i = 0; i < allElements.Length; i++)
        {
            if (allElements[i].gameObject.activeInHierarchy)
            {
                elements.Add(allElements[i]);
            }
        }

        // and then update the elements appropriately.
        for (int i = 0; i < elements.Count; i++)
        {
            MenuElement element = (MenuElement)elements[i];
            // we set the element to active if it's position in the array is the same as the current element
            element.SetActive(currentElement == i);
        }

        // we then update the selection elements array
        selectionElements = elements.ToArray();
    }
}
