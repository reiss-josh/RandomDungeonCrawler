using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplaySliderValue : MonoBehaviour
{
    Text textComponent;

    void Start()
        {textComponent = GetComponent<Text>();}

    public void SetSliderValue(float sliderValue)
    {
        int sliderSaved = (int)(Mathf.Pow((sliderValue/20), 1.75f));
        textComponent.text = sliderSaved.ToString();
    }
}
