using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using Photon.Realtime;

public class GameManager : MonoBehaviourPunCallbacks
{
    void Awake()
    {
        Vector3 pos = new Vector3(Random.Range(-150.0f, 150.0f), 0.0f, Random.Range(-150.0f, 150.0f));
        PhotonNetwork.Instantiate("Tank", pos, Quaternion.identity, 0);
    }

    public void OnExitClick()
    {
        PhotonNetwork.LeaveRoom();
    }

    // Î£∏Ïóê?Ñú Ï¢ÖÎ£å?êú ?õÑ (CleanUP)
    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("Lobby");
    }

    // public override void OnPlayerEnteredRoom(Player newPlayer)
    // {
    //     string msg = $"\n<color=#00ff00>{newPlayer}";
    // }
}
