using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FrontBackLights// �����, ������� ������ � ���� ����� � ������ ���� ������ ����
{
    [Header("������ ����")]
    public Light rightHeadlight;// ������ ����
    [Header("����� ����")]
    public Light leftHeadlight;// ����� ����
}

[System.Serializable]
public class TurnSignalLights// �����, ������� ������ � ���� ����� � ������ ����������� ������ ����
{
    [Header("������ ����������")]
    public Light rightTurnSignalLight;// ������ ����������
    [Header("����� ����������")]
    public Light leftTurnSignalLight;// ����� ����������
}

[System.Serializable]
public class BackLights// �����, ������� ������ � ���� ����� � ������ ������ ���� ������ ����
{
    [Header("������ ������ ����")]
    public Light rightBackLight;// ������ ������ ����
    [Header("����� ������ ����")]
    public Light leftBackLight;// ����� ������ ����
}

[System.Serializable]
public class SpecialBackLights// �����, ������� ������ � ���� ����� � ������ ������ ����������� ���� ������ ����
{
    [Header("������ ������ ����������� ���� (��� ����� �����)")]
    public Light rightSpecialBackLight;// ������ ������ ����
    [Header("����� ������ ����������� ���� (��� ����� �����)")]
    public Light leftSpecialBackLight;// ����� ������ ����
}

public class Lights : MonoBehaviour
{
    [Header("���-�� ��� ���")]
    public List<FrontBackLights> frontBackLights;// ���� ��� ���
    [Header("������ ���������/���������� ���")]
    public KeyCode keyHeadlights;// ������, ��� ������� �� ������� ���������� ���������/���������� ���

    [Header("���-�� ��� ������ ���")]
    public List<BackLights> backLights;// ���� ��� ������ ���
    [Header("������������ ������� ������ ���")]
    public float maxBrightness;// ������������ ������� ������ ���
    [Header("����������� ������� ������ ���")]
    public float defaultBrightness;// ����������� ������� ������ ���

    [Header("���-�� ��� ����������� ������ ���")]
    public List<SpecialBackLights> specialBackLights;// ���� ��� ����������� ������ ���
    [Header("������������ ������� ������ ��� (��� ����� �����)")]
    public float maxSpecialLightBrightness;// ������������ ������� ����������� ������ ��� (��� ����������)
    [Header("����������� ������� ����������� ������ ��� (��� ����� �����)")]
    public float defaultSpecailLightBrightness;// ����������� ������� ����������� ������ ��� (��� ����������)

    [Header("���-�� ��� ������������")]
    public List<TurnSignalLights> turnSignalLights;// ���� ��� ������������
    [Header("������ ���������/���������� ������� �����������")]
    public KeyCode keyRightTurnSignal;// ������, ��� ������� �� ������� ���������� ���������/���������� ������� �����������
    [Header("������ ���������/���������� ������ �����������")]
    public KeyCode keyLeftTurnSignal;// ������, ��� ������� �� ������� ���������� ���������/���������� ������ �����������

    private static bool isLightsOn = false;// ������������ ��������� ���������� ���

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

        if ((Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.Space)) && isLightsOn)// ����������/���������� ������� ������ ���
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

    private void OnLights()// ��������� � ���������� ���
    {
        isLightsOn = !isLightsOn;

        foreach (var twoHeadlights in frontBackLights)
        {
            twoHeadlights.leftHeadlight.enabled = !twoHeadlights.leftHeadlight.enabled;// ������ �������� ����� ���� �� ���������������
            twoHeadlights.rightHeadlight.enabled = !twoHeadlights.rightHeadlight.enabled;// ������ �������� ������ ���� �� ���������������
        }
    }

    private void BreakTurnSignal()// ��������� ��� �����������
    {
        StopAllCoroutines();
        isStartCoroutine = false;

        foreach (var twoTurnSignalLights in turnSignalLights)
        {
            twoTurnSignalLights.leftTurnSignalLight.enabled = false;    
            twoTurnSignalLights.rightTurnSignalLight.enabled = false;
        }
    }

    private IEnumerator FlashingTurnSignal()// ���������� ����������� ������
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

    private void BrightnessBackLights()// ��������� ������� ������ ��� � ����������� �� ��������
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