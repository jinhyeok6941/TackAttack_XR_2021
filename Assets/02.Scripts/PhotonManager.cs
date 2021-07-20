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

    // 룸 목록을 저장할 딕셔너리 자료 
    private Dictionary<string, GameObject> roomDict = new Dictionary<string, GameObject>();
    // 룸 생성을 위한 프리팹
    public GameObject roomPrefab;
    // RoomItem 프래핍을 생성할 페어린트
    public Transform scrollContents;

    void Awake()
    {
        // 방장이 호출한 씬을 다른 클라이언트들은 자동으로 호출(로딩)시키는 옵션
        PhotonNetwork.AutomaticallySyncScene = true;

        // 게임 버전 지정
        PhotonNetwork.GameVersion = gameVersion;

        // 유저명 지정
        PhotonNetwork.NickName = userId;
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

    // 룸 목록이 변경되면 호출되는 콜백
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        GameObject tempRoom = null;

        foreach (var room in roomList)
        {
            //Debug.Log($"room name ={room.Name} , player ={room.PlayerCount}/{room.MaxPlayers}");

            // 룸이 삭제된 경우 --> 딕셔너리 제거
            if (room.RemovedFromList == true)
            {
                //딕셔너리에서 검색
                roomDict.TryGetValue(room.Name, out tempRoom);
                // RoomItem 삭제
                Destroy(tempRoom);
                // 딕셔너리에서 데이를 삭제
                roomDict.Remove(room.Name);
            }
            else // 룸 정보를 갱신(변경)
            {
                //  1. 처음 생성된 룸일 경우 --> 딕셔너리에 추가
                if (roomDict.ContainsKey(room.Name) == false)
                {
                    // 룸 생성
                    GameObject _room = Instantiate(roomPrefab, scrollContents);
                    // 룸 정보를 표시
                    _room.GetComponent<RoomData>().RoomInfo = room;
                    // 딕셔너리에 추가
                    roomDict.Add(room.Name, _room);
                }
                //  2. 기존에 생성된 룸일 경우 --> 룸 정보를 갱신
                else
                {
                    // 룸 정보를 갱신
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
