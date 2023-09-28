using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using Photon.Pun.UtilityScripts;

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager instance;
    public int[] playerStartPoint;

    public int playerOrder;

    public Player[] player;
    public Sprite[] player1Clothes;
    public Sprite[] player2Clothes;
    public Sprite[] player3Clothes;
    public Sprite[] player4Clothes;

    public Vector3 curCamPos;
    public Vector3 minCamPos;
    public Vector3 maxCamPos;

    public CheckIncorrectProblems checkIncorrectProblemSc;

    public int newDiceSide;
    public bool timerOn;
    public bool isLadder;
    public bool isTransport;
    public bool finishRound;
    public string spaceCategory;
    public int correctCount;
    public bool secondRoll;//정답 5번일때 
    public bool isOver;
    public bool gameStart;
    public int nowMovingPlayerIndex;


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
    public AudioClip newBGMClip;
    public TMP_Text controlPlayerNameText;

    public TMP_Text testTMP;
    public TMP_Text answerText;
    public int controlPlayerIndexWithOrder;
    public int localPlayerIndexWithOrder;
    public bool isThisTurnTimeSteal;
    public bool isUsedBind = false;
    public bool isMovableWithBind = false;
    public int bindPlayerIndex;

    public int currentTurnASetItem;
    public int currentTurnBSetItem;
    public bool AllDoesntHaveBsetCard;
    public bool movableAfterSecondRoll;
    public int initialPlayerNum;
    
    public GameObject diceAndSoundPanel;

    public quitInTheMiddle quitScript;
    public GameObject noIncorrectText;

    public int bindDiceSide;

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
        initialPlayerNum = PhotonNetwork.CurrentRoom.PlayerCount;
        setPlayerInformationUIs();
        setPlayerPieces();
        setLocalPlayerIndexWithOrder();
        initVariables();
        SoundManager.instance.bgmPlayer.clip = newBGMClip;
        SoundManager.instance.bgmPlayer.Play();

    }

    void initVariables()
    {
        currentTurnASetItem = 0;
        currentTurnBSetItem = 0;
        isThisTurnTimeSteal = false;
        isUsedBind = false;
        isMovableWithBind = false;
        AllDoesntHaveBsetCard = false;
        movableAfterSecondRoll = true;
        controlPlayerIndexWithOrder = 0;
        controlPlayer = DontDestroyObjects.instance.playerListWithOrder[0];
        controlPlayerNameText.text = "현재 차례: " + controlPlayer.NickName;
        for (int i = 0; i < PhotonNetwork.CurrentRoom.PlayerCount;i++)
        {
            player[i].correctCount = 4;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        ShowFullMap();
    }

    // Update is called once per frame
    void Update()
    {
        
        testTMP.text = "";

        
        //게임 종료
        if (isOver)
            return;
        for (int i = 0; i < PhotonNetwork.CurrentRoom.PlayerCount; i++)
        {
            List<DontDestroyObjects.items> test = DontDestroyObjects.instance.playerItems[i];
            testTMP.text += i + " : ";
            for (int j = 0; j < test.Count; j++)
            {
                testTMP.text += test[j].ToString() + " ";
            }
            testTMP.text += "\n";
        }
        //when to stop moving
        if (player[controlPlayerIndexWithOrder].curIndex > playerStartPoint[controlPlayerIndexWithOrder] + newDiceSide)
        {
            player[controlPlayerIndexWithOrder].movingAllowed = false;
            playerStartPoint[controlPlayerIndexWithOrder] = player[controlPlayerIndexWithOrder].curIndex - 1;
            CheckPlayersPosition(controlPlayerIndexWithOrder);
            if (secondRoll)
            {
                if (isMovableWithBind == false)
                {
                    secondRoll = false;
                    finishRound = true;
                    UISmaller();
                }
            }

            //정답 5번이면 주사위 한번 더 굴리기
            if (player[GameManager.instance.controlPlayerIndexWithOrder].correctCount == 5)
            {
                player[GameManager.instance.controlPlayerIndexWithOrder].correctCount = 0;
                movableAfterSecondRoll = false;
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
                    if (movableAfterSecondRoll == true)
                    {
                        updatePlayerInformationUI(controlPlayerIndexWithOrder);
                        RpcManager.instance.bindPlayerIndexes.Sort();
                        RpcManager.instance.isMovableWithBind = true;
                    }
                }
                else if (!secondRoll)
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
            RpcManager.instance.isSomeoneUseCardSteal = false;
            if (controlPlayer == PhotonNetwork.LocalPlayer)
            {
                updatePlayerInformationUI(controlPlayerIndexWithOrder);
            }
            controlPlayerIndexWithOrder++;
            if (controlPlayerIndexWithOrder == initialPlayerNum)
            {
                controlPlayerIndexWithOrder = 0;
            }
            controlPlayer = DontDestroyObjects.instance.playerListWithOrder[controlPlayerIndexWithOrder];
            Invoke("RoundStart", 1);
        }
    }

    public void CheckPlayersPosition(int index)
    {
        bool isSamePos = false;
        float highestYPos = player[index].transform.position.y;

        for (int i = 0; i < PhotonNetwork.CurrentRoom.PlayerCount; i++)
        {
            if (i != index)
            {
                if (player[index].transform.position.x == player[i].transform.position.x)
                {
                    isSamePos = true;
                    if (highestYPos <= player[i].transform.position.y)
                        highestYPos = player[i].transform.position.y;
                }
            }
        }
        if(isSamePos)
            player[index].transform.position = new Vector3(player[index].transform.position.x, highestYPos + 2.0f, player[index].transform.position.z);
    }

    public void ShowFullMap()
    {
        curCamPos = Camera.main.transform.position;
        Camera.main.orthographicSize = 100;
        StartCoroutine(ShowFullMapRoutine());
    }

    IEnumerator ShowFullMapRoutine()
    {
        float journeyLength = Vector3.Distance(minCamPos, maxCamPos);
        float startTime = Time.time;
        float distanceCovered = 0f;

        while (distanceCovered < journeyLength)
        {
            float distancePerFrame = (Time.time - startTime) * 30.0f; // 시간 간격으로 변경
            float fractionOfJourney = distancePerFrame / journeyLength;
            Camera.main.transform.position = Vector3.Lerp(minCamPos, maxCamPos, fractionOfJourney);
            distanceCovered = Vector3.Distance(Camera.main.transform.position, minCamPos);
            yield return null;
        }

        yield return new WaitForSeconds(1f);
        Camera.main.transform.position = curCamPos;
        Camera.main.orthographicSize = 60;
        gameStart = true;
        diceAndSoundPanel.SetActive(true); //임시
        Invoke("RoundStart", 1.5f);
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

        if (checkControlPlayerOut() == true)
        {
            controlPlayerIndexWithOrder++;
            if (controlPlayerIndexWithOrder == initialPlayerNum)
            {
                controlPlayerIndexWithOrder = 0;
                for (int i = 0; i < initialPlayerNum; i++)
                {
                    if (checkControlPlayerOut() == true)
                    {
                        controlPlayerIndexWithOrder++;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            controlPlayer = DontDestroyObjects.instance.playerListWithOrder[controlPlayerIndexWithOrder];
        }
        controlPlayerNameText.text = "현재 차례: " + controlPlayer.NickName;
        nextTurn = false;
        StartCoroutine(UIBiggerRoutine(true));
        currentTurnASetItem = 0;
        currentTurnBSetItem = 0;
        AllDoesntHaveBsetCard = false;
        RpcManager.instance.currentTurnUsedItemOfLocalPlayer = "";
        RpcManager.instance.isSomeoneUseCardSteal = false;
        movableAfterSecondRoll = true;
        if (correctCount != 5)
            secondRoll = false;
        player[controlPlayerIndexWithOrder].moveLadder = false;
        finishRound = false;
        StartCoroutine(RoundStartRoutine());
    }

    public bool checkControlPlayerOut()
    {
        if (quitInTheMiddle.instance.outPlayerIndex.Contains(controlPlayerIndexWithOrder))
        {
            return true;
        }
        else
        {
            return false;
        }
    }


    IEnumerator RoundStartRoutine()
    {
        startRoundText.text = controlPlayer.NickName + "님 플레이를 시작합니다."; //문구 수정
        SoundManager.instance.SoundPlayer("ShowPanel1");
        startRoundPanel.SetActive(true);
        
        yield return new WaitForSeconds(1.5f);

        startRoundPanel.SetActive(false);

        yield return new WaitForSeconds(1f);

        RpcManager.instance.setDiceTrue();
        timerOn = true;
    }

    

    IEnumerator RollDiceAndGetItem()
    {
        movableAfterSecondRoll = false;
        secondRoll = true;
        if (DontDestroyObjects.instance.playerItems[controlPlayerIndexWithOrder].Count < 4) //가지고있는 아이템 개수가 3개이하일때 아이템 한장 추가 획득
        {
            spaceText.text = "랜덤한 아이템 하나를 추가로 획득합니다!";
            SoundManager.instance.SoundPlayer("ShowPanel1");
            space.SetActive(true);
            if (PhotonNetwork.LocalPlayer == controlPlayer)
            {
                int ran = Random.Range(0, 6);
                RpcManager.instance.GetAdditionalItem(ran);
            }

            yield return new WaitForSeconds(1f);
        }
        spaceText.text = "주사위를 한번 더 굴릴 수 있습니다!";
        space.SetActive(true);
        SoundManager.instance.SoundPlayer("ShowPanel1");

        yield return new WaitForSeconds(1.5f);
        space.SetActive(false);
        ResetGaugeImg();

        yield return new WaitForSeconds(1f);
        //diceImg.SetActive(true);
        diceTimer.gameObject.SetActive(true);
        timerOn = true;
        movableAfterSecondRoll = true;

    }

    public void MovePlayer()
    {
        player[controlPlayerIndexWithOrder].movingAllowed = true;
        nowMovingPlayerIndex = controlPlayerIndexWithOrder;
    }

    public void moveBindPlayer()
    {
        int firstBindIndex = RpcManager.instance.bindPlayerIndexes[0];
        player[firstBindIndex].movingAllowed = true;
        nowMovingPlayerIndex = bindPlayerIndex;
    }
        
    //check
    public int getPlayerNextPosition()
    {
        if (player[controlPlayerIndexWithOrder].curIndex == 0)
        {
            return player[controlPlayerIndexWithOrder].curIndex + newDiceSide;
        }
        else
        {
            return player[controlPlayerIndexWithOrder].curIndex + newDiceSide - 1;
        }
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
            case 101:
            case 102:
            case 103:
            case 104:
            case 105:
            case 106:
                spaceCategory = "Finish";
                spaceText.text = "도착지점에 도달했습니다.";
                break;
            default:
                spaceCategory = "Problem";
                spaceText.text = "현재 칸은 문제 칸입니다.\n 문제를 맞추면 이동가능합니다.";
                break;
        }
        space.SetActive(true);
        SoundManager.instance.SoundPlayer("ShowPanel1");
        StartCoroutine(DoActionRoutine());
        
    }

    IEnumerator DoActionRoutine()
    {
        yield return new WaitForSeconds(1.5f);
        space.SetActive(false);

        if(spaceCategory == "Problem")
        {
            activeItemUsePanel();
            yield return new WaitForSeconds(1.5f);
            if (AllDoesntHaveBsetCard == false)
            {
                yield return new WaitForSeconds(4.5f);
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

    public void EndGame(int winnerIndex)
    {
        winner = DontDestroyObjects.instance.playerListWithOrder[winnerIndex].NickName;
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
        SoundManager.instance.SoundPlayer("Button");
        PhotonNetwork.LeaveRoom();
        Destroy(DontDestroyObjects.instance);
        quitScript.isApplicationQuit = true;
    }

    public override void OnLeftRoom()
    {
        if (SceneManager.GetActiveScene().name == "SampleScene")
        {
            SceneManager.LoadScene("Main");
        }
    }

    public void QuitGame()
    {
        SoundManager.instance.SoundPlayer("Button");
        quitScript.isApplicationQuit = true;
        PhotonNetwork.LeaveRoom();
        Application.Quit();
    }

    public void CheckIncorrectProblems()
    {
        
        SoundManager.instance.SoundPlayer("Button");
        //수정
        for(int i = 0; i < PhotonNetwork.CurrentRoom.MaxPlayers; i++)
        {
            if(DontDestroyObjects.instance.playerListWithOrder[i].NickName == PhotonNetwork.LocalPlayer.NickName)
            {
                Debug.Log("incorrect");
                checkIncorrectProblemSc.ShowIncorrectProblems(i);
            }
        }
        Debug.Log("incorrect11");
    }


    public void updatePlayerInformationUI(int controlPlayerIndex)
    {
        playerInformationUIs[controlPlayerIndex].GetComponent<playerInformationUI>().updatePlayerBoardNum(player[controlPlayerIndex].curIndex - 1);
    }


    void setLocalPlayerIndexWithOrder()
    {
        for (int i = 0; i < initialPlayerNum; i++)
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
        SoundManager.instance.SoundPlayer("ShowPanel2");
    }

    public void activeItemUseResultPanel()
    {
        itemUseResultPanel.SetActive(true);
        SoundManager.instance.SoundPlayer("ShowPanel2");
    }

    
    public void eraseItemUI(int playerIndex, int cardIndex)
    {
        GameObject itemPanel = playerInformationUIs[controlPlayerIndexWithOrder].transform.GetChild(1).gameObject;
        GameObject stolenCard = itemPanel.transform.GetChild(cardIndex).gameObject;
        string itemName = stolenCard.GetComponent<Image>().sprite.name;
        Destroy(stolenCard);

        if (itemName == "운명공동체 UI")
        {
            setItemImage(DontDestroyObjects.items.bind, playerIndex, 5);
        }
        else if (itemName == "카드 빼앗기")
        {
            setItemImage(DontDestroyObjects.items.cardSteal, playerIndex, 3);
        }
        else if (itemName == "시간 빼앗기 UI")
        {
            setItemImage(DontDestroyObjects.items.timeSteal, playerIndex, 4);
        }
        else if (itemName == "힌트 UI")
        {
            setItemImage(DontDestroyObjects.items.hint, playerIndex, 0);
        }
        else if (itemName == "선택지 지우기 UI")
        {
            setItemImage(DontDestroyObjects.items.erase, playerIndex, 1);
        }
        else
        {
            setItemImage(DontDestroyObjects.items.pass, playerIndex, 2);
        }

        // 이 밑에는 최종 버전 때 지우기
        for (int i = 0; i < PhotonNetwork.CurrentRoom.PlayerCount; i++)
        {
            List<DontDestroyObjects.items> test = DontDestroyObjects.instance.playerItems[i];
            testTMP.text += i + " : ";
            for (int j = 0; j < test.Count; j++)
            {
                testTMP.text += test[j].ToString() + " ";
            }
            testTMP.text += "\n";
        }
        //
    }

    void setItemImage(DontDestroyObjects.items item, int playerIndex, int index)
    {
        GameObject itemUIPrefab = Resources.Load<GameObject>("Prefabs/itemImageUI");

        DontDestroyObjects.instance.playerItems[controlPlayerIndexWithOrder].Remove(item);
        DontDestroyObjects.instance.playerItems[playerIndex].Add(item);
        GameObject createdItem = Instantiate(itemUIPrefab);
        createdItem.transform.SetParent(playerInformationUIs[playerIndex].transform.GetChild(1), false);
        createdItem.GetComponent<Image>().sprite = itemSmallSprites[index];
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
        SoundManager.instance.SoundPlayer("ChangeClothes");
        switch (age)
        {
            case "삼국시대":
                if (nowMovingPlayerIndex == 0)
                {
                    player[nowMovingPlayerIndex].gameObject.GetComponent<SpriteRenderer>().sprite = player1Clothes[1];
                }
                else if (nowMovingPlayerIndex == 1)
                {
                    player[nowMovingPlayerIndex].gameObject.GetComponent<SpriteRenderer>().sprite = player2Clothes[1];
                }
                else if (nowMovingPlayerIndex == 2)
                {
                    player[nowMovingPlayerIndex].gameObject.GetComponent<SpriteRenderer>().sprite = player3Clothes[1];
                }
                else if (nowMovingPlayerIndex == 3)
                {
                    player[nowMovingPlayerIndex].gameObject.GetComponent<SpriteRenderer>().sprite = player4Clothes[1];
                }

                break;
            case "고려시대":
                if (nowMovingPlayerIndex == 0)
                {
                    player[nowMovingPlayerIndex].gameObject.GetComponent<SpriteRenderer>().sprite = player1Clothes[2];
                }
                else if (nowMovingPlayerIndex == 1)
                {
                    player[nowMovingPlayerIndex].gameObject.GetComponent<SpriteRenderer>().sprite = player2Clothes[2];
                }
                else if (nowMovingPlayerIndex == 2)
                {
                    player[nowMovingPlayerIndex].gameObject.GetComponent<SpriteRenderer>().sprite = player3Clothes[2];
                }
                else if (nowMovingPlayerIndex == 3)
                {
                    player[nowMovingPlayerIndex].gameObject.GetComponent<SpriteRenderer>().sprite = player4Clothes[2];
                }
                break;
            case "조선시대":
                if (nowMovingPlayerIndex == 0)
                {
                    player[nowMovingPlayerIndex].gameObject.GetComponent<SpriteRenderer>().sprite = player1Clothes[3];
                }
                else if (nowMovingPlayerIndex == 1)
                {
                    player[nowMovingPlayerIndex].gameObject.GetComponent<SpriteRenderer>().sprite = player2Clothes[3];
                }
                else if (nowMovingPlayerIndex == 2)
                {
                    player[nowMovingPlayerIndex].gameObject.GetComponent<SpriteRenderer>().sprite = player3Clothes[3];
                }
                else if (nowMovingPlayerIndex == 3)
                {
                    player[nowMovingPlayerIndex].gameObject.GetComponent<SpriteRenderer>().sprite = player4Clothes[3];
                }
                break;
            case "근현대":
                if (nowMovingPlayerIndex == 0)
                {
                    player[nowMovingPlayerIndex].gameObject.GetComponent<SpriteRenderer>().sprite = player1Clothes[4];
                }
                else if (nowMovingPlayerIndex == 1)
                {
                    player[nowMovingPlayerIndex].gameObject.GetComponent<SpriteRenderer>().sprite = player2Clothes[4];
                }
                else if (nowMovingPlayerIndex == 2)
                {
                    player[nowMovingPlayerIndex].gameObject.GetComponent<SpriteRenderer>().sprite = player3Clothes[4];
                }
                else if (nowMovingPlayerIndex == 3)
                {
                    player[nowMovingPlayerIndex].gameObject.GetComponent<SpriteRenderer>().sprite = player4Clothes[4];
                }
                break;
            default:
                if (nowMovingPlayerIndex == 0)
                {
                    player[nowMovingPlayerIndex].gameObject.GetComponent<SpriteRenderer>().sprite = player1Clothes[0];
                }
                else if (nowMovingPlayerIndex == 1)
                {
                    player[nowMovingPlayerIndex].gameObject.GetComponent<SpriteRenderer>().sprite = player2Clothes[0];
                }
                else if (nowMovingPlayerIndex == 2)
                {
                    player[nowMovingPlayerIndex].gameObject.GetComponent<SpriteRenderer>().sprite = player3Clothes[0];
                }
                else if (nowMovingPlayerIndex == 3)
                {
                    player[nowMovingPlayerIndex].gameObject.GetComponent<SpriteRenderer>().sprite = player4Clothes[0];
                }
                break;

        }

    }

    public void Flip(bool isFlip)
    {
        player[nowMovingPlayerIndex].gameObject.GetComponent<SpriteRenderer>().flipX = isFlip;
    }


    public void CloseNoIncorrectPanel()
    {
        SoundManager.instance.SoundPlayer("Button");
        noIncorrectText.SetActive(false);
    }

}

