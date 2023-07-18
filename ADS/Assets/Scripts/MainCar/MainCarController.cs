using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Experimental.GlobalIllumination;

[System.Serializable]
public class AxleInfo
{
    public WheelCollider leftWheel;
    public WheelCollider rightWheel;
    public bool motor;
    public bool steering;
}

public class MainCarController : MonoBehaviour
{
    public List<AxleInfo> axleInfos; // ќси колес
    public float maxMotorTorque; // ћаксимальный крут€щий момент двигател€
    public float maxSteeringAngle; // ћаксимальный угол поворота колес

    // находит визуальную часть колес
    // устанавливает новые координаты
    public void ApplyLocalPositionToVisuals(WheelCollider collider)
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

            ApplyLocalPositionToVisuals(axleInfo.leftWheel);
            ApplyLocalPositionToVisuals(axleInfo.rightWheel);     
        }
    }

    private static float speed;
    private bool isPositive; 
    private void Update()
    {
        speed = gameObject.GetComponent<Rigidbody>().velocity.magnitude;

        if((Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) && speed <= 0.2)
        {
            isPositive = true;
        }
        else if ((Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) && speed <= 0.2)
        {
            isPositive = false;
        }

        speed = isPositive ? speed : speed * -1;
    }

    public static float GetSpeed()
    {
        return speed;
    }
}