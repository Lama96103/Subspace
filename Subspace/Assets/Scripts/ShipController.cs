﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ShipController : Photon.PunBehaviour
{
    
    PlayerController currentPilot;

    [SerializeField]
    float movemnetSpeed;

    [SerializeField]
    float rotationSpeed;

    Rigidbody rig;
	// Use this for initialization
	void Start () {
        currentPilot = null;
        rig = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (currentPilot == null)
            return;


    }

    public void GetMovement(Vector3 translation, Vector2 rotation)
    {
        rig.AddForce(transform.forward * translation.x * movemnetSpeed);
        rig.AddForce(transform.right * translation.z * movemnetSpeed);
        rig.AddForce(transform.up * translation.y * movemnetSpeed);
    }
    

    public void GetRotation(float h, float v)
    {
        h *= rotationSpeed;
        v *= rotationSpeed;

        Vector3 eulerAngleVelocity = new Vector3(0, h * Time.deltaTime, 0);
        transform.Rotate(eulerAngleVelocity);


       // rig.AddTorque(eulerAngleVelocity);
    }

    public int AssignPilot(PlayerController player)
    {
        if (currentPilot == null)
        {
            currentPilot = player;
            return 0;
        }

        return -1;
    }

    public int UnAssignPilot(PlayerController player)
    {
        if (currentPilot == player)
        {
            currentPilot = null;
            return 0;
        }

        return -1;
    }


}
