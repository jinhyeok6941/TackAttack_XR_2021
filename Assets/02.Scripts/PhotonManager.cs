using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PhotonManager : MonoBehaviourPunCallbacks
{
    private readonly string gameVersion = "v1.0";
    private string userId = "Zackiller";

    void Awake()
    {
        // 방장이 호출한 씬을 다른 클라이언트들은 자동으로 호출(로딩)시키는 옵션
        PhotonNetwork.AutomaticallySyncScene = true;

        // 게임 버전 지정
        PhotonNetwork.GameVersion = gameVersion;
        // 유저명 지정
        PhotonNetwork.NickName = userId;

        // 접속
        PhotonNetwork.ConnectUsingSettings();
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

        PhotonNetwork.JoinRandomRoom();
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
        Vector3 pos = new Vector3(Random.Range(-50.0f, 50.0f), 0.0f, Random.Range(-50.0f, 50.0f));
        PhotonNetwork.Instantiate("Tank", pos, Quaternion.identity, 0);
    }


}
