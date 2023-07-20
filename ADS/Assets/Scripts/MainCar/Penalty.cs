using System.Collections;
using UnityEngine;

public class Penalty : MonoBehaviour
{
    private static int scores = 100;// ���� ��������

    private void FixedUpdate()
    {
        if ((Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) && MainCarController.GetSpeed() > 1 && !Lights.GetStatusLights())// ���� ��������� � ������������ ������
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

    private bool isStartCoroutine = false;

    /// <summary>
    /// ������� ������ ����� ��� ��� � ������ �����������
    /// </summary>
    private void TrafficAccident()
    {
        if (!isStartCoroutine)
        StartCoroutine(SubtractionScores(0));
    }

    /// <summary>
    /// ������� ������ ����� ��� ���� � ������������ ������
    /// </summary>
    private void CarLightsOff()
    {
        if (!isStartCoroutine)
        StartCoroutine(SubtractionScores(1));
    }

    /// <summary>
    /// ������� ������ ����� � ����������� �� ������� ���������
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
    /// ������� ��������� ����� ��������
    /// </summary>
    /// <returns></returns>
    public static int GetScores()
    {
        return scores;
    }
}
