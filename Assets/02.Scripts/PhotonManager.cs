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

    // �? 목록?�� ????��?�� ?��?��?���? ?���? 
    private Dictionary<string, GameObject> roomDict = new Dictionary<string, GameObject>();
    // �? ?��?��?�� ?��?�� ?��리팹
    public GameObject roomPrefab;
    // RoomItem ?��?��?��?�� ?��?��?�� ?��?��린트
    public Transform scrollContents;

    void Awake()
    {
        // 방장?�� ?��출한 ?��?�� ?���? ?��?��?��?��?��?��??? ?��?��?���? ?���?(로딩)?��?��?�� ?��?��
        PhotonNetwork.AutomaticallySyncScene = true;

        // 게임 버전 �??��
        PhotonNetwork.GameVersion = gameVersion;

        // ?��???�? �??��
        PhotonNetwork.NickName = userId;
        //PhotonNetwork.OfflineMode = true;

        // ?��?��
        if (PhotonNetwork.IsConnected == false)
            PhotonNetwork.ConnectUsingSettings();
    }

    void Start()
    {
        userId = PlayerPrefs.GetString("USER_ID", $"USER_{Random.Range(0, 100):00}");
        nickName.text = userId;

        // ?��???�? �??��
        PhotonNetwork.NickName = userId;
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected To Photon Server !!!");
        // 로비?�� ?��?��
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("로비?�� ?��?��");

        // PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log($"무작?�� 조인 ?��?�� {returnCode} : {message}");

        // 룸에 ?��?��?�� ?��?��
        RoomOptions ro = new RoomOptions();
        ro.IsOpen = true;
        ro.IsVisible = true;
        ro.MaxPlayers = 100;

        // �? ?��?��
        PhotonNetwork.CreateRoom("MyRoom", ro);
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("방생?�� ?���?");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("방입?�� ?���?");


        // 방장?�� 경우?�� ?��?�� ?��?�� ?���?(로딩)
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("BattleField");
        }
    }

    // �? 목록?�� �?경되�? ?��출되?�� 콜백
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        GameObject tempRoom = null;

        foreach (var room in roomList)
        {
            //Debug.Log($"room name ={room.Name} , player ={room.PlayerCount}/{room.MaxPlayers}");

            // 룸이 ?��?��?�� 경우 --> ?��?��?���? ?���?
            if (room.RemovedFromList == true)
            {
                //?��?��?��리에?�� �??��
                roomDict.TryGetValue(room.Name, out tempRoom);
                // RoomItem ?��?��
                Destroy(tempRoom);
                // ?��?��?��리에?�� ?��?���? ?��?��
                roomDict.Remove(room.Name);
            }
            else // �? ?��보�?? 갱신(�?�?)
            {
                //  1. 처음 ?��?��?�� 룸일 경우 --> ?��?��?��리에 추�??
                if (roomDict.ContainsKey(room.Name) == false)
                {
                    // �? ?��?��
                    GameObject _room = Instantiate(roomPrefab, scrollContents);
                    // �? ?��보�?? ?��?��
                    _room.GetComponent<RoomData>().RoomInfo = room;
                    // ?��?��?��리에 추�??
                    roomDict.Add(room.Name, _room);
                }
                //  2. 기존?�� ?��?��?�� 룸일 경우 --> �? ?��보�?? 갱신
                else
                {
                    // �? ?��보�?? 갱신
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
        // �? ?��?��?�� ?��?��
        RoomOptions ro = new RoomOptions();
        ro.IsOpen = ro.IsVisible = true; // ?��?��?���?, ?��출여�?�? ?��?��
        ro.MaxPlayers = 100;             // ?��?��?��?��?�� ?�� ?��?��

        if (string.IsNullOrEmpty(roomName.text))
        {
            roomName.text = $"ROOM_{Random.Range(0, 100):00}";
        }

        // 룸생?��
        PhotonNetwork.CreateRoom(roomName.text, ro);
    }
    #endregion



}
