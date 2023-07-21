using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FrontBackLights// Класс, который хранит в себя левую и правую фары каждой пары
{
    [Header("Правая фара")]
    public Light rightHeadlight;// Правая фара
    [Header("Левая фара")]
    public Light leftHeadlight;// Левая фара
}

[System.Serializable]
public class TurnSignalLights// Класс, который хранит в себя левый и правый поворотники каждой пары
{
    [Header("Правый поворотник")]
    public Light rightTurnSignalLight;// Правый поворотник
    [Header("Левый поворотник")]
    public Light leftTurnSignalLight;// Левый поворотник
}

[System.Serializable]
public class BackLights// Класс, который хранит в себе левую и правую зпдние фары каждой пары
{
    [Header("Правая задняя фара")]
    public Light rightBackLight;// Правая задняя фара
    [Header("Левая задняя фара")]
    public Light leftBackLight;// Левая задняя фара
}

[System.Serializable]
public class SpecialBackLights// Класс, который хранит в себе левую и правую задней специальной фары каждой пары
{
    [Header("Правая задняя специальная фара (При сдаче назад)")]
    public Light rightSpecialBackLight;// Правая задняя фара
    [Header("Левая задняя специальная фара (При сдаче назад)")]
    public Light leftSpecialBackLight;// Левая задняя фара
}

public class Lights : MonoBehaviour
{
    [Header("Кол-во пар фар")]
    public List<FrontBackLights> frontBackLights;// Лист пар фар
    [Header("Кнопка включения/выключения фар")]
    public KeyCode keyHeadlights;// Кнопка, при нажатии на которую происходит включение/выключение фар

    [Header("Кол-во пар задних фар")]
    public List<BackLights> backLights;// Лист пар задних фар
    [Header("Максимальная яркость задних фар")]
    public float maxBrightness;// Максимальная яркость задних фар
    [Header("Стандартная яркость задних фар")]
    public float defaultBrightness;// Стандартная яркость задних фар

    [Header("Кол-во пар специальных задних фар")]
    public List<SpecialBackLights> specialBackLights;// Лист пар специальных задних фар
    [Header("Максимальная яркость задних фар (при сдачи назад)")]
    public float maxSpecialLightBrightness;// Максимальная яркость специальных задних фар (при торможении)
    [Header("Стандартная яркость специальных задних фар (при сдачи назад)")]
    public float defaultSpecailLightBrightness;// Стандартная яркость специальных задних фар (при торможении)

    [Header("Кол-во пар поворотников")]
    public List<TurnSignalLights> turnSignalLights;// Лист пар поворотников
    [Header("Кнопка включения/выключения правого поворотника")]
    public KeyCode keyRightTurnSignal;// Кнопка, при нажатии на которую происходит включение/выключение правого поворотника
    [Header("Кнопка включения/выключения левого поворотника")]
    public KeyCode keyLeftTurnSignal;// Кнопка, при нажатии на которую происходит включение/выключение левого поворотника

    private static bool isLightsOn = false;// Отслеживание состояния активности фар

    private bool isMovingBack = false;// Отслеживание езды машины назад

    private bool isStartCoroutine = false;// Отслеживание работы карутина
    private bool isTurnSignalOn = false;// Отслеживание работы поворотников
    private bool isLeftTurnSignalOn = false;// Отслеживание какой поворотник включен

    private void Update()
    {
        if (Input.GetKeyDown(keyHeadlights))// Включение/Выключение фар
        {
            OnLights();
        }

        if ((Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.Space)) && isLightsOn)// Увеличение/уменьшение яркости задних фар
        {
            isMovingBack = true;
            BrightnessBackLights();
        }
        else 
        {
            isMovingBack = false;
            BrightnessBackLights();
        }
   

        if (Input.GetKeyDown(keyRightTurnSignal))// Включение/Выключение правого поворотника
        {
            isLeftTurnSignalOn = false;
            isTurnSignalOn = !isTurnSignalOn;
            BreakTurnSignal();
        }
        else if (Input.GetKeyDown(keyLeftTurnSignal))// Включение/Выключение левого поворотника
        {
            isLeftTurnSignalOn = true;
            isTurnSignalOn = !isTurnSignalOn;
            BreakTurnSignal();
        }

