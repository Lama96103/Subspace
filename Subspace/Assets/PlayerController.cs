using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PlayerController : NetworkBehaviour {

    CharacterController character;
    Camera cam;

    [SerializeField]
    float movementSpeed;

    [SerializeField]
    float lookSpeed;

    [SerializeField]
    State state;

    [SerializeField]
    Color nonInteractive;

    [SerializeField]
    Color interactive;

    ShipController currentShip;
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
                CheckInteractive();
                break;
            case State.flying:
                Flying();
                if (Input.GetKeyDown(KeyCode.E))
                    ExitCockpit();
                break;
            default:
                break;
        }

        if (onShip)
        {
            OnShip();
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
            if(hit.collider.transform.parent.tag == "Interactive")
            {
                Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
                Debug.Log("Did Hit");
                dot.color = interactive;
                if (Input.GetKeyDown(KeyCode.E))
                {
                    EnterCockpit(hit.collider.transform.parent.gameObject);
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

    #endregion


    Vector3 lastShipPos;
    void Flying()
    {
        Vector3 translation = Vector3.zero;
        Vector2 rotation = Vector2.zero;

        if (Input.GetKey(KeyCode.W))
        {
            translation.x = -1;
        }else if (Input.GetKey(KeyCode.S))
        {
            translation.x = 1;
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
            translation.z = 1;
        }else if (Input.GetKey(KeyCode.D))
        {
            translation.z = -1;
        }

        rotation.x = Input.GetAxis("Mouse X");
        rotation.y = Input.GetAxis("Mouse Y");

        currentShip.GetInput(translation, rotation);
    }

    void OnShip()
    {
        Vector3 curShipPos = currentShip.transform.position;
        Vector3 moveDir = Vector3.zero;
        moveDir = curShipPos - lastShipPos;

        transform.position += moveDir;

        lastShipPos = curShipPos;
    }

    void EnterCockpit(GameObject ship)
    {
        currentShip = ship.GetComponent<ShipController>();
        if(currentShip.AssignPilot(this) != 0)
        {
            currentShip = null;
            state = State.walking;
        }
        else
        {
            state = State.flying;
            lastShipPos = ship.transform.position;
            onShip = true;
        }
    }

    void ExitCockpit()
    {
        if(currentShip.UnAssignPilot(this) == 0)
        {
            //currentShip = null;
            state = State.walking;
        }
        else
        {
            Debug.LogError("Error exiting Pilot Seat");
            Debug.DebugBreak();
        }

    }

    enum State
    {
        walking, flying
    }
}
