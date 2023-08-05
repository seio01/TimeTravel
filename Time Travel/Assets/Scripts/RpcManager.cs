using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.SceneManagement;

public class RpcManager : MonoBehaviour
{
    public static RpcManager instance;
    public TMP_Text testTMP;
    public TMP_Text resultText;
    public PhotonView PV;

    public Dictionary<Photon.Realtime.Player, string> currentTurnItems;
    public string currentTurnUsedItemOfLocalPlayer;
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
        currentTurnItems = new Dictionary<Photon.Realtime.Player, string>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void updatePlayerBoardNum(playerInformationUI obj, int playerPosition)
    {
        obj.playerPositionText.text = (playerPosition).ToString();
    }

    public void useItemOfLocalPlayer()
    {
        PV.RPC("eraseItemToOthers", RpcTarget.AllViaServer, GameManager.instance.localPlayerIndexWithOrder, currentTurnUsedItemOfLocalPlayer);
    }

    public void eraseItem(int index, DontDestroyObjects.items itemName)
    {
        PV.RPC("eraseItemToOthers", RpcTarget.AllViaServer, index, itemName.ToString());
    }

    public void useAsetItemCard(DontDestroyObjects.items itemName)
    {
        PV.RPC("useAsetItemCardToOthers", RpcTarget.All, itemName.ToString());
    }

    public void useBsetItemCard(DontDestroyObjects.items itemName)
    {
        PV.RPC("useBsetItemCardToOthers", RpcTarget.MasterClient, PhotonNetwork.LocalPlayer, itemName.ToString());
    }

    public void cardSteal(int playerIndex, int stolenCardIndex)
    {
        PV.RPC("cardStealToOthers", RpcTarget.All, playerIndex, stolenCardIndex);
    }

    public void setIsThisTurnTimeStealTrue()
    {
        PV.RPC("setIsThisTurnTimeStealTrueToOthers", RpcTarget.All);
    }

    public void setResultText()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (currentTurnItems.Count == 0)
            {
                PV.RPC("setResultTextToOthersWithNoOneUsed", RpcTarget.All);
            }
            else
            {

                List<string> itemUsePlayers = new List<string>();
                List<string> usedItems = new List<string>();
                foreach (KeyValuePair<Photon.Realtime.Player, string> entry in currentTurnItems)
                {
                    itemUsePlayers.Add(entry.Key.ToString());
                    usedItems.Add(entry.Value);
                }
                PV.RPC("setResultTextToOthers", RpcTarget.All, itemUsePlayers.ToArray(), usedItems.ToArray());
            }
        }
    }

    [PunRPC]
    void setResultTextToOthers(string[] players, string[] items)
    {
        for (int i = 0; i < players.Length; i++)
        {
            resultText.text += players[i] + "님이 " + items[i] + "를 사용했습니다.";
        }
    }

    [PunRPC]
    void setResultTextToOthersWithNoOneUsed()
    {
        resultText.text = "사용된 아이템이 없습니다.\n";
    }

    public void removeCurrentTurnItems()
    {
        currentTurnItems.Clear();
    }

    [PunRPC]
    void eraseItemToOthers(int index, string itemName)
    {
        if (currentTurnUsedItemOfLocalPlayer == "bind")
        {
            DontDestroyObjects.instance.playerItems[index].Remove(DontDestroyObjects.items.bind);
        }
        else if (currentTurnUsedItemOfLocalPlayer == "cardSteal")
        {
            DontDestroyObjects.instance.playerItems[index].Remove(DontDestroyObjects.items.cardSteal);
        }
        else
        {
            DontDestroyObjects.instance.playerItems[index].Remove(DontDestroyObjects.items.timeSteal);
        }
        GameManager.instance.eraseItemUI(index, itemName);
    }

    [PunRPC]
    void useAsetItemCardToOthers(string itemName)
    {
        GameManager.instance.eraseItemUI(GameManager.instance.controlPlayerIndexWithOrder, itemName);
    }

    [PunRPC]
    void useBsetItemCardToOthers(Photon.Realtime.Player p, string itemName)
    {
        currentTurnItems.Add(p, itemName);
    }

    [PunRPC]
    void cardStealToOthers(int playerIndex, int stolenCardIndex)
    {
        GameManager.instance.eraseItemUI(playerIndex, stolenCardIndex);
    }

    [PunRPC]
    void setIsThisTurnTimeStealTrueToOthers()
    {
        GameManager.instance.isThisTurnTimeSteal = true;
    }
}
