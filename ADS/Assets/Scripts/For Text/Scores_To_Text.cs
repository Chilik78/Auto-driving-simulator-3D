using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Scores_To_Text : MonoBehaviour
{
    [Header("Текст содержащий общее кол-во очков")]
    public Text scoresText;// Текст содержащий общее кол-во очков
    [Header("Текст содержащий кол-во отнимаемых очков")]
    public Text visualSubScores;// Текст содержащий кол-во отнимаемых очков

    private static int lastScores = 100;// Переменная сохранения последних очков
    private int deltaScores;// Переменная разности текущих и последних очков

    private void OnEnable()
    {
        scoresText.text = "Очки: " + lastScores;// При включении камеры выставляем значение
    }

    private void OnDisable()
    {
        lastScores = 100;// Возвращаем исходное значение lastScores
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
            scoresText.text = "Очки: " + lastScores;
            StartCoroutine(MoveText());
        }
    }

    /// <summary>
    /// Функция плавно перемещает текст отниманя очков
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
