using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RpcManager : MonoBehaviour
{
    public static RpcManager instance;
    public TMP_Text testTMP;
    public TMP_Text resultText;
    public PhotonView PV;

    public Dictionary<Photon.Realtime.Player, string> currentTurnItems;
    public string currentTurnUsedItemOfLocalPlayer;

    public GameObject diceImg;
    public Text diceTimer;
    public Dice dice;

    public problem problemScript;
    public Canvas problemCanvas;

    public bool isUpdatedPlayerUI;

    public bool isMovableWithBind;
    public int bindPlayerIndex;
    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        isUpdatedPlayerUI = false;
        isMovableWithBind = false;
        bindPlayerIndex = -1;
    }

    void Start()
    {
        currentTurnItems = new Dictionary<Photon.Realtime.Player, string>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isMovableWithBind == true)
        {
            if (GameManager.instance.player[bindPlayerIndex].curIndex > GameManager.instance.playerStartPoint[bindPlayerIndex] + GameManager.instance.newDiceSide)
            {
                GameManager.instance.player[bindPlayerIndex].movingAllowed = false;
                GameManager.instance.playerStartPoint[bindPlayerIndex] = GameManager.instance.player[bindPlayerIndex].curIndex - 1;
                if (GameManager.instance.isLadder && !GameManager.instance.player[bindPlayerIndex].movingAllowed)
                {
                    GameManager.instance.player[bindPlayerIndex].moveLadder = true;
                }
                else if (GameManager.instance.isTransport && !GameManager.instance.player[bindPlayerIndex].movingAllowed)
                {
                    GameManager.instance.player[bindPlayerIndex].Transport();
                }
                else
                {
                    isMovableWithBind = false;
                    GameManager.instance.isMovableWithBind = false;
                    GameManager.instance.updatePlayerInformationUI(bindPlayerIndex);
                    GameManager.instance.finishRound = true;
                }
            }
        }
    }

    public void updatePlayerBoardNum(playerInformationUI obj, int playerPosition)
    {
        if (playerPosition == -1)
        {
            obj.playerPositionText.text = "0";
        }
        else
        {
            obj.playerPositionText.text = (playerPosition).ToString();
        }
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

    public void moveWithBind(int localPlayerIndex)
    {
        PV.RPC("moveWithBindToOthers", RpcTarget.AllViaServer, localPlayerIndex);
    }

    public void makeIsUsedBindTrue(int localPlayerIndex)
    {
        PV.RPC("makeIsUsedBindTrueToOthers", RpcTarget.AllViaServer, localPlayerIndex);
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
            resultText.text += players[i] + "님이 ";
            if (items[i] == "cardSteal")
            {
                resultText.text+= "카드 빼앗기를 사용했습니다.\n";
            }
            else if (items[i] == "timeSteal")
            {
                resultText.text += "시간 빼앗기를 사용했습니다.\n";
            }
            else
            {
                resultText.text += "운명공동체를 사용했습니다.\n";
            }
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
        int playerIndex = GameManager.instance.controlPlayerIndexWithOrder;
        if (itemName == "erase")
        {
            DontDestroyObjects.instance.playerItems[playerIndex].Remove(DontDestroyObjects.items.erase);
        }
        else if (itemName == "hint")
        {
            DontDestroyObjects.instance.playerItems[playerIndex].Remove(DontDestroyObjects.items.hint);
        }
        else
        {
            DontDestroyObjects.instance.playerItems[playerIndex].Remove(DontDestroyObjects.items.pass);
        }
        testTMP.text = DontDestroyObjects.instance.playerItems[playerIndex].Count.ToString();
        GameManager.instance.eraseItemUI(playerIndex, itemName);
    }

    [PunRPC]
    void useBsetItemCardToOthers(Photon.Realtime.Player p, string itemName)
    {
        currentTurnItems.Add(p, itemName);
    }

    [PunRPC]
    void moveWithBindToOthers(int localPlayerIndex)
    {
        bindPlayerIndex = localPlayerIndex;
        isMovableWithBind = true;
        GameManager.instance.moveBindPlayer(localPlayerIndex);
    }

    [PunRPC]
    void makeIsUsedBindTrueToOthers(int localPlayerIndex)
    {
        bindPlayerIndex = localPlayerIndex;
        GameManager.instance.isMovableWithBind = true;
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
       //GameManager.instance. player[bindPlayerIndex].movingAllowed = true;
    }

    public void setDiceTrue()
    {
        if (GameManager.instance.controlPlayer == PhotonNetwork.LocalPlayer)
        {
            PV.RPC("setDiceTrueToOthers", RpcTarget.AllViaServer);
        }

    }

    [PunRPC]
    public void setDiceTrueToOthers()
    {
        diceImg.SetActive(true);
        diceTimer.gameObject.SetActive(true);
    }

    public void rollDice()
    {
        int[] diceSpriteIndex = new int[11];
        for (int i = 0; i <= 10; i++)
        {
            diceSpriteIndex[i] = Random.Range(0, 6);
        }
        PV.RPC("rollDiceRPC", RpcTarget.AllViaServer, diceSpriteIndex);
    }

    [PunRPC]
    public void rollDiceRPC(int[] diceSpriteIndex)
    {
        dice.rollDice(diceSpriteIndex);
    }

    public void showProblem()
    {
        PV.RPC("showProblemToOthers", RpcTarget.AllViaServer);
    }

    public void setProblemID(int playerPosition)
    {
        int problemID = 0;
        int prevDynasty = 0;
        Debug.Log(playerPosition);
        if (playerPosition >= 1 && playerPosition <= 8)
        {
            problemID = Random.Range(1, 31);
            prevDynasty = 0;
        }
        else if (playerPosition >= 9 && playerPosition <= 20)
        {
            problemID = Random.Range(1, 66) + 30;
            prevDynasty = 30;
        }
        else if (playerPosition >= 21 && playerPosition <= 40)
        {
            problemID = Random.Range(1, 81) + 95;
            prevDynasty = 95;
        }
        else if (playerPosition >= 41 && playerPosition <= 70)
        {
            problemID = Random.Range(1, 111) + 175;
            prevDynasty = 175;
        }
        else
        {
            problemID = Random.Range(1, 121) + 285;
            prevDynasty =285;
        }
        PV.RPC("setProblemIDToOThers", RpcTarget.AllViaServer, problemID, prevDynasty);
    }

    [PunRPC]
    void showProblemToOthers()
    {
        problemCanvas.gameObject.SetActive(true);
    }

    [PunRPC]
    public void setProblemIDToOThers(int problemID, int prevDynasty)
    {
        problemScript.problemID = problemID;
        problemScript.prevDynasty = prevDynasty;
        problemScript.setProblemPanel(problemID, prevDynasty);
    }

    //endgame
    public void ShowEndPanel()
    {
        PV.RPC("ShowEndPanelToOthers", RpcTarget.All, GameManager.instance.winner, GameManager.instance.isOver);
    }

    [PunRPC]
    public void ShowEndPanelToOthers(string winner, bool isOver)
    {
        GameManager.instance.endPanel.SetActive(true);
        GameManager.instance.isOver = isOver;
        GameManager.instance.winnerName.text = winner + " 님 승리를 축하합니다!";
    }

}
