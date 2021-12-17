using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuOptions : MonoBehaviour
{
    public static MenuOptions instance;

    public Slider sensSlider;
    public Text sensText;
    public int sens;
    public Slider fovSlider;
    public Text fovText;
    public int fov;
    public Slider LoadingBar;

    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }
    }

    public void ChangeSens()
    {
        sensText.text = ($"Sensitivity: {sensSlider.value}");
        sens = (int)sensSlider.value;
    }

    public void ChangeFov()
    {
        fovText.text = ($"Field Of View: {fovSlider.value}");
        fov = (int)fovSlider.value;
    }
}
