using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class JoinAndConnect : MonoBehaviourPunCallbacks
{
    public TextMeshProUGUI joinField;
    public TextMeshProUGUI createField;

    // Start is called before the first frame update
    private void Start()
    {
    }

    public void CreateRoom()
    {
        PhotonNetwork.CreateRoom(createField.text);
    }

    public void JoinRoom()
    {
        PhotonNetwork.JoinRoom(joinField.text);
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("GameScene");
    }
}