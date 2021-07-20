using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class PhotonManager : MonoBehaviourPunCallbacks
{
    private readonly string gameVersion = "v1.0";
    private string userId = "Zackiller";

    public TMP_InputField nickName;
    public TMP_InputField roomName;

    // ë£? ëª©ë¡?„ ????¥?•  ?”•?…”?„ˆë¦? ?ë£? 
    private Dictionary<string, GameObject> roomDict = new Dictionary<string, GameObject>();
    // ë£? ?ƒ?„±?„ ?œ„?•œ ?”„ë¦¬íŒ¹
    public GameObject roomPrefab;
    // RoomItem ?”„?˜?•?„ ?ƒ?„±?•  ?˜?–´ë¦°íŠ¸
    public Transform scrollContents;

    void Awake()
    {
        // ë°©ì¥?´ ?˜¸ì¶œí•œ ?”¬?„ ?‹¤ë¥? ?´?¼?´?–¸?Š¸?“¤??? ??™?œ¼ë¡? ?˜¸ì¶?(ë¡œë”©)?‹œ?‚¤?Š” ?˜µ?…˜
        PhotonNetwork.AutomaticallySyncScene = true;

        // ê²Œì„ ë²„ì „ ì§?? •
        PhotonNetwork.GameVersion = gameVersion;

        // ?œ ???ëª? ì§?? •
        PhotonNetwork.NickName = userId;
        //PhotonNetwork.OfflineMode = true;

        // ? ‘?†
        if (PhotonNetwork.IsConnected == false)
            PhotonNetwork.ConnectUsingSettings();
    }

    void Start()
    {
        userId = PlayerPrefs.GetString("USER_ID", $"USER_{Random.Range(0, 100):00}");
        nickName.text = userId;

        // ?œ ???ëª? ì§?? •
        PhotonNetwork.NickName = userId;
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected To Photon Server !!!");
        // ë¡œë¹„?— ? ‘?†
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("ë¡œë¹„?— ?…?¥");

        // PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log($"ë¬´ì‘?œ„ ì¡°ì¸ ?‹¤?Œ¨ {returnCode} : {message}");

        // ë£¸ì— ?†?„±?„ ? •?˜
        RoomOptions ro = new RoomOptions();
        ro.IsOpen = true;
        ro.IsVisible = true;
        ro.MaxPlayers = 100;

        // ë£? ?ƒ?„±
        PhotonNetwork.CreateRoom("MyRoom", ro);
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("ë°©ìƒ?„± ?™„ë£?");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("ë°©ì…?¥ ?™„ë£?");


        // ë°©ì¥?¼ ê²½ìš°?— ? „?ˆ¬ ?”¬?„ ?˜¸ì¶?(ë¡œë”©)
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("BattleField");
        }
    }

    // ë£? ëª©ë¡?´ ë³?ê²½ë˜ë©? ?˜¸ì¶œë˜?Š” ì½œë°±
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        GameObject tempRoom = null;

        foreach (var room in roomList)
        {
            //Debug.Log($"room name ={room.Name} , player ={room.PlayerCount}/{room.MaxPlayers}");

            // ë£¸ì´ ?‚­? œ?œ ê²½ìš° --> ?”•?…”?„ˆë¦? ? œê±?
            if (room.RemovedFromList == true)
            {
                //?”•?…”?„ˆë¦¬ì—?„œ ê²??ƒ‰
                roomDict.TryGetValue(room.Name, out tempRoom);
                // RoomItem ?‚­? œ
                Destroy(tempRoom);
                // ?”•?…”?„ˆë¦¬ì—?„œ ?°?´ë¥? ?‚­? œ
                roomDict.Remove(room.Name);
            }
            else // ë£? ? •ë³´ë?? ê°±ì‹ (ë³?ê²?)
            {
                //  1. ì²˜ìŒ ?ƒ?„±?œ ë£¸ì¼ ê²½ìš° --> ?”•?…”?„ˆë¦¬ì— ì¶”ê??
                if (roomDict.ContainsKey(room.Name) == false)
                {
                    // ë£? ?ƒ?„±
                    GameObject _room = Instantiate(roomPrefab, scrollContents);
                    // ë£? ? •ë³´ë?? ?‘œ?‹œ
                    _room.GetComponent<RoomData>().RoomInfo = room;
                    // ?”•?…”?„ˆë¦¬ì— ì¶”ê??
                    roomDict.Add(room.Name, _room);
                }
                //  2. ê¸°ì¡´?— ?ƒ?„±?œ ë£¸ì¼ ê²½ìš° --> ë£? ? •ë³´ë?? ê°±ì‹ 
                else
                {
                    // ë£? ? •ë³´ë?? ê°±ì‹ 
                    roomDict.TryGetValue(room.Name, out tempRoom);
                    tempRoom.GetComponent<RoomData>().RoomInfo = room;
                }
            }
        }
    }



    #region UI_BUTTON_CALLBACK
    public void OnRandomJoinClick()
    {
        if (string.IsNullOrEmpty(nickName.text))
        {
            userId = $"USER_{Random.Range(0, 100):00}";
            nickName.text = userId;
        }
        PlayerPrefs.SetString("USER_ID", nickName.text);

        PhotonNetwork.NickName = nickName.text;
        PhotonNetwork.JoinRandomRoom();
    }

    public void OnMakeRoomClick()
    {
        // ë£? ?†?„±?„ ? •?˜
        RoomOptions ro = new RoomOptions();
        ro.IsOpen = ro.IsVisible = true; // ?˜¤?”ˆ?—¬ë¶?, ?…¸ì¶œì—¬ë¶?ë¥? ?„¤? •
        ro.MaxPlayers = 100;             // ?™? ‘?‚¬?š©? ?ˆ˜ ?„¤? •

        if (string.IsNullOrEmpty(roomName.text))
        {
            roomName.text = $"ROOM_{Random.Range(0, 100):00}";
        }

        // ë£¸ìƒ?„±
        PhotonNetwork.CreateRoom(roomName.text, ro);
    }
    #endregion



}
