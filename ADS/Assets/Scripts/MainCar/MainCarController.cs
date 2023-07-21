using UnityEngine;
using System.Collections.Generic;
using System.Collections;

[System.Serializable]
public class AxleInfo
{
    [Header("Левое колесо")]
    public WheelCollider leftWheel;
    [Header("Правое колесо")]
    public WheelCollider rightWheel;
    [Header("Мотор?")]
    public bool motor;
    [Header("Поворачиваются?")]
    public bool steering;
}

public class MainCarController : MonoBehaviour
{
    [Header("Оси колес")]
    public List<AxleInfo> axleInfos; // Оси колес

    [Header("Максимальный крутящий момент двигателя")]
    public float maxMotorTorque; // Максимальный крутящий момент двигателя

    [Header("Максимальный угол поворота колес")]
    public float maxSteeringAngle; // Максимальный угол поворота колес

    [Header("Значение тормозов")]
    public float brakes; // Значение тормозов

    private static int scores = 100;// Очки вождения

    public void FixedUpdate()
    {
        float motor = maxMotorTorque * Input.GetAxis("Vertical");
        float steering = maxSteeringAngle * Input.GetAxis("Horizontal");

        foreach (AxleInfo axleInfo in axleInfos)
        {
            if (axleInfo.steering)
            {
                axleInfo.leftWheel.steerAngle = steering;
                axleInfo.rightWheel.steerAngle = steering;
            }

            if (axleInfo.motor)
            {
                axleInfo.leftWheel.motorTorque = motor;
                axleInfo.rightWheel.motorTorque = motor;
            }

            if (Input.GetKey(KeyCode.Space))
            {
                axleInfo.leftWheel.brakeTorque = brakes;
                axleInfo.rightWheel.brakeTorque = brakes;
            }
            else
            {
                axleInfo.leftWheel.brakeTorque = 0;
                axleInfo.rightWheel.brakeTorque = 0;
            }

            ApplyLocalPositionToVisuals(axleInfo.leftWheel);
            ApplyLocalPositionToVisuals(axleInfo.rightWheel);
        }
    }

    private void Update()
    {
        CalculateSpeed();
        DestroyCar();
    }

    /// <summary>
    /// Функция находит визуальную часть колес и устанавливает новые координаты
    /// </summary>
    /// <param name="collider"></param>
    private static void ApplyLocalPositionToVisuals(WheelCollider collider)
    {
        if (collider.transform.childCount == 0)
        {
            return;
        }

        Transform visualWheel = collider.transform.GetChild(0);

        Vector3 position;
        Quaternion rotation;
        collider.GetWorldPose(out position, out rotation);

        visualWheel.transform.position = position;
        visualWheel.transform.position = position;
        visualWheel.transform.rotation = rotation;
    }

    private static float speed;// Скорость машины
    private bool isPositive;// Отслеживание направления скорости относительно машины

    /// <summary>
    /// Функция расчета скорости машины
    /// </summary>
    private void CalculateSpeed()
    {
        speed = gameObject.GetComponent<Rigidbody>().velocity.magnitude;

        if ((Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) && speed <= 0.2)
        {
            isPositive = true;
        }
        else if ((Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) && speed <= 0.2)
        {
            isPositive = false;
        }

        speed = isPositive ? speed : speed * -1;
        speed *= 3.6f;
    }

    /// <summary>
    /// Функция удаления машины, если очки вождения <= 0
    /// </summary>
    private void DestroyCar()
    {
        if(scores <= 0)
        {
            Destroy(gameObject);
            MenuScripts.SwitchOnCameraMenu();
            scores = 100;
            GameObject.FindWithTag("Car Camera").SetActive(false);
        }
    }

    /// <summary>
    /// Функция получения скорости автомобиля
    /// </summary>
    /// <returns></returns>
    public static float GetSpeed()
    {
        return speed;
    }

    private static bool isStartCoroutine = false;
    /// <summary>
    /// Функция вычета очков в зависимости от индекса нарушения
    /// </summary>
    /// <param name="indexPenalty"></param>
    /// <returns></returns>
    public static IEnumerator SubtractionScores(int indexPenalty)
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
    /// Функция получения состояния карутина
    /// </summary>
    /// <returns></returns>
    public static bool GetStateCoroutine()
    {
        return isStartCoroutine;
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