using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PowerMeter : MonoBehaviour
{
    public Slider slider;

    public void SetPower(float power)
    {
        slider.value = power;
    }
}
