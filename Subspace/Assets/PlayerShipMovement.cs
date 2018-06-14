using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SubSpace.Player.Ship
{
    public class PlayerShipMovement : MonoBehaviour
    {

        public void UpdateMovement(ShipController ship)
        {
            RoatateShip(ship);
            Flying(ship);
        }


        #region Flying

        void Flying(ShipController ship)
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

            rotation.x = Input.GetAxis("Mouse X");
            rotation.y = Input.GetAxis("Mouse Y");

            ship.Cmd_GetInput(translation, rotation);
        }

        void RoatateShip(ShipController ship)
        {
            //print(Input.mousePosition);
            ship.Cmd_GetRotation(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        }

        #endregion
    }
}