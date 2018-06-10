using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PlayerController : NetworkBehaviour {

    CharacterController character;
    Camera cam;

    [SerializeField]
    private float movementSpeed;

    [SerializeField]
    private float lookSpeed;

    [SerializeField]
    State state;

    [SerializeField]
    Color nonInteractive;

    [SerializeField]
    Color interactive;

    ShipController currentShip;
    [SerializeField]
    bool onShip = false;

    Image dot;

    // Use this for initialization
    void Start () {
        character = GetComponent<CharacterController>();
        cam = GetComponentInChildren<Camera>();
        state = State.walking;
        dot = GetComponentInChildren<Image>();
        dot.color = nonInteractive;
        currentShip = null;
    }
	
	// Update is called once per frame
	void Update () {
        if (!isLocalPlayer)
            return;

        switch (state)
        {
            case State.walking:
                Walking();
                Rotation();
                //CheckInteractive();
                break;
            case State.flying:
                Flying();
                RoatateShip();
                if (Input.GetKeyDown(KeyCode.E))
                    ExitPilot();
                break;
            default:
                break;
        }

        if (onShip)
        {
            SyncPlayerRotationWithShip();
            SyncPlayerLocationWithShip();
        }
    }

    #region Walking

    void Rotation()
    {
        float h = lookSpeed * Input.GetAxis("Mouse X");
        float v = lookSpeed * Input.GetAxis("Mouse Y");

        transform.Rotate(0, h, 0);
        cam.transform.Rotate(-v, 0, 0);
    }

    void Walking()
    {
        if (Input.GetKey(KeyCode.W))
        {
            character.SimpleMove(transform.forward * movementSpeed );
        }

        if (Input.GetKey(KeyCode.S))
        {
            character.SimpleMove(-transform.forward * movementSpeed );
        }

        if (Input.GetKey(KeyCode.A))
        {
            character.SimpleMove(-transform.right * movementSpeed );
        }

        if (Input.GetKey(KeyCode.D))
        {
            character.SimpleMove(transform.right * movementSpeed );
        }
    }

    void CheckInteractive()
    {
        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, 2f))
        {
            if (hit.collider.tag == "Interactive")
            {
                dot.color = interactive;
                if (Input.GetKeyDown(KeyCode.E))
                {
                    EnterPilot(hit.collider.transform.parent.gameObject);
                }
            }
            else
            {
                dot.color = nonInteractive;
            }
            
        }
        else
        {
            dot.color = nonInteractive;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Ship")
        {
            currentShip = other.GetComponent<ShipController>();
            onShip = true;

            lastShipPos = currentShip.transform.position;
            lastYRotation = currentShip.transform.rotation.eulerAngles.y;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Ship")
        {
            onShip = false;
        }
    }
    #endregion

    #region Flying

    void Flying()
    {
        Vector3 translation = Vector3.zero;
        Vector2 rotation = Vector2.zero;

        if (Input.GetKey(KeyCode.W))
        {
            translation.x = 1;
        }else if (Input.GetKey(KeyCode.S))
        {
            translation.x = -1;
        }

        if (Input.GetKey(KeyCode.Space))
        {
            translation.y = 1;
        }
        else if (Input.GetKey(KeyCode.LeftControl))
        {
            translation.y = -1;
        }

        if (Input.GetKey(KeyCode.A))
        {
            translation.z = -1;
        }else if (Input.GetKey(KeyCode.D))
        {
            translation.z = 1;
        }

        rotation.x = Input.GetAxis("Mouse X");
        rotation.y = Input.GetAxis("Mouse Y");

        currentShip.Cmd_GetInput(translation, rotation);
    }

    void RoatateShip()
    {
        print(Input.mousePosition);
        currentShip.Cmd_GetRotation(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
    }

    float lastYRotation;
    void SyncPlayerRotationWithShip()
    {
        float curYRotation = currentShip.transform.rotation.eulerAngles.y;
        float yAngle = currentShip.transform.rotation.eulerAngles.y - lastYRotation;
        transform.RotateAround(currentShip.transform.position, Vector3.up, yAngle);

        lastYRotation = curYRotation;
    }

    private Vector3 lastShipPos;
    void SyncPlayerLocationWithShip()
    {
        Vector3 curShipPos = currentShip.transform.position;
        Vector3 moveDir = Vector3.zero;
        moveDir = curShipPos - lastShipPos;

        transform.position += moveDir;

        lastShipPos = curShipPos;
    }

    #endregion
    

    void EnterPilot(GameObject ship)
    {
        currentShip = ship.GetComponent<ShipController>();
        if(currentShip.AssignPilot(this) != 0)
        {
            state = State.walking;
            Debug.LogError("Couldn't Enter Ship");
        }
        else
        {
            Cmd_Assignauthority(ship);
            state = State.flying;
            Quaternion rotation = new Quaternion(0, 0, 0, 0);
            cam.transform.rotation = rotation;
        }
    }

    [Command]
    void Cmd_Assignauthority(GameObject ship)
    {
        bool success = ship.GetComponent<NetworkIdentity>().AssignClientAuthority(GetComponent<NetworkIdentity>().connectionToClient);
        if (!success)
        {
            Debug.LogError("Coundn't Assign Authority");
        }
    }

    [Command]
    void Cmd_UnAssignauthority()
    {
        bool success = currentShip.GetComponent<NetworkIdentity>().RemoveClientAuthority(GetComponent<NetworkIdentity>().connectionToClient);
        if (!success)
        {
            Debug.LogError("Coundn't Remove Authority");
        }
    }

    void ExitPilot()
    {
        if(currentShip.UnAssignPilot(this) == 0)
        {
            state = State.walking;
            Cmd_UnAssignauthority();
        }
        else
        {
            Debug.LogError("Error exiting Pilot Seat");
            Debug.DebugBreak();
        }

    }

    enum State
    {
        walking, flying, gunning, interact, engineering
    }
}
