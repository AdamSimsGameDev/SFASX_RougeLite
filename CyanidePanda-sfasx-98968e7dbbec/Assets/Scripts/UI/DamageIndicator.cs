using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamageIndicator : MonoBehaviour
{
    public TextMeshProUGUI text;
    public CanvasGroup group;
    
    private bool isActive;
    private float value = 1.0F;
    private float fadeSpeed = 1.0F;
    private float riseSpeed = 1.0F;

    private void Update()
    {
        group.alpha = value;

        if (isActive)
        {
            value = Mathf.Clamp01(value - Time.deltaTime * fadeSpeed);

            transform.Translate(Vector3.up * Time.deltaTime * riseSpeed);

            if (value == 0.0F)
            {
                Destroy(gameObject);
            }
        }
    }

    public void SetValue(int value)
    {
        text.text = value.ToString("n0");

        isActive = true;
    }
}
