using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SubSpace.Player.Ship
{
    public class PlayerShipMovement : MonoBehaviour
    {

        public void UpdateMovement(SubSpace.Ship.ShipController ship, GameObject pilotSeat)
        {
            RoatateShip(ship);
            Flying(ship);
            SyncPlayerShipLocationRotation(pilotSeat);
        }


        #region Flying

        void Flying(SubSpace.Ship.ShipController ship)
        {
            Vector3 translation = Vector3.zero;
            Vector2 rotation = Vector2.zero;

            if (Input.GetKey(KeyCode.W))
            {
                translation.x = 1;
            }
            else if (Input.GetKey(KeyCode.S))
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
            }
            else if (Input.GetKey(KeyCode.D))
            {
                translation.z = 1;
            }

            if (translation != Vector3.zero)
                ship.GetMovement(translation);
            else
                ship.DampenVelocity();
        }

        void RoatateShip(SubSpace.Ship.ShipController ship)
        {
            //print(Input.mousePosition);
            //ship.GetRotation(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
            
            ship.GetRotation(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        }

        void SyncPlayerShipLocationRotation(GameObject pilotSeat)
        {
            this.transform.rotation = pilotSeat.transform.rotation;
            this.transform.position = pilotSeat.transform.position;
        }

        #endregion
    }
}