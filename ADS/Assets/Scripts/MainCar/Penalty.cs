using System;
using UnityEngine;

public class Penalty : MonoBehaviour
{
    private void FixedUpdate()
    {
        if ((Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) && Math.Abs(MainCarController.GetSpeed()) > 1 && !Lights.GetStatusLights())// Если двигаемся с выключенными фарами
        {
            CarLightsOff();
        }
    }

    /// <summary>
    /// Работает в случае ДТП с другим транспортом
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
    /// Функция вычета очков при ДТП с другим транспортом
    /// </summary>
    private void TrafficAccident()
    {
        if (!MainCarController.GetStateCoroutine())
        StartCoroutine(MainCarController.SubtractionScores(0));
    }

    /// <summary>
    /// Функция вычета очков при езде с выключенными фарами
    /// </summary>
    private void CarLightsOff()
    {
        if (!MainCarController.GetStateCoroutine())
        StartCoroutine(MainCarController.SubtractionScores(1));
    }

    
}
