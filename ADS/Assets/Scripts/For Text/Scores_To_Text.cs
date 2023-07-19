using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Scores_To_Text : MonoBehaviour
{
    public Text visualSubScores;
    public Canvas currentCanvas;
    private int lastScores = 100;
    public Text scoresText;

    private Vector3 startPos;
    private Vector3 endPos;
    private float desiredDuration = 3f;
    private float elapsedTime = 0;
    private bool isMoving = false;

    void Update()
    {

        /*if (*//*lastScores != Penalty.GetScores() &&*//* !isMoving && Input.GetKeyDown(KeyCode.H))
        {


            isMoving = true;

            if(scoresText.transform.childCount < 1)
            {
                Instantiate(visualSubScores, currentCanvas.transform);
                visualSubScores.text = $"{Penalty.GetScores() - lastScores}";
                visualSubScores.rectTransform.position = new Vector3(0, 0, 0);
                startPos = scoresText.rectTransform.position;
                endPos = new Vector3(visualSubScores.rectTransform.position.x + 100, visualSubScores.rectTransform.position.y, visualSubScores.rectTransform.position.z);
            }

            StartCoroutine(MovingSubstraction());
            lastScores = Penalty.GetScores();
            scoresText.text = "Очки: " + lastScores;
        }*/

        scoresText.text = "Очки: " + Penalty.GetScores();
    }


    private IEnumerator MovingSubstraction()
    {
        isMoving = true;

  

        yield return new WaitForSeconds(1.0f);

        Debug.Log("Zachel v Coroutine");

        if (isMoving)
        {
            elapsedTime += Time.deltaTime;

            visualSubScores.rectTransform.position = Vector3.Lerp(startPos, endPos, Mathf.SmoothStep(0, 1, elapsedTime));

            if (visualSubScores.rectTransform.position == endPos)
            {
                Debug.Log("END");
                elapsedTime = 0;
                Destroy(visualSubScores);
                StopAllCoroutines();
                isMoving = false;
            }

            StartCoroutine(MovingSubstraction());
        }
    }
}
