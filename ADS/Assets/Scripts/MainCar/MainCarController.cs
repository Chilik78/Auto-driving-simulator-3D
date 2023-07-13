using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCarController : MonoBehaviour
{
    public List<AxleInfo> axleInfos; // информация о каждой оси
    public float maxMotorTorque; // максимальный крутящий момент
    public float maxSteeringAngle; // максимальный угол поворота колес

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
        }
    }
}
[System.Serializable]
public class AxleInfo
{
    public WheelCollider leftWheel;
    public WheelCollider rightWheel;
    public bool motor; // присоединено ли колесо к мотору?
    public bool steering; // поворачивает ли это колесо?
}
