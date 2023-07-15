using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class networkMaker : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    int maxPlayer;
    void Start()
    {
        Screen.SetResolution(1920, 1080, false);
        PhotonNetwork.ConnectUsingSettings();
        maxPlayer = 4;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void setMaxPlayer(int num)
    {
        maxPlayer = num;
    }

    public override void OnConnectedToMaster()
    {
    }
}
