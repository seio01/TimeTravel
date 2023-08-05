using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using TMPro;

public class DontDestroyObjects : MonoBehaviour
{
    public static DontDestroyObjects instance;
    public RoomManager manager;
    public List<Photon.Realtime.Player> playerListWithOrder;

    public enum items { hint, erase, pass, cardSteal, timeSteal, bind };
    public List<items>[] playerItems; //플레이어 전체 아이템 보유 리스트 배열

    public TMP_Text test;
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        if (instance == null)
        {
            instance = this;
        }
    }
        // Start is called before the first frame update
    void Start()
    {
        playerListWithOrder = new List<Photon.Realtime.Player>();
        playerItems = new List<items>[PhotonNetwork.CurrentRoom.MaxPlayers];
        for (int i=0;i< PhotonNetwork.CurrentRoom.MaxPlayers;i++ )
        {
            playerItems[i] = new List<items>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setPlayerListWithOrder(List<int> updatedPlayerList)
    {
        for (int i = 0; i < updatedPlayerList.Count; i++)
        {
            playerListWithOrder.Add(PhotonNetwork.PlayerList[updatedPlayerList[i]]);
        }
    }

    public void fillItemList(int playerOrderIndex, int[] updatedList)
    {

        for (int i = 0; i < updatedList.Length; i++)
        {
            if (updatedList[i] == 0)
            {
                playerItems[playerOrderIndex].Add(items.hint);
            }
            else if (updatedList[i] == 1)
            {
                playerItems[playerOrderIndex].Add(items.pass);
            }
            else if (updatedList[i] == 2)
            {
                playerItems[playerOrderIndex].Add(items.erase);
            }
            else if (updatedList[i] == 3)
            {
                playerItems[playerOrderIndex].Add(items.timeSteal);
            }
            else if (updatedList[i] == 4)
            {
                playerItems[playerOrderIndex].Add(items.bind);
            }
            else
            {
                playerItems[playerOrderIndex].Add(items.cardSteal);
            }
        }

    }
}
