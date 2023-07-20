using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Scores_To_Text : MonoBehaviour
{
    public Text scoresText;
    public Text visualSubScores;
    private int lastScores = 100;
    

    private int deltaScores;
    void Update()
    {
        if (lastScores != Penalty.GetScores() && !visualSubScores.GetComponent<Text>().enabled)
        {
            deltaScores = Penalty.GetScores() - lastScores;
            lastScores = Penalty.GetScores();
            scoresText.text = "Очки: " + lastScores;
            StartCoroutine(MoveText());
        }
    }

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
