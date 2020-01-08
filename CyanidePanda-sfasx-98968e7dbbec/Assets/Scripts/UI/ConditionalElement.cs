using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class ConditionalElement : MenuElement
{
    public enum HiddenType { Hidden, Stricken }
    public HiddenType hiddenType;
    public GameObject strickenObject;

    public abstract void UpdateVisuals();
}
