using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FrontBackLights// Класс, который хранит в себя левую и правую фары каждой пары
{
    public Light rightHeadlight;// Правая фара
    public Light leftHeadlight;// Левая фара
}

[System.Serializable]
public class TurnSignalLights// Класс, который хранит в себя левую и правую фары каждой пары
{
    public Light rightTurnSignalLight;// Правая фара
    public Light leftTurnSignalLight;// Левая фара
}

[System.Serializable]
public class BackLights// Класс, который хранит в себя левую и правую фары каждой пары
{
    public Light rightBackLight;// Правая задняя фара
    public Light leftBackLight;// Левая задняя фара
}

[System.Serializable]
public class SpecialBackLights// Класс, который хранит в себя левую и правую фары каждой пары
{
    public Light rightSpecialBackLight;// Правая задняя фара
    public Light leftSpecialBackLight;// Левая задняя фара
}

public class Lights : MonoBehaviour
{
    public List<FrontBackLights> frontBackLights;// Лист пар фар
    public KeyCode keyHeadlights;// Кнопка, при нажатии на которую происходит включение/выключение фар

    public List<BackLights> backLights;// Лист пар задних фар
    public float maxBrightness;// Максимальная яркость задних фар
    public float defaultBrightness;// Стандартная яркость задних фар
    public List<SpecialBackLights> specialBackLights;// Лист пар специальных задних фар
    public float maxSpecialLightBrightness;// Максимальная яркость задних фар (при торможении)
    public float defaultSpecailLightBrightness;// Стандартная яркость задних фар (при торможении)

    public List<TurnSignalLights> turnSignalLights;// Лист пар поворотников
    public KeyCode keyRightTurnSignal;// Кнопка, при нажатии на которую происходит включение/выключение правого поворотника
    public KeyCode keyLeftTurnSignal;// Кнопка, при нажатии на которую происходит включение/выключение левого поворотника

    private bool isLightsOn = false;// Отслеживание состояния активности фар

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

        if ((Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) && isLightsOn)// Увеличение/уменьшение яркости задних фар
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

    public void OnLights()// Включение и выключение фар
    {
        isLightsOn = !isLightsOn;

        foreach (var twoHeadlights in frontBackLights)
        {
            twoHeadlights.leftHeadlight.enabled = !twoHeadlights.leftHeadlight.enabled;// Меняем значение левой фары на противоположное
            twoHeadlights.rightHeadlight.enabled = !twoHeadlights.rightHeadlight.enabled;// Меняем значение правой фары на противоположное
        }
    }

    public void BreakTurnSignal()// Отключает все поворотники
    {
        StopAllCoroutines();
        isStartCoroutine = false;

        foreach (var twoTurnSignalLights in turnSignalLights)
        {
            twoTurnSignalLights.leftTurnSignalLight.enabled = false;    
            twoTurnSignalLights.rightTurnSignalLight.enabled = false;
        }
    }

    IEnumerator FlashingTurnSignal()// Заставляет поворотники мигать
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

    public void BrightnessBackLights()// Настройка яркости задних фар в зависимости от движения
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
                twoBackLights.rightBackLight.intensity = 0;
                twoBackLights.leftBackLight.intensity = 0;
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
}