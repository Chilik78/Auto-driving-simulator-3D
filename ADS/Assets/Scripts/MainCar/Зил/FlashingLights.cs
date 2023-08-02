using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashingLights : MonoBehaviour
{
    [Header("Кнопка включения/выключения мигалок")]
    public KeyCode flashingLightsKey;// Кнопка включения/выключения мигалок
    [Header("Кол-во мигалок")]
    public List<Light> flashingLights;// Лист со всеми мигалками

    private bool isFlashingLightsOn = false;

    private void OnDisable()
    {
        isFlashingLightsOn = false;// Возвращаем в исходное состояние
    }

    void Update()
    {
        if(Input.GetKeyDown(flashingLightsKey) && !isFlashingLightsOn)// Включение мигалок
        {
            isFlashingLightsOn = true;
            SwitchStateFlashLight();
        }
        else if (Input.GetKeyDown(flashingLightsKey) && isFlashingLightsOn)// Выключение мигалок
        {
            isFlashingLightsOn = false;
            SwitchStateFlashLight();
        }

        if (isFlashingLightsOn)// Кручение мигалок
        {
            rotateFlashLights();
        }
    }

    /// <summary>
    /// Функция кручения мигалок
    /// </summary>
    private void rotateFlashLights()
    {
        foreach(Light flashLight in flashingLights)
        {
            flashLight.transform.Rotate(new Vector3(0f, Time.deltaTime * 400f, 0f));
        }
    }

    /// <summary>
    /// Функция переключения состояния мигалок
    /// </summary>
    private void SwitchStateFlashLight()
    {
        foreach (Light flashLight in flashingLights)
        {
            flashLight.enabled = !flashLight.enabled;
        }
    }
}
