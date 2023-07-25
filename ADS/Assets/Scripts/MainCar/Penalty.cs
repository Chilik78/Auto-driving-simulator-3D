using System;
using UnityEngine;

public class Penalty : MonoBehaviour
{
    private void FixedUpdate()
    {
        if ((Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) && Math.Abs(MainCarController.GetSpeed()) > 1 && !Lights.GetStatusLights())// ���� ��������� � ������������ ������
        {
            CarLightsOff();
        }
    }

    /// <summary>
    /// �������� � ������ ��� � ������ �����������
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Other cars")
        {
            TrafficAccident();
        }
    }

    /// <summary>
    /// ������� ������ ����� ��� ��� � ������ �����������
    /// </summary>
    private void TrafficAccident()
    {
        if (!MainCarController.GetStateCoroutine())
        StartCoroutine(MainCarController.SubtractionScores(0));
    }

    /// <summary>
    /// ������� ������ ����� ��� ���� � ������������ ������
    /// </summary>
    private void CarLightsOff()
    {
        if (!MainCarController.GetStateCoroutine())
        StartCoroutine(MainCarController.SubtractionScores(1));
    }

    
}
