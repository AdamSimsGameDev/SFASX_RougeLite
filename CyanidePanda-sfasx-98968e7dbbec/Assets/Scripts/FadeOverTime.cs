using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeOverTime : MonoBehaviour
{
    public bool destroy;
    public float delay;
    public float fadeTime;

    private float timer;
    private bool hasStarted;

    private CanvasGroup group;

    private void Awake()
    {
        group = GetComponent<CanvasGroup>();
    }
    private void Update()
    {
        timer += Time.deltaTime;
        if (!hasStarted)
        {
            if (timer >= delay)
            {
                timer = 0.0F;
                hasStarted = true;
            }
        }
        else
        {
            group.alpha = 1.0F - (timer / fadeTime);

            if (timer >= fadeTime)
            {
                if (destroy) Destroy(gameObject);
                else group.alpha = 0.0F;
            }
        }
    }
}
