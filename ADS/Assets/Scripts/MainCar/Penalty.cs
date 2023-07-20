using System.Collections;
using UnityEngine;

public class Penalty : MonoBehaviour
{
    private static int scores = 100;// Очки вождения

    private void FixedUpdate()
    {
        if ((Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) && MainCarController.GetSpeed() > 1 && !Lights.GetStatusLights())// Если двигаемся с выключенными фарами
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

    private bool isStartCoroutine = false;

    /// <summary>
    /// Функция вычета очков при ДТП с другим транспортом
    /// </summary>
    private void TrafficAccident()
    {
        if (!isStartCoroutine)
        StartCoroutine(SubtractionScores(0));
    }

    /// <summary>
    /// Функция вычета очков при езде с выключенными фарами
    /// </summary>
    private void CarLightsOff()
    {
        if (!isStartCoroutine)
        StartCoroutine(SubtractionScores(1));
    }

    /// <summary>
    /// Функция вычета очков в зависимости от индекса нарушения
    /// </summary>
    /// <param name="indexPenalty"></param>
    /// <returns></returns>
    private IEnumerator SubtractionScores(int indexPenalty)
    {
        isStartCoroutine = true;

        yield return new WaitForSeconds(0.5f);

        switch (indexPenalty)
        {
            case 0: scores -= 30; break;
            case 1: scores -= 10; break;
        }

        isStartCoroutine = false;
    }

    /// <summary>
    /// Функция получения очков вождения
    /// </summary>
    /// <returns></returns>
    public static int GetScores()
    {
        return scores;
    }
}
