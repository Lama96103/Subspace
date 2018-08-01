using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Lobby : MonoBehaviour {

    public RoomInfo room;

    public event EventHandler<OnJoinLobbyArgs> OnJoinLobby;

    public void JoinLobby()
    {
        if (OnJoinLobby != null)
        {
            OnJoinLobby(this, new OnJoinLobbyArgs { Room = room });
        }
    }
}

public class OnJoinLobbyArgs : EventArgs
{
    public RoomInfo Room { get; set; }
}
