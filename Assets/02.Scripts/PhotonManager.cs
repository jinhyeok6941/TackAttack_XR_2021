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

    void Awake()
    {
        // 방장이 호출한 씬을 다른 클라이언트들은 자동으로 호출(로딩)시키는 옵션
        PhotonNetwork.AutomaticallySyncScene = true;

        // 게임 버전 지정
        PhotonNetwork.GameVersion = gameVersion;

        //PhotonNetwork.OfflineMode = true;

        // 접속
        PhotonNetwork.ConnectUsingSettings();
    }

    void Start()
    {
        userId = PlayerPrefs.GetString("USER_ID", $"USER_{Random.Range(0, 100):00}");
        nickName.text = userId;

        // 유저명 지정
        PhotonNetwork.NickName = userId;
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected To Photon Server !!!");
        // 로비에 접속
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("로비에 입장");

        // PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log($"무작위 조인 실패 {returnCode} : {message}");

        // 룸에 속성을 정의
        RoomOptions ro = new RoomOptions();
        ro.IsOpen = true;
        ro.IsVisible = true;
        ro.MaxPlayers = 100;

        // 룸 생성
        PhotonNetwork.CreateRoom("MyRoom", ro);
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("방생성 완료");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("방입장 완료");


        // 방장일 경우에 전투 씬을 호출(로딩)
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("BattleField");
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
        // 룸 속성을 정의
        RoomOptions ro = new RoomOptions();
        ro.IsOpen = ro.IsVisible = true; // 오픈여부, 노출여부를 설정
        ro.MaxPlayers = 100;             // 동접사용자 수 설정

        if (string.IsNullOrEmpty(roomName.text))
        {
            roomName.text = $"ROOM_{Random.Range(0, 100):00}";
        }

        // 룸생성
        PhotonNetwork.CreateRoom(roomName.text, ro);
    }
    #endregion



}
