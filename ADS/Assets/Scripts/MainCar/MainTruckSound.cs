using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainTruckSound : MonoBehaviour
{
    public float minSpeed;
    public float maxSpeed;
    private float currentSpeed;

    private Rigidbody carRb;
    private AudioSource carSound;


    public float minPitch;
    public float maxPitch;
    private float pitchFromCar;



    // Start is called before the first frame update
    void Start()
    {
        carSound = GetComponent<AudioSource>();
        carRb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        EngineSound();
    }

    void EngineSound()
    {
        currentSpeed = carRb.velocity.magnitude;
        pitchFromCar = carRb.velocity.magnitude / 50f;

        if (currentSpeed < minSpeed)
        {
            carSound.pitch = minPitch;
        }

        if (currentSpeed > minSpeed && currentSpeed < maxSpeed)
        {
            carSound.pitch = minPitch + pitchFromCar;
        }

        if (currentSpeed > maxSpeed)
        {
            carSound.pitch = maxPitch;
        }
    }




}
