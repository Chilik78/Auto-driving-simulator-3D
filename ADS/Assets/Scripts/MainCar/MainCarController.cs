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
    public List<AxleInfo> axleInfos; // Оси колес
    public float maxMotorTorque; // Максимальный крутящий момент двигателя
    public float maxSteeringAngle; // Максимальный угол поворота колес
    public KeyCode headlights; // Кнопка, по которой будут включаться и выключаться фары
    public KeyCode leftTurnSignal;// Кнопка, по которой будет включаться и выключаться левый поворотник
    public KeyCode rightTurnSignal;// Кнопка, по которой будет включаться и выключаться правый поворотник

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

        if (isLightTurnSignal)
        {
            switchTurnSignals();
        }
    }

    private bool isLightOn = false;
    private bool isLightTurnSignal = false;
    private bool isLeftTurnSignal = false;
    public void Update()
    {
        if (Input.GetKeyDown(headlights))
        {
            OnLights();
        }

        if((Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) && (isLightOn))
        {
            brightnessUpBackLights();
        }
        else
        {
            brightnessDownBackLights();
        }

        if(Input.GetKeyDown(leftTurnSignal) && !isLightTurnSignal)
        {
            isLeftTurnSignal = true;
            switchTurnSignals();
            isLightTurnSignal = true;
        }
        else if (Input.GetKeyDown(rightTurnSignal) && !isLightTurnSignal)
        {
            isLeftTurnSignal = false;
            switchTurnSignals();
            isLightTurnSignal = true;
        }
        else if (Input.GetKeyDown(leftTurnSignal) && isLightTurnSignal)
        {
            isLeftTurnSignal = true;
            switchTurnSignals(true);
            isLightTurnSignal = false;
        }
        else if (Input.GetKeyDown(rightTurnSignal) && isLightTurnSignal)
        {
            isLeftTurnSignal = false;
            switchTurnSignals(true);
            isLightTurnSignal = false;
        }
    }

    public void OnLights()// Включение и выключение фар
    {
        
        GameObject car = gameObject;

        //Меняем значение задних фар на противоположное
        Light currentLight = car.transform.GetChild(2).transform.GetChild(0).GetComponent<Light>();
        currentLight.enabled = !currentLight.enabled;
        currentLight = car.transform.GetChild(2).transform.GetChild(1).GetComponent<Light>();
        currentLight.enabled = !currentLight.enabled;

        //Меняем значение передних фар на противоположное
        currentLight = car.transform.GetChild(5).transform.GetChild(0).GetComponent<Light>();
        currentLight.enabled = !currentLight.enabled;
        currentLight = car.transform.GetChild(5).transform.GetChild(1).GetComponent<Light>();
        currentLight.enabled = !currentLight.enabled;

        isLightOn = !isLightOn;
    }

    
    public void brightnessUpBackLights()// Повышение яркости задних фар
    {
        GameObject car = gameObject;

        Light firstLight = car.transform.GetChild(2).transform.GetChild(0).GetComponent<Light>();
        Light secondLight = car.transform.GetChild(2).transform.GetChild(1).GetComponent<Light>();

        if(firstLight.intensity < 30 && secondLight.intensity < 30)
        {
            firstLight.intensity += 15;
            secondLight.intensity += 15;
        }
    }

    public void brightnessDownBackLights()// Понижение яркости задних фар
    {
        GameObject car = gameObject;

        Light firstLight = car.transform.GetChild(2).transform.GetChild(0).GetComponent<Light>();
        Light secondLight = car.transform.GetChild(2).transform.GetChild(1).GetComponent<Light>();

        if (firstLight.intensity > 10 && secondLight.intensity > 10)
        {
            firstLight.intensity -= 15;
            secondLight.intensity -= 15;
        }
    }

    public void switchTurnSignals(bool isStopSignal = false)
    {
        //Debug.Log(isLeftTurnSignal + " | " + isLeftTurnSignal);

        GameObject car = gameObject;

        if(isLeftTurnSignal && isLeftTurnSignal && isStopSignal)
        {
            Light currentTurnSignal = car.transform.GetChild(3).transform.GetChild(1).GetComponent<Light>();
            currentTurnSignal.enabled = false;
        }
        else if(!isLeftTurnSignal && isLeftTurnSignal && isStopSignal)
        {
            Light currentTurnSignal = car.transform.GetChild(3).transform.GetChild(0).GetComponent<Light>();
            currentTurnSignal.enabled = false;
        }

        if (isLeftTurnSignal && !isLightTurnSignal)
        {
            Light currentTurnSignal = car.transform.GetChild(3).transform.GetChild(1).GetComponent<Light>();
            currentTurnSignal.enabled = !currentTurnSignal.enabled;
            Debug.Log("True");
        }
        else if (!isLeftTurnSignal && !isLightTurnSignal)
        {
            Light currentTurnSignal = car.transform.GetChild(3).transform.GetChild(0).GetComponent<Light>();
            currentTurnSignal.enabled = !currentTurnSignal.enabled;
        }
        else if (isLeftTurnSignal && isLeftTurnSignal)
        {
            Light currentTurnSignal = car.transform.GetChild(3).transform.GetChild(1).GetComponent<Light>();
            currentTurnSignal.enabled = !currentTurnSignal.enabled;
        }
        else if (!isLeftTurnSignal && isLightTurnSignal)
        {
            Light currentTurnSignal = car.transform.GetChild(3).transform.GetChild(0).GetComponent<Light>();
            currentTurnSignal.enabled = !currentTurnSignal.enabled;
        }
    }
}