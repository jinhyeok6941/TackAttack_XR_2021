using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using TMPro;
using Photon.Realtime;

public class GameManager : MonoBehaviourPunCallbacks
{
    public TMP_Text messageText;
    public TMP_Text connectInfoText;

    public TMP_InputField chatMsg;

    private PhotonView pv;

    void Awake()
    {
        Vector3 pos = new Vector3(Random.Range(-150.0f, 150.0f), 0.0f, Random.Range(-150.0f, 150.0f));
        PhotonNetwork.Instantiate("Tank", pos, Quaternion.identity, 0);
        pv = GetComponent<PhotonView>();
    }

    void Start()
    {
        var roomInfo = PhotonNetwork.CurrentRoom;
        connectInfoText.text = $"({roomInfo.PlayerCount}/{roomInfo.MaxPlayers})";
    }

    public void OnExitClick()
    {
        PhotonNetwork.LeaveRoom();
    }

    public void OnSendClick()
    {
        string _msg = $"\n<color=#00ff00>[{PhotonNetwork.NickName}]</color> {chatMsg.text}";
        pv.RPC("SendChatMessage", RpcTarget.AllBufferedViaServer, _msg);
    }

    [PunRPC]
    void SendChatMessage(string msg)
    {
        messageText.text += msg;
    }

    // 룸에서 종료된 후 (CleanUP)
    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("Lobby");
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        string msg = $"\n<color=#00ff00>{newPlayer.NickName}</color> is joined room";
        messageText.text += msg;

        var roomInfo = PhotonNetwork.CurrentRoom;
        connectInfoText.text = $"({roomInfo.PlayerCount}/{roomInfo.MaxPlayers})";
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        string msg = $"\n<color=#ff0000>{otherPlayer.NickName}</color> is left room";
        messageText.text += msg;

        var roomInfo = PhotonNetwork.CurrentRoom;
        connectInfoText.text = $"({roomInfo.PlayerCount}/{roomInfo.MaxPlayers})";
    }

}
