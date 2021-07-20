using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class RoomData : MonoBehaviour
{
    private TMP_Text roomInfoText;
    private RoomInfo _roomInfo;

    public RoomInfo RoomInfo
    {
        get
        {
            return _roomInfo;
        }
        set
        {
            _roomInfo = value;
            roomInfoText.text = $"{_roomInfo.Name} ({_roomInfo.PlayerCount}/{_roomInfo.MaxPlayers})";
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        roomInfoText = GetComponentInChildren<TMP_Text>();
    }
}