        if (isTurnSignalOn && !isStartCoroutine)// Запуск мигания поворотников
        {
            StartCoroutine(FlashingTurnSignal());
        }
    }

    private void OnLights()// Включение и выключение фар
    {
        isLightsOn = !isLightsOn;

        foreach (var twoHeadlights in frontBackLights)
        {
            twoHeadlights.leftHeadlight.enabled = !twoHeadlights.leftHeadlight.enabled;// Меняем значение левой фары на противоположное
            twoHeadlights.rightHeadlight.enabled = !twoHeadlights.rightHeadlight.enabled;// Меняем значение правой фары на противоположное
        }
    }

    private void BreakTurnSignal()// Отключает все поворотники
    {
        StopAllCoroutines();
        isStartCoroutine = false;

        foreach (var twoTurnSignalLights in turnSignalLights)
        {
            twoTurnSignalLights.leftTurnSignalLight.enabled = false;    
            twoTurnSignalLights.rightTurnSignalLight.enabled = false;
        }
    }

    private IEnumerator FlashingTurnSignal()// Заставляет поворотники мигать
    {
        isStartCoroutine = true;
        yield return new WaitForSeconds(0.5f);
        
        foreach(var twoTurnSignalLights in turnSignalLights)
        {
            if (isLeftTurnSignalOn && isTurnSignalOn)
            {
                twoTurnSignalLights.leftTurnSignalLight.enabled = !twoTurnSignalLights.leftTurnSignalLight.enabled;
            }
            else
            {
                twoTurnSignalLights.rightTurnSignalLight.enabled = !twoTurnSignalLights.rightTurnSignalLight.enabled;
            }  
        }

        isStartCoroutine = false;
    }

    private void BrightnessBackLights()// Настройка яркости задних фар в зависимости от движения
    {
        if(MainCarController.GetSpeed() < 0 && isMovingBack)
        {
            foreach(var twoSpecialBackLights in specialBackLights)
            {
                if (twoSpecialBackLights.rightSpecialBackLight.intensity < maxSpecialLightBrightness && twoSpecialBackLights.rightSpecialBackLight.intensity < maxSpecialLightBrightness)
                {
                    twoSpecialBackLights.rightSpecialBackLight.intensity = maxSpecialLightBrightness;
                    twoSpecialBackLights.leftSpecialBackLight.intensity = maxSpecialLightBrightness;
                }
            }

            foreach(var twoBackLights in backLights)
            {
                if(specialBackLights.Count != 0)
                {
                    twoBackLights.rightBackLight.intensity = 0;
                    twoBackLights.leftBackLight.intensity = 0;
                }
                else
                {
                    twoBackLights.rightBackLight.intensity = maxBrightness;
                    twoBackLights.leftBackLight.intensity = maxBrightness;
                }   
            }
        }
        else if (isMovingBack)
        {
            foreach (var twoSpecialBackLights in specialBackLights)
            {
                twoSpecialBackLights.rightSpecialBackLight.intensity = 0;
                twoSpecialBackLights.leftSpecialBackLight.intensity = 0;
            }

            foreach (var twoBackLights in backLights)
            {
                if (twoBackLights.rightBackLight.intensity < maxBrightness && twoBackLights.leftBackLight.intensity < maxBrightness)
                {
                    twoBackLights.rightBackLight.intensity = maxBrightness;
                    twoBackLights.leftBackLight.intensity = maxBrightness;
                }
            }
        }
        else
        {
            foreach (var twoSpecialBackLights in specialBackLights)
            {
                twoSpecialBackLights.rightSpecialBackLight.intensity = 0;
                twoSpecialBackLights.leftSpecialBackLight.intensity = 0;
            }

            foreach (var twoBackLights in backLights)
            {
                twoBackLights.rightBackLight.intensity = defaultBrightness;
                twoBackLights.leftBackLight.intensity = defaultBrightness;
            }
        }
    }

    public static bool GetStatusLights()
    {
        return isLightsOn;
    }
}