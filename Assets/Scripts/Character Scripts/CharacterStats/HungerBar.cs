using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HungerBar : MonoBehaviour
{

    public Slider slider;
    public Gradient gradient;
    public RawImage fill;
    public float sliderValue;

    public void Awake()
    {
        //slider = GameObject.Find(transform.name).GetComponent<Slider>();
        slider.value = 5f;
        sliderValue = slider.value;
    }

    public void Update()
    {
        sliderValue = slider.value;
        fill.color = gradient.Evaluate(slider.normalizedValue);
    }
    public void FixedUpdate()
    {
        sliderValue = slider.value;
        fill.color = gradient.Evaluate(slider.normalizedValue);
    }

    public void setMaxHunger(float hunger)
    {
        slider.maxValue = hunger;
        slider.value = hunger;
        fill.color = gradient.Evaluate(1f);
    }

    public void setHunger(float hunger)
    {
        if (slider.value != slider.maxValue)
        {
            slider.value = hunger;
            fill.color = gradient.Evaluate(slider.normalizedValue);
        }
    }
    

}
