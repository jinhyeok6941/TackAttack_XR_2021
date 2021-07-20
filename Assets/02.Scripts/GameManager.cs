using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

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

    // 룸에서 종료된 후 (CleanUP)
    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("Lobby");
    }
}
