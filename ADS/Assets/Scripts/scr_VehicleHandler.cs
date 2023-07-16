using UnityEngine;

public class scr_VehicleHandler : MonoBehaviour
{
    [Header("Колёса")]
    [SerializeField] private GameObject[] Wheels = new GameObject[4];
    private float lastRot;
    private float Rotation;

    [Header("Машина")]
    public Transform target;

    [Header("Параметры управления")]
    public float steer_rat = 14.4f;    //steering ratio

    [SerializeField] private float speedmultiplier = 0.1f; 
    public float steer_correction = 0; //Соответствие между текущим положением рулевого колеса и нулевым углом в градусах
    private float hdgdif;
    private float lasthdg;
    float steerangle;
    [SerializeField] private float steeringDamping = 4;
    private float SA_last;
    private float SA_wanted;
    private float SA_calc;


    [Header("BrakeLight Objects")]
    [SerializeField] private GameObject[] BrakeLight = new GameObject[2];

    private void Start()
    {
        SA_last = 0;
        SA_wanted = 0;
        lasthdg = 0;

        Rotation = 0;
        lastRot = 0;
    }

    public void CalculateSteering(float heading, float speed, float timer)
    {
        hdgdif = heading - lasthdg; //Разница между текущим heading и взвешенными последними 3 heading
        lasthdg = heading;
        SA_wanted =  hdgdif * steer_rat;
        SA_calc = Mathf.LerpAngle(SA_last, SA_wanted, steeringDamping * Time.deltaTime);
        SA_last = SA_calc;
        WheelHandler(SA_calc, heading, speed, timer);
    }

    public void WheelHandler(float SteerAngle, float heading, float speed, float timer) //Другие автомобили
    {
        Rotation = lastRot + speed*(speedmultiplier * timer * (1/3.6f)); //Вычисление вращения колес на основе скорости и времени
        if (Rotation > 360)
        {
            Rotation = Rotation - 360;
        }
        else if (Rotation < 0)
        {
            Rotation = Rotation + 360;
        }
        Wheels[0].transform.rotation = Quaternion.Euler((Rotation), (SteerAngle) + heading, 0f);
        Wheels[1].transform.rotation = Quaternion.Euler((Rotation), (SteerAngle) + heading, 0f);
        Wheels[2].transform.rotation = Quaternion.Euler((Rotation), heading, 0f);
        Wheels[3].transform.rotation = Quaternion.Euler((Rotation), heading, 0f);
        lastRot = Rotation;
    }

    public void BrakeLightSwitch(bool state)
    {
        for (int i = 0; i < 2; i++)
        {
            Light light = (Light)BrakeLight[i].GetComponent("Light");    //Поворотники
            if (state)
            {
                if (light.intensity < 20)
                    light.intensity += 5;
            }
            else
            {
                if (light.intensity > 0)
                    light.intensity -= 1;
            }
        }        
    }
}
