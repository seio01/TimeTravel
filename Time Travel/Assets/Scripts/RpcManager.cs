using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class RpcManager : MonoBehaviour
{
    public static RpcManager instance;
    public TMP_Text testTMP;
    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void updatePlayerBoardNum(playerInformationUI obj, int playerPosition)
    {
        testTMP.text = "RPC check";
        obj.playerPositionText.text = (playerPosition).ToString();
    }
}
