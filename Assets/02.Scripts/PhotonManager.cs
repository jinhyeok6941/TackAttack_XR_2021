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

    // λ£? λͺ©λ‘? ????₯?  ???λ¦? ?λ£? 
    private Dictionary<string, GameObject> roomDict = new Dictionary<string, GameObject>();
    // λ£? ??±? ?? ?λ¦¬νΉ
    public GameObject roomPrefab;
    // RoomItem ???? ??±?  ??΄λ¦°νΈ
    public Transform scrollContents;

    void Awake()
    {
        // λ°©μ₯?΄ ?ΈμΆν ?¬? ?€λ₯? ?΄?Ό?΄?Έ?Έ?€??? ???Όλ‘? ?ΈμΆ?(λ‘λ©)??€? ?΅?
        PhotonNetwork.AutomaticallySyncScene = true;

        // κ²μ λ²μ  μ§?? 
        PhotonNetwork.GameVersion = gameVersion;

        // ? ???λͺ? μ§?? 
        PhotonNetwork.NickName = userId;
        //PhotonNetwork.OfflineMode = true;

        // ? ?
        if (PhotonNetwork.IsConnected == false)
            PhotonNetwork.ConnectUsingSettings();
    }

    void Start()
    {
        userId = PlayerPrefs.GetString("USER_ID", $"USER_{Random.Range(0, 100):00}");
        nickName.text = userId;

        // ? ???λͺ? μ§?? 
        PhotonNetwork.NickName = userId;
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected To Photon Server !!!");
        // λ‘λΉ? ? ?
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("λ‘λΉ? ??₯");

        // PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log($"λ¬΄μ? μ‘°μΈ ?€?¨ {returnCode} : {message}");

        // λ£Έμ ??±? ? ?
        RoomOptions ro = new RoomOptions();
        ro.IsOpen = true;
        ro.IsVisible = true;
        ro.MaxPlayers = 100;

        // λ£? ??±
        PhotonNetwork.CreateRoom("MyRoom", ro);
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("λ°©μ?± ?λ£?");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("λ°©μ?₯ ?λ£?");


        // λ°©μ₯?Ό κ²½μ°? ? ?¬ ?¬? ?ΈμΆ?(λ‘λ©)
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("BattleField");
        }
    }

    // λ£? λͺ©λ‘?΄ λ³?κ²½λλ©? ?ΈμΆλ? μ½λ°±
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        GameObject tempRoom = null;

        foreach (var room in roomList)
        {
            //Debug.Log($"room name ={room.Name} , player ={room.PlayerCount}/{room.MaxPlayers}");

            // λ£Έμ΄ ?­? ? κ²½μ° --> ???λ¦? ? κ±?
            if (room.RemovedFromList == true)
            {
                //???λ¦¬μ? κ²??
                roomDict.TryGetValue(room.Name, out tempRoom);
                // RoomItem ?­? 
                Destroy(tempRoom);
                // ???λ¦¬μ? ?°?΄λ₯? ?­? 
                roomDict.Remove(room.Name);
            }
            else // λ£? ? λ³΄λ?? κ°±μ (λ³?κ²?)
            {
                //  1. μ²μ ??±? λ£ΈμΌ κ²½μ° --> ???λ¦¬μ μΆκ??
                if (roomDict.ContainsKey(room.Name) == false)
                {
                    // λ£? ??±
                    GameObject _room = Instantiate(roomPrefab, scrollContents);
                    // λ£? ? λ³΄λ?? ??
                    _room.GetComponent<RoomData>().RoomInfo = room;
                    // ???λ¦¬μ μΆκ??
                    roomDict.Add(room.Name, _room);
                }
                //  2. κΈ°μ‘΄? ??±? λ£ΈμΌ κ²½μ° --> λ£? ? λ³΄λ?? κ°±μ 
                else
                {
                    // λ£? ? λ³΄λ?? κ°±μ 
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
        // λ£? ??±? ? ?
        RoomOptions ro = new RoomOptions();
        ro.IsOpen = ro.IsVisible = true; // ?€??¬λΆ?, ?ΈμΆμ¬λΆ?λ₯? ?€? 
        ro.MaxPlayers = 100;             // ?? ?¬?©? ? ?€? 

        if (string.IsNullOrEmpty(roomName.text))
        {
            roomName.text = $"ROOM_{Random.Range(0, 100):00}";
        }

        // λ£Έμ?±
        PhotonNetwork.CreateRoom(roomName.text, ro);
    }
    #endregion



}
