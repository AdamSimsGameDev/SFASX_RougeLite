using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatBar : MonoBehaviour
{
    public Color active;
    public Color inactive;

    private Sprite[] sprites;
    private List<GameObject> pieces = new List<GameObject>();

    public int value
    {
        get
        {
            return Value;
        }
        set
        {
            Value = value;
            UpdateBar();
        }
    }
    public int maxValue
    {
        get
        {
            return MaxValue;
        }
        set
        {
            MaxValue = value;
            UpdateBar();
        }
    }

    private int Value = 3;
    private int MaxValue = 3;

    private void Start()
    {
        sprites = Resources.LoadAll<Sprite>("UI/BarSprites");
        pieces.Add(transform.Find("Start").gameObject);
        pieces.Add(transform.Find("End").gameObject);
    }

    private void UpdateBar ()
    {
        if (pieces.Count != maxValue)
        {
            int diff = Mathf.Abs(pieces.Count - maxValue);

            if (pieces.Count > maxValue && pieces.Count > 1)
            {
                // remove pieces
                for (int i = 0; i < diff; i++)
                {
                    Destroy(pieces[1]);
                    pieces.RemoveAt(1);
                }
            }
            else
            {
                for (int i = 0; i < diff; i++)
                {
                    // add pieces
                    GameObject g = Instantiate((GameObject)Resources.Load("UI/StatBar"));
                    g.transform.SetParent(transform, false);
                    g.transform.SetSiblingIndex(3);
                    pieces.Insert(1, g);
                }
            }
        }

        for (int i = 0; i < pieces.Count; i++)
        {
            Image im = pieces[i].GetComponent<Image>();

            if (i == 0)
            {
                if (pieces.Count == 1)
                {
                    // set the sprite to either 0, or 1 if the player's health is lower than
                    // or equal to the value of i.
                    // this switches it between either the full or empty sprite in one line.
                    im.sprite = sprites[value > i ? 5 : 4];
                }
                else
                {
                    // we do the same here, except we use different sprites as this is the start
                    // of the bar, rather than a single piece.
                    im.sprite = sprites[value > i ? 7 : 6];
                }
            }
            else if (i == pieces.Count - 1)
            {
                // end of the bar
                im.sprite = sprites[value > i ? 1 : 0];
            }
            else
            {
                // middle sections of the bar
                im.sprite = sprites[value > i ? 3 : 2];
            }

            if (value > i)
            {
                // set the colour of the bar to the active colour.
                im.color = active;
            }
            else
            {
                // set the colour of the bar to the inactive colour.
                im.color = inactive;
            }
        }
    }
}
