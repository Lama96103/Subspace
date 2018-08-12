using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace SubSpace.Ship
{
    public class ShipController : Photon.PunBehaviour
    {

        SubSpace.Player.PlayerManager currentPilot;

        #pragma warning disable IDE0044 // Add readonly modifier
        [SerializeField]
        private float movemnetSpeed;


        [SerializeField]
        private float rotationSpeed;

        [SerializeField]
        private float dampening;
        #pragma warning restore IDE0044 // Add readonly modifier    
        Rigidbody rig;

        Animator anim;
        // Use this for initialization
        void Start()
        {
            currentPilot = null;
            rig = GetComponent<Rigidbody>();
            anim = GetComponent<Animator>();
        }

        // Update is called once per frame
        void Update()
        {
            if (currentPilot == null)
            {
                DampenVelocity();
                return;
            }


        }

        public void GetMovement(Vector3 translation)
        {
            rig.AddForce(transform.forward * translation.x * movemnetSpeed);
            rig.AddForce(transform.right * translation.z * movemnetSpeed);
            rig.AddForce(transform.up * translation.y * movemnetSpeed);
        }

        public void DampenVelocity()
        {
            if(rig.velocity.x > 0)
            {
                rig.AddForce(-Vector3.right * dampening);
            }else if(rig.velocity.x < 0)
            {
                rig.AddForce(Vector3.right * dampening);
            }

            if (rig.velocity.y > 0)
            {
                rig.AddForce(-Vector3.up * dampening);
            }
            else if (rig.velocity.y < 0)
            {
                rig.AddForce(Vector3.up * dampening);
            }

            if (rig.velocity.z > 0)
            {
                rig.AddForce(-Vector3.forward * dampening);
            }
            else if (rig.velocity.z < 0)
            {
                rig.AddForce(Vector3.forward * dampening);
            }
        }

        public void GetRotation(float h, float v)
        {
            h *= rotationSpeed;
            v *= rotationSpeed;

            Vector3 eulerAngleVelocity = new Vector3(0, h * Time.deltaTime, 0);
            transform.Rotate(eulerAngleVelocity);
        }

        public int AssignPilot(SubSpace.Player.PlayerManager player)
        {
            if (currentPilot == null)
            {
                currentPilot = player;
                return 0;
            }

            return -1;
        }

        public int UnAssignPilot(SubSpace.Player.PlayerManager player)
        {
            if (currentPilot == player)
            {
                currentPilot = null;
                return 0;
            }

            return -1;
        }

        [PunRPC]
        public void ActivateDoor(bool open)
        {
            if (open)
            {
                anim.ResetTrigger("OnDoorClose");
                anim.SetTrigger("OnDoorOpen");
            }
            else
            {
                anim.ResetTrigger("OnDoorOpen");
                anim.SetTrigger("OnDoorClose");
            }
        }
    }
}
