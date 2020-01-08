using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MenuElement : MonoBehaviour
{
    // the cursor for the menu, in this case it is the '>' character in a text box.
    public GameObject selectionCursor;
    // whether the selection element is active or not.
    public bool isActive;
    // the events that are ran when this element is used
    public UnityEvent onUse;

    private void Start()
    {
        OnStart();
    }
    private void Update()
    {
        OnUpdate();
    }

    protected virtual void OnStart ()
    {
        // if the selection cursor is null, attempt to find it.
        if (selectionCursor == null)
        {
            selectionCursor = transform.Find("Cursor").gameObject;
        }
    }
    protected virtual void OnUpdate ()
    {
        // set the cursor to be active / inactive if it exists
        if (selectionCursor != null)
        {
            selectionCursor.SetActive(isActive);
        }
    }

    public void SetActive(bool active)
    {
        // set the activity of the element
        isActive = active;
    }
}
