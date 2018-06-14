using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SubSpace.Networking
{
    public class PlayerNameInputField : MonoBehaviour
    {
        InputField inputField;

        static string playerNamePrefKey = "PlayerName";

        // Use this for initialization
        private void Start()
        {
            string defaultName = "";
            inputField = this.GetComponent<InputField>();

            if (inputField != null)
            {
                if (PlayerPrefs.HasKey(playerNamePrefKey))
                {
                    defaultName = PlayerPrefs.GetString(playerNamePrefKey);
                    inputField.text = defaultName;
                }
            }

            PhotonNetwork.playerName = defaultName;
        }

        public void SetPlayerName()
        {

         
            PhotonNetwork.playerName = inputField.text + " "; // force a trailing space string in case value is an empty string, else playerName would not be updated.


            PlayerPrefs.SetString(playerNamePrefKey, inputField.text);
        }
    }
}