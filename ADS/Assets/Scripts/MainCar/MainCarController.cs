using UnityEngine;
using System.Collections.Generic;
using System.Collections;

[System.Serializable]
public class AxleInfo
{
    [Header("����� ������")]
    public WheelCollider leftWheel;
    [Header("������ ������")]
    public WheelCollider rightWheel;
    [Header("�����?")]
    public bool motor;
    [Header("��������������?")]
    public bool steering;
}

public class MainCarController : MonoBehaviour
{
    [Header("��� �����")]
    public List<AxleInfo> axleInfos; // ��� �����

    [Header("������������ �������� ������ ���������")]
    public float maxMotorTorque; // ������������ �������� ������ ���������

    [Header("������������ ���� �������� �����")]
    public float maxSteeringAngle; // ������������ ���� �������� �����

    [Header("�������� ��������")]
    public float brakes; // �������� ��������

    private static int scores = 100;// ���� ��������

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
    /// ������� ������� ���������� ����� ����� � ������������� ����� ����������
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

    private static float speed;// �������� ������
    private bool isPositive;// ������������ ����������� �������� ������������ ������

    /// <summary>
    /// ������� ������� �������� ������
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
    /// ������� �������� ������, ���� ���� �������� <= 0
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
    /// ������� ��������� �������� ����������
    /// </summary>
    /// <returns></returns>
    public static float GetSpeed()
    {
        return speed;
    }

    private static bool isStartCoroutine = false;
    /// <summary>
    /// ������� ������ ����� � ����������� �� ������� ���������
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
    /// ������� ��������� ��������� ��������
    /// </summary>
    /// <returns></returns>
    public static bool GetStateCoroutine()
    {
        return isStartCoroutine;
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