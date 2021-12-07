using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuOptions : MonoBehaviour
{
    public Slider sensSlider;
    public Text sensText;
    public int sens;

    public void ChangeSens()
    {
        sensText.text = ($"Sensitivity: {sensSlider.value}");
        sens = (int)sensSlider.value;
    }
}
