using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelElement : MenuElement
{
    public int index;

    protected override void OnUpdate()
    {
        base.OnUpdate();

        if (Input.GetButtonDown("Look"))
        {
            Global.instance.DeleteSave(index);
        }
    }
}
