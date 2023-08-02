using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Scores_To_Text : MonoBehaviour
{
    [Header("����� ���������� ����� ���-�� �����")]
    public Text scoresText;// ����� ���������� ����� ���-�� �����
    [Header("����� ���������� ���-�� ���������� �����")]
    public Text visualSubScores;// ����� ���������� ���-�� ���������� �����

    private static int lastScores = 100;// ���������� ���������� ��������� �����
    private int deltaScores;// ���������� �������� ������� � ��������� �����

    private void OnEnable()
    {
        scoresText.text = "����: " + lastScores;// ��� ��������� ������ ���������� ��������
    }

    private void OnDisable()
    {
        lastScores = 100;// ���������� �������� �������� lastScores
        StopAllCoroutines();
        visualSubScores.GetComponent<Text>().enabled = false;
        visualSubScores.GetComponent<Animator>().enabled = false;
    }

    void Update()
    {
        if (lastScores != MainCarController.GetScores() && !visualSubScores.GetComponent<Text>().enabled)
        {
            deltaScores = MainCarController.GetScores() - lastScores;
            lastScores = MainCarController.GetScores();
            scoresText.text = "����: " + lastScores;
            StartCoroutine(MoveText());
        }
    }

    /// <summary>
    /// ������� ������ ���������� ����� �������� �����
    /// </summary>
    /// <returns></returns>
    IEnumerator MoveText()
    {
        visualSubScores.GetComponent<Animator>().enabled = true;
        visualSubScores.GetComponent<Text>().enabled = true;
        visualSubScores.GetComponent<Text>().text = deltaScores.ToString();
        yield return new WaitForSeconds(0.5f);
        visualSubScores.GetComponent<Text>().enabled = false;
        visualSubScores.GetComponent<Animator>().enabled = false;
    }
}
