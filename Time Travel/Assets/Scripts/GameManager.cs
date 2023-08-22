using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using Photon.Pun.UtilityScripts;
public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public int[] playerStartPoint;

    public int playerOrder;

    public Player[] player;
    public Sprite[] player1Clothes;
    public Sprite[] player2Clothes;
    public Sprite[] player3Clothes;
    public Sprite[] player4Clothes;
    public int newDiceSide;
    public bool timerOn;
    public bool isLadder;
    public bool isTransport;
    public bool finishRound;
    public string spaceCategory;
    public int correctCount;
    public bool secondRoll;//정답 5번일때 
    public bool isOver;

    [Header("UI")]
    public GameObject canvas;
    public Canvas problemCanvas;
    public Dice dice;
    public Space spaceAction;
    public Text diceTimer;
    public Text spaceText;
    public TMP_Text startRoundText;
    public GameObject startRoundPanel;
    public GameObject space;
    public GameObject diceImg;
    public Image[] gaugeImg;
    public GameObject[] playerInformationUIs;
    public GameObject itemUsePanel;
    public GameObject itemUseResultPanel;
    public Photon.Realtime.Player controlPlayer; //문제 푸는 사람. 현재 차례인 플레이어.
    public Sprite[] itemSmallSprites;
    public problem problemMaker;
    public GameObject endPanel;
    public GameObject endGameText;
    public string winner;
    public TMP_Text winnerName;
    public bool nextTurn;

    public TMP_Text testTMP;
    public int controlPlayerIndexWithOrder;
    public int localPlayerIndexWithOrder;
    public bool isThisTurnTimeSteal;
    public bool isUsedBind = false;
    public bool isMovableWithBind = false;
    public int bindPlayerIndex;

    public int currentTurnASetItem;
    public int currentTurnBSetItem;
    public bool AllDoesntHaveBsetCard;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        problemMaker.problemData= CSVReader.Read("문제");
        problemMaker.answerData = CSVReader.Read("답");
        problemMaker.problemScript = problemMaker.gameObject.GetComponent<problemGraph>();
        playerStartPoint = new int[PhotonNetwork.CurrentRoom.PlayerCount];
        for (int i = 0; i < PhotonNetwork.CurrentRoom.PlayerCount; i++)
        {
            playerStartPoint[i] = 0;
        }
        setPlayerInformationUIs();
        setPlayerPieces();
        setLocalPlayerIndexWithOrder();
        initVariables();
    }

    void initVariables()
    {
        currentTurnASetItem = 0;
        currentTurnBSetItem = 0;
        isThisTurnTimeSteal = false;
        isUsedBind = false;
        isMovableWithBind = false;
        AllDoesntHaveBsetCard = false;
        controlPlayerIndexWithOrder = 0;
        controlPlayer = DontDestroyObjects.instance.playerListWithOrder[0];
    }

    // Start is called before the first frame update
    void Start()
    {
        Invoke("RoundStart", 1.5f);
    }

    // Update is called once per frame
    void Update()
    {
        //게임 종료
        if (isOver)
            return;
        //when to stop moving
        if (player[controlPlayerIndexWithOrder].curIndex > playerStartPoint[controlPlayerIndexWithOrder] + newDiceSide)
        {
            player[controlPlayerIndexWithOrder].movingAllowed = false;
            playerStartPoint[controlPlayerIndexWithOrder] = player[controlPlayerIndexWithOrder].curIndex - 1;

            if (secondRoll)
            {
                secondRoll = false;
                finishRound = true;
                UISmaller();
            }
                
            //정답 5번이면 주사위 한번 더 굴리기
            if (player[GameManager.instance.controlPlayerIndexWithOrder].correctCount == 5)
            {
                player[GameManager.instance.controlPlayerIndexWithOrder].correctCount = 0;
                StartCoroutine(RollDiceAndGetItem());
            }
            if (isLadder && !player[controlPlayerIndexWithOrder].movingAllowed)
            {
                player[controlPlayerIndexWithOrder].moveLadder = true;
            }
            else if (isTransport && !player[controlPlayerIndexWithOrder].movingAllowed)
            {
                player[controlPlayerIndexWithOrder].Transport();
            }
            else
            {
                if (isMovableWithBind == true)
                {
                    updatePlayerInformationUI(controlPlayerIndexWithOrder);
                    RpcManager.instance.isMovableWithBind = true;
                    moveBindPlayer(RpcManager.instance.bindPlayerIndex);
                }
                else if(!secondRoll)
                {
                    finishRound = true;
                    UISmaller();
                }
            }

        }

        if (finishRound && nextTurn)
        {
            finishRound = false;
            isMovableWithBind = false;
            isUsedBind = false;
            if (controlPlayer == PhotonNetwork.LocalPlayer)
            {
                updatePlayerInformationUI(controlPlayerIndexWithOrder);
            }
            controlPlayerIndexWithOrder++;
            if (controlPlayerIndexWithOrder == PhotonNetwork.CurrentRoom.PlayerCount)
            {
                controlPlayerIndexWithOrder = 0;
            }
            controlPlayer = DontDestroyObjects.instance.playerListWithOrder[controlPlayerIndexWithOrder];
            Invoke("RoundStart", 1);
        }
    }



    IEnumerator UIBiggerRoutine(bool bigger)
    {
        player[controlPlayerIndexWithOrder].gameObject.GetComponent<SpriteRenderer>().sortingOrder = 2;
        float time = 0f;

        while (bigger)
        {
            playerInformationUIs[controlPlayerIndexWithOrder].transform.localScale = Vector3.one * (1 + time);
            time += Time.deltaTime;
            if (time > 0.3f)
            {
                bigger = false;
            }
            yield return null;
        }
    }

    public void UISmaller()
    {
        StartCoroutine(UISmallerRoutine(true));
    }
    IEnumerator UISmallerRoutine(bool smaller)
    {
        player[controlPlayerIndexWithOrder].gameObject.GetComponent<SpriteRenderer>().sortingOrder = 0;
        float time = 0f;
        while (smaller)
        {
            playerInformationUIs[controlPlayerIndexWithOrder].transform.localScale = Vector3.one * (1.3f - time);
            time += Time.deltaTime;
            if (time > 0.3f)
            {
                smaller = false;
            }
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);

        nextTurn = true;

    }

    public void setPlayerInformationUIs()
    {
        int playerCount = PhotonNetwork.CurrentRoom.PlayerCount;
        for (int i = 0; i < playerCount; i++)
        {
            DontDestroyObjects.items[] arr = DontDestroyObjects.instance.playerItems[i].ToArray();
            for (int j = 0; j< 4; j++)
            {
                if (arr[j] == DontDestroyObjects.items.hint)
                {
                    playerInformationUIs[i].transform.GetChild(1).GetChild(j).GetComponent<Image>().sprite = itemSmallSprites[0];
                }
                else if (arr[j] == DontDestroyObjects.items.erase)
                {
                    playerInformationUIs[i].transform.GetChild(1).GetChild(j).GetComponent<Image>().sprite = itemSmallSprites[1];
                }
                else if (arr[j] == DontDestroyObjects.items.pass)
                {
                    playerInformationUIs[i].transform.GetChild(1).GetChild(j).GetComponent<Image>().sprite = itemSmallSprites[2];
                }
                else if (arr[j] == DontDestroyObjects.items.cardSteal)
                {
                    playerInformationUIs[i].transform.GetChild(1).GetChild(j).GetComponent<Image>().sprite = itemSmallSprites[3];
                }
                else if (arr[j] == DontDestroyObjects.items.timeSteal)
                {
                    playerInformationUIs[i].transform.GetChild(1).GetChild(j).GetComponent<Image>().sprite = itemSmallSprites[4];
                }
                else
                {
                    playerInformationUIs[i].transform.GetChild(1).GetChild(j).GetComponent<Image>().sprite = itemSmallSprites[5];
                }
            }
            playerInformationUIs[i].transform.GetChild(0).GetComponent<TMP_Text>().text = DontDestroyObjects.instance.playerListWithOrder[i].NickName;

           playerInformationUIs[i].SetActive(true);
        }
    }

    void setPlayerPieces()
    {
        int playerCount = PhotonNetwork.CurrentRoom.PlayerCount;
        for (int i = 0; i < playerCount; i++)
        {
            player[i].gameObject.SetActive(true);
        }
    }

    public void RoundStart()
    {

        nextTurn = false;
        StartCoroutine(UIBiggerRoutine(true));
        //수정..?
        currentTurnASetItem = 0;
        currentTurnBSetItem = 0;
        AllDoesntHaveBsetCard = false;
        if (correctCount != 5)
            secondRoll = false;
        player[controlPlayerIndexWithOrder].moveLadder = false;
        finishRound = false;
        StartCoroutine(RoundStartRoutine());
    }

    IEnumerator RoundStartRoutine()
    {
        startRoundText.text = controlPlayer.NickName + "님 플레이를 시작합니다."; //문구 수정
        startRoundPanel.SetActive(true);

        yield return new WaitForSeconds(1.5f);

        startRoundPanel.SetActive(false);

        yield return new WaitForSeconds(1f);

        RpcManager.instance.setDiceTrue();
        timerOn = true;
    }

    IEnumerator RollDiceAndGetItem()
    {
        secondRoll = true;
        if (DontDestroyObjects.instance.playerItems[controlPlayerIndexWithOrder].Count < 4) //가지고있는 아이템 개수가 3개이하일때 아이템 한장 추가 획득
        {
            int itemCount = DontDestroyObjects.instance.playerItems[controlPlayerIndexWithOrder].Count;
            spaceText.text = "랜덤한 아이템 하나를 추가로 획득합니다!";
            space.SetActive(true);

            int ran = Random.Range(0, 6);
            GameObject itemUIPrefab = Resources.Load<GameObject>("Prefabs/itemImageUI");
            GameObject createdItem = Instantiate(itemUIPrefab);

            if (ran == 0)
            {
                DontDestroyObjects.instance.playerItems[controlPlayerIndexWithOrder].Add(DontDestroyObjects.items.hint);
                createdItem.transform.SetParent(playerInformationUIs[controlPlayerIndexWithOrder].transform.GetChild(1), false);
                createdItem.GetComponent<Image>().sprite = itemSmallSprites[0];

            }
            else if (ran == 1)
            {
                DontDestroyObjects.instance.playerItems[controlPlayerIndexWithOrder].Add(DontDestroyObjects.items.erase);
                createdItem.transform.SetParent(playerInformationUIs[controlPlayerIndexWithOrder].transform.GetChild(1), false);
                createdItem.GetComponent<Image>().sprite = itemSmallSprites[1];

            }
            else if (ran == 2)
            {
                DontDestroyObjects.instance.playerItems[controlPlayerIndexWithOrder].Add(DontDestroyObjects.items.pass);
                createdItem.transform.SetParent(playerInformationUIs[controlPlayerIndexWithOrder].transform.GetChild(1), false);
                createdItem.GetComponent<Image>().sprite = itemSmallSprites[3];
            }
            else if (ran == 3)
            {
                DontDestroyObjects.instance.playerItems[controlPlayerIndexWithOrder].Add(DontDestroyObjects.items.cardSteal);
                createdItem.transform.SetParent(playerInformationUIs[controlPlayerIndexWithOrder].transform.GetChild(1), false);
                createdItem.GetComponent<Image>().sprite = itemSmallSprites[0];
            }
            else if (ran == 4)
            {
                DontDestroyObjects.instance.playerItems[controlPlayerIndexWithOrder].Add(DontDestroyObjects.items.timeSteal);
                createdItem.transform.SetParent(playerInformationUIs[controlPlayerIndexWithOrder].transform.GetChild(1), false);
                createdItem.GetComponent<Image>().sprite = itemSmallSprites[4];
            }
            else
            {
                DontDestroyObjects.instance.playerItems[controlPlayerIndexWithOrder].Add(DontDestroyObjects.items.bind);
                createdItem.transform.SetParent(playerInformationUIs[controlPlayerIndexWithOrder].transform.GetChild(1), false);
                createdItem.GetComponent<Image>().sprite = itemSmallSprites[5];
            }

            yield return new WaitForSeconds(1f);
        }
        spaceText.text = "주사위를 한번 더 굴릴 수 있습니다!";
        space.SetActive(true);

        yield return new WaitForSeconds(1.5f);
        space.SetActive(false);
        ResetGaugeImg();

        yield return new WaitForSeconds(1f);
        diceImg.SetActive(true);
        diceTimer.gameObject.SetActive(true);
        timerOn = true;


    }

    public void MovePlayer()
    {
        player[controlPlayerIndexWithOrder].movingAllowed = true;
    }

    public void moveBindPlayer(int bindPlayerIndex)
    {
        player[bindPlayerIndex].movingAllowed = true;
    }
        
    //check
    public int getPlayerNextPosition()
    {
        return player[controlPlayerIndexWithOrder].curIndex + newDiceSide;
    }

    public void useItemCard(DontDestroyObjects.items itemName)
    {
        DontDestroyObjects.instance.playerItems[controlPlayerIndexWithOrder].Remove(itemName);
        RpcManager.instance.eraseItem(controlPlayerIndexWithOrder, itemName);
    }

    public void RpcCheck(string s)
    {
        testTMP.text = s;
    }

    public void CheckCurPoint(int diceNum)
    {
        switch (diceNum)
        {
            case 3:
            case 8:
            case 11:
            case 12:
            case 19:
            case 24:
            case 28:
            case 34:
            case 42:
            case 43:
            case 49:
            case 50:
            case 55:
            case 58:
            case 70:
            case 74:
            case 78:
            case 82:
            case 91:
                spaceCategory = "Nothing";
                spaceText.text = "현재 칸은 빈 칸입니다.\n 바로 이동가능합니다.";
                break;
            case 7:
            case 22:
            case 53:
            case 64:
            case 76:
                spaceCategory = "Ladder";
                spaceText.text = "현재 칸은 사다리 칸입니다.\n 사다리를 타고 이동합니다.";
                break;
            case 15:
            case 30:
            case 26:
            case 38:
            case 32:
            case 80:
            case 46:
            case 67:
            case 62:
            case 90:
                spaceCategory = "Portal";
                spaceText.text = "현재 칸은 포털 칸입니다.\n 같은 색깔의 포털로 이동합니다.";
                break;
            default:
                spaceCategory = "Problem";
                spaceText.text = "현재 칸은 문제 칸입니다.\n 문제를 맞추면 이동가능합니다.";
                break;
        }
        space.SetActive(true);

        StartCoroutine(DoActionRoutine());
        
    }

    IEnumerator DoActionRoutine()
    {
        yield return new WaitForSeconds(1.5f);
        space.SetActive(false);

        if(spaceCategory == "Problem")
        {
            activeItemUsePanel();
            yield return new WaitForSeconds(3.0f);
            if (AllDoesntHaveBsetCard == false)
            {
                yield return new WaitForSeconds(3.0f);
                activeItemUseResultPanel();
                yield return new WaitForSeconds(3.5f);
            }
        }
        else
            yield return new WaitForSeconds(1.5f);
        spaceAction.DoAction(spaceCategory);
    }

    public void UpdateGaugeImg()
    {
        for (int i = 0; i < player[GameManager.instance.controlPlayerIndexWithOrder].correctCount; i++)
        {
            playerInformationUIs[controlPlayerIndexWithOrder].transform.Find("Gauge Bar").GetChild(i).GetComponent<Image>().color = new Color(1, 1, 1, 1);

        }
    }

    void ResetGaugeImg()
    {
        for (int i = 0; i < 5; i++)
        {
            playerInformationUIs[controlPlayerIndexWithOrder].transform.Find("Gauge Bar").GetChild(i).GetComponent<Image>().color = new Color(1, 1, 1, 0.3f);
        }
    }



    

    public void EndGame(string name)
    {
        winner = name;
        isOver = true;
        StartCoroutine(EndGameRoutine());
    }

    IEnumerator EndGameRoutine()
    {
        endGameText.SetActive(true);

        yield return new WaitForSeconds(0.5f);

        endGameText.SetActive(false);
        RpcManager.instance.ShowEndPanel();

    }

    public void ReturnToLobby()
    {
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene("Main");
    }


    public void updatePlayerInformationUI(int controlPlayerIndex)
    {
        playerInformationUIs[controlPlayerIndex].GetComponent<playerInformationUI>().updatePlayerBoardNum(player[controlPlayerIndex].curIndex - 1);
    }


    void setLocalPlayerIndexWithOrder()
    {
        for (int i = 0; i < PhotonNetwork.CurrentRoom.MaxPlayers; i++)
        {
            if (DontDestroyObjects.instance.playerListWithOrder[i] == PhotonNetwork.LocalPlayer)
            {
                localPlayerIndexWithOrder = i;
                break;
            }
        }
    }

    public void activeItemUsePanel()
    {
        itemUsePanel.SetActive(true);
    }

    public void activeItemUseResultPanel()
    {
        itemUseResultPanel.SetActive(true);
    }

    
    public void eraseItemUI(int playerIndex, int cardIndex)
    {
        GameObject itemPanel = playerInformationUIs[controlPlayerIndexWithOrder].transform.GetChild(1).gameObject;
        GameObject stolenCard = itemPanel.transform.GetChild(cardIndex).gameObject;
        string itemName = stolenCard.GetComponent<Image>().sprite.name;
        GameObject itemUIPrefab = Resources.Load<GameObject>("Prefabs/itemImageUI");
        Destroy(stolenCard);
        if (itemName == "운명공동체 UI")
        {
            DontDestroyObjects.instance.playerItems[controlPlayerIndexWithOrder].Remove(DontDestroyObjects.items.bind);
            DontDestroyObjects.instance.playerItems[playerIndex].Add(DontDestroyObjects.items.bind);
            GameObject createdItem = Instantiate(itemUIPrefab);
            createdItem.transform.SetParent(playerInformationUIs[playerIndex].transform.GetChild(1), false);
            createdItem.GetComponent<Image>().sprite = itemSmallSprites[5];
        }
        else if (itemName == "카드빼앗기")
        {
            DontDestroyObjects.instance.playerItems[controlPlayerIndexWithOrder].Remove(DontDestroyObjects.items.cardSteal);
            DontDestroyObjects.instance.playerItems[playerIndex].Add(DontDestroyObjects.items.cardSteal);
            GameObject createdItem = Instantiate(itemUIPrefab);
            createdItem.transform.SetParent(playerInformationUIs[playerIndex].transform.GetChild(1), false);
            createdItem.GetComponent<Image>().sprite = itemSmallSprites[3];
        }
        else if (itemName == "시간빼앗기 UI")
        {
            DontDestroyObjects.instance.playerItems[controlPlayerIndexWithOrder].Remove(DontDestroyObjects.items.timeSteal);
            DontDestroyObjects.instance.playerItems[playerIndex].Add(DontDestroyObjects.items.timeSteal);
            GameObject createdItem = Instantiate(itemUIPrefab);
            createdItem.transform.SetParent(playerInformationUIs[playerIndex].transform.GetChild(1), false);
            createdItem.GetComponent<Image>().sprite = itemSmallSprites[4];
        }
        else if (itemName == "힌트 UI")
        {
            DontDestroyObjects.instance.playerItems[controlPlayerIndexWithOrder].Remove(DontDestroyObjects.items.hint);
            DontDestroyObjects.instance.playerItems[playerIndex].Add(DontDestroyObjects.items.hint);
            GameObject createdItem = Instantiate(itemUIPrefab);
            createdItem.transform.SetParent(playerInformationUIs[playerIndex].transform.GetChild(1), false);
            createdItem.GetComponent<Image>().sprite = itemSmallSprites[0];
        }
        else if (itemName == "선택지 지우기 UI")
        {
            DontDestroyObjects.instance.playerItems[controlPlayerIndexWithOrder].Remove(DontDestroyObjects.items.erase);
            DontDestroyObjects.instance.playerItems[playerIndex].Add(DontDestroyObjects.items.bind);
            GameObject createdItem = Instantiate(itemUIPrefab);
            createdItem.transform.SetParent(playerInformationUIs[playerIndex].transform.GetChild(1), false);
            createdItem.GetComponent<Image>().sprite = itemSmallSprites[1];
        }
        else
        {
            DontDestroyObjects.instance.playerItems[controlPlayerIndexWithOrder].Remove(DontDestroyObjects.items.pass);
            DontDestroyObjects.instance.playerItems[playerIndex].Add(DontDestroyObjects.items.pass);
            GameObject createdItem = Instantiate(itemUIPrefab);
            createdItem.transform.SetParent(playerInformationUIs[playerIndex].transform.GetChild(1), false);
            createdItem.GetComponent<Image>().sprite = itemSmallSprites[2];
        }

    }

    public void eraseItemUI(int index, string itemName)
    {
        GameObject itemPanel = playerInformationUIs[index].transform.GetChild(1).gameObject;
        string itemNameToSpirteName = "";
        if (itemName == "운명공동체")
        {
            itemNameToSpirteName = "운명공동체 UI";
        }
        else if (itemName == "카드빼앗기")
        {
            itemNameToSpirteName = "카드 빼앗기";
        }
        else if (itemName == "시간빼앗기")
        {
            itemNameToSpirteName = "시간 빼앗기 UI";
        }
        else if (itemName == "힌트" || itemName=="hint" )
        {
            itemNameToSpirteName = "힌트 UI";
        }
        else if (itemName == "선택지제거" || itemName == "erase" )
        {
            itemNameToSpirteName = "선택지 지우기 UI";
        }
        else
        {
            itemNameToSpirteName = "문제 스킵 UI";
        }
        foreach (Transform child in itemPanel.transform)
        {
            if (child.gameObject.GetComponent<Image>().sprite.name == itemNameToSpirteName)
            {
                Destroy(child.gameObject);
                break;
            }
        }
    }

    public void ChangeClothes(string age)
    {
        switch (age)
        {
            case "삼국시대":
                if(controlPlayerIndexWithOrder == 0)
                {
                    player[controlPlayerIndexWithOrder].gameObject.GetComponent<SpriteRenderer>().sprite = player1Clothes[1];
                }
                else if (controlPlayerIndexWithOrder == 1)
                {
                    player[controlPlayerIndexWithOrder].gameObject.GetComponent<SpriteRenderer>().sprite = player2Clothes[1];
                }
                else if (controlPlayerIndexWithOrder == 2)
                {
                    player[controlPlayerIndexWithOrder].gameObject.GetComponent<SpriteRenderer>().sprite = player3Clothes[1];
                }
                else if (controlPlayerIndexWithOrder == 3)
                {
                    player[controlPlayerIndexWithOrder].gameObject.GetComponent<SpriteRenderer>().sprite = player4Clothes[1];
                }

                break;
            case "고려시대":
                if (controlPlayerIndexWithOrder == 0)
                {
                    player[controlPlayerIndexWithOrder].gameObject.GetComponent<SpriteRenderer>().sprite = player1Clothes[2];
                }
                else if (controlPlayerIndexWithOrder == 1)
                {
                    player[controlPlayerIndexWithOrder].gameObject.GetComponent<SpriteRenderer>().sprite = player2Clothes[2];
                }
                else if (controlPlayerIndexWithOrder == 2)
                {
                    player[controlPlayerIndexWithOrder].gameObject.GetComponent<SpriteRenderer>().sprite = player3Clothes[2];
                }
                else if (controlPlayerIndexWithOrder == 3)
                {
                    player[controlPlayerIndexWithOrder].gameObject.GetComponent<SpriteRenderer>().sprite = player4Clothes[2];
                }
                break;
            case "조선시대":
                if (controlPlayerIndexWithOrder == 0)
                {
                    player[controlPlayerIndexWithOrder].gameObject.GetComponent<SpriteRenderer>().sprite = player1Clothes[3];
                }
                else if (controlPlayerIndexWithOrder == 1)
                {
                    player[controlPlayerIndexWithOrder].gameObject.GetComponent<SpriteRenderer>().sprite = player2Clothes[3];
                }
                else if (controlPlayerIndexWithOrder == 2)
                {
                    player[controlPlayerIndexWithOrder].gameObject.GetComponent<SpriteRenderer>().sprite = player3Clothes[3];
                }
                else if (controlPlayerIndexWithOrder == 3)
                {
                    player[controlPlayerIndexWithOrder].gameObject.GetComponent<SpriteRenderer>().sprite = player4Clothes[3];
                }
                break;
            case "근현대":
                if (controlPlayerIndexWithOrder == 0)
                {
                    player[controlPlayerIndexWithOrder].gameObject.GetComponent<SpriteRenderer>().sprite = player1Clothes[4];
                }
                else if (controlPlayerIndexWithOrder == 1)
                {
                    player[controlPlayerIndexWithOrder].gameObject.GetComponent<SpriteRenderer>().sprite = player2Clothes[4];
                }
                else if (controlPlayerIndexWithOrder == 2)
                {
                    player[controlPlayerIndexWithOrder].gameObject.GetComponent<SpriteRenderer>().sprite = player3Clothes[4];
                }
                else if (controlPlayerIndexWithOrder == 3)
                {
                    player[controlPlayerIndexWithOrder].gameObject.GetComponent<SpriteRenderer>().sprite = player4Clothes[4];
                }
                break;
            default:
                if (controlPlayerIndexWithOrder == 0)
                {
                    player[controlPlayerIndexWithOrder].gameObject.GetComponent<SpriteRenderer>().sprite = player1Clothes[0];
                }
                else if (controlPlayerIndexWithOrder == 1)
                {
                    player[controlPlayerIndexWithOrder].gameObject.GetComponent<SpriteRenderer>().sprite = player2Clothes[0];
                }
                else if (controlPlayerIndexWithOrder == 2)
                {
                    player[controlPlayerIndexWithOrder].gameObject.GetComponent<SpriteRenderer>().sprite = player3Clothes[0];
                }
                else if (controlPlayerIndexWithOrder == 3)
                {
                    player[controlPlayerIndexWithOrder].gameObject.GetComponent<SpriteRenderer>().sprite = player4Clothes[0];
                }
                break;

        }
    }

    public void Flip(bool isFlip)
    {
        player[controlPlayerIndexWithOrder].gameObject.GetComponent<SpriteRenderer>().flipX = isFlip;
    }
}

