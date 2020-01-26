using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StatText : MonoBehaviour
{
    public enum Type { Stat, Currency }

    public float value
    {
        get
        {
            return Value;
        }
        set
        {
            // ensure that the value actually changes
            if (Value != value)
            {
                // get the difference between the two values
                float diff = value - Value;
                // set the value to equal the new value
                Value = value;
                // update the text
                UpdateText();
                // if an indicator exists, update it
                if (indicator != null)
                {
                    UpdateIndicator(diff);
                }
            }
        }
    }
    public float maxValue
    {
        get
        {
            return MaxValue;
        }
        set
        {
            MaxValue = value;
            UpdateText();
        }
    }
    private float Value;
    private float MaxValue;

    public Type type;

    public TextMeshProUGUI text;
    public TextMeshProUGUI indicator;

    private Coroutine FadeIndicator;

    private void UpdateText()
    {
        switch (type)
        {
            case Type.Stat:
                text.text = value.ToString("n0") + "/" + maxValue.ToString("n0");
                break;
            case Type.Currency:
                text.text = value.ToString("n0") + "GP";
                break;
        }
    }
    private void UpdateIndicator(float val)
    {
        bool negative = val < 0.0F;
        indicator.text = (negative ? "" : "+") + val.ToString("n0");

        if (FadeIndicator != null)
            StopCoroutine(FadeIndicator);
        FadeIndicator = StartCoroutine(DoFadeIndicator());
    }

    private IEnumerator DoFadeIndicator()
    {
        CanvasGroup group = indicator.GetComponent<CanvasGroup>();

        float alpha = 1.0F;
        do
        {
            alpha = Mathf.Clamp01(alpha - Time.deltaTime);
            group.alpha = alpha;

            yield return null;
        }
        while (alpha != 0.0F);
    }
}
