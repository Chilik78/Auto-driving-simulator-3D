using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashingLights : MonoBehaviour
{
    [Header("������ ���������/���������� �������")]
    public KeyCode flashingLightsKey;// ������ ���������/���������� �������
    [Header("���-�� �������")]
    public List<Light> flashingLights;// ���� �� ����� ���������

    private bool isFlashingLightsOn = false;

    private void OnDisable()
    {
        isFlashingLightsOn = false;// ���������� � �������� ���������
    }

    void Update()
    {
        if(Input.GetKeyDown(flashingLightsKey) && !isFlashingLightsOn)// ��������� �������
        {
            isFlashingLightsOn = true;
            SwitchStateFlashLight();
        }
        else if (Input.GetKeyDown(flashingLightsKey) && isFlashingLightsOn)// ���������� �������
        {
            isFlashingLightsOn = false;
            SwitchStateFlashLight();
        }

        if (isFlashingLightsOn)// �������� �������
        {
            rotateFlashLights();
        }
    }

    /// <summary>
    /// ������� �������� �������
    /// </summary>
    private void rotateFlashLights()
    {
        foreach(Light flashLight in flashingLights)
        {
            flashLight.transform.Rotate(new Vector3(0f, Time.deltaTime * 400f, 0f));
        }
    }

    /// <summary>
    /// ������� ������������ ��������� �������
    /// </summary>
    private void SwitchStateFlashLight()
    {
        foreach (Light flashLight in flashingLights)
        {
            flashLight.enabled = !flashLight.enabled;
        }
    }
}
