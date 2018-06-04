using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipController : MonoBehaviour {

    [SerializeField]
    PlayerController currentPilot;

    [SerializeField]
    float speed;

    Rigidbody rig;
	// Use this for initialization
	void Start () {
        currentPilot = null;
        rig = GetComponent<Rigidbody>();
    }
	
	// Update is called once per frame
	void Update () {
        if (currentPilot == null)
            return;


	}

    public void GetInput(Vector3 translation, Vector2 rotation)
    {

        rig.AddForce(transform.forward * translation.x * speed);
        rig.AddForce(transform.right * translation.z * speed);
        rig.AddForce(transform.up * translation.y * speed);
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
