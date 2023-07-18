using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FrontBackLights// �����, ������� ������ � ���� ����� � ������ ���� ������ ����
{
    public Light rightHeadlight;// ������ ����
    public Light leftHeadlight;// ����� ����
}

[System.Serializable]
public class TurnSignalLights// �����, ������� ������ � ���� ����� � ������ ���� ������ ����
{
    public Light rightTurnSignalLight;// ������ ����
    public Light leftTurnSignalLight;// ����� ����
}

[System.Serializable]
public class BackLights// �����, ������� ������ � ���� ����� � ������ ���� ������ ����
{
    public Light rightBackLight;// ������ ������ ����
    public Light leftBackLight;// ����� ������ ����
}

[System.Serializable]
public class SpecialBackLights// �����, ������� ������ � ���� ����� � ������ ���� ������ ����
{
    public Light rightSpecialBackLight;// ������ ������ ����
    public Light leftSpecialBackLight;// ����� ������ ����
}

public class Lights : MonoBehaviour
{
    public List<FrontBackLights> frontBackLights;// ���� ��� ���
    public KeyCode keyHeadlights;// ������, ��� ������� �� ������� ���������� ���������/���������� ���

    public List<BackLights> backLights;// ���� ��� ������ ���
    public float maxBrightness;// ������������ ������� ������ ���
    public float defaultBrightness;// ����������� ������� ������ ���
    public List<SpecialBackLights> specialBackLights;// ���� ��� ����������� ������ ���
    public float maxSpecialLightBrightness;// ������������ ������� ������ ��� (��� ����������)
    public float defaultSpecailLightBrightness;// ����������� ������� ������ ��� (��� ����������)

    public List<TurnSignalLights> turnSignalLights;// ���� ��� ������������
    public KeyCode keyRightTurnSignal;// ������, ��� ������� �� ������� ���������� ���������/���������� ������� �����������
    public KeyCode keyLeftTurnSignal;// ������, ��� ������� �� ������� ���������� ���������/���������� ������ �����������

    private bool isLightsOn = false;// ������������ ��������� ���������� ���

    private bool isMovingBack = false;// ������������ ���� ������ �����

    private bool isStartCoroutine = false;// ������������ ������ ��������
    private bool isTurnSignalOn = false;// ������������ ������ ������������
    private bool isLeftTurnSignalOn = false;// ������������ ����� ���������� �������

    private void Update()
    {
        if (Input.GetKeyDown(keyHeadlights))// ���������/���������� ���
        {
            OnLights();
        }

        if ((Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) && isLightsOn)// ����������/���������� ������� ������ ���
        {
            isMovingBack = true;
            BrightnessBackLights();
        }
        else 
        {
            isMovingBack = false;
            BrightnessBackLights();
        }
   

        if (Input.GetKeyDown(keyRightTurnSignal))// ���������/���������� ������� �����������
        {
            isLeftTurnSignalOn = false;
            isTurnSignalOn = !isTurnSignalOn;
            BreakTurnSignal();
        }
        else if (Input.GetKeyDown(keyLeftTurnSignal))// ���������/���������� ������ �����������
        {
            isLeftTurnSignalOn = true;
            isTurnSignalOn = !isTurnSignalOn;
            BreakTurnSignal();
        }

        if (isTurnSignalOn && !isStartCoroutine)// ������ ������� ������������
        {
            StartCoroutine(FlashingTurnSignal());
        }
    }

    public void OnLights()// ��������� � ���������� ���
    {
        isLightsOn = !isLightsOn;

        foreach (var twoHeadlights in frontBackLights)
        {
            twoHeadlights.leftHeadlight.enabled = !twoHeadlights.leftHeadlight.enabled;// ������ �������� ����� ���� �� ���������������
            twoHeadlights.rightHeadlight.enabled = !twoHeadlights.rightHeadlight.enabled;// ������ �������� ������ ���� �� ���������������
        }
    }

    public void BreakTurnSignal()// ��������� ��� �����������
    {
        StopAllCoroutines();
        isStartCoroutine = false;

        foreach (var twoTurnSignalLights in turnSignalLights)
        {
            twoTurnSignalLights.leftTurnSignalLight.enabled = false;    
            twoTurnSignalLights.rightTurnSignalLight.enabled = false;
        }
    }

    IEnumerator FlashingTurnSignal()// ���������� ����������� ������
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

    public void BrightnessBackLights()// ��������� ������� ������ ��� � ����������� �� ��������
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