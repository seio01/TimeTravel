using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;

public class Player : MonoBehaviour
{
    public GameManager manager;
    public Transform[] points;
    public List<int> incorrectProblemsFromDynasty1;
    public List<int> incorrectProblemsFromDynasty2;
    public List<int> incorrectProblemsFromDynasty3;
    public List<int> incorrectProblemsFromDynasty4;
    public List<int> incorrectProblemsFromDynasty5;


    public float speed = 15f;
    public bool movingAllowed;
    public bool moveLadder;
    public int curIndex;
    public int ran;
    public int correctCount;

    void Start()
    {
        //transform.position = points[curIndex].transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.isOver)
            return;
        if (movingAllowed)
            MovePath();
        if (moveLadder)
            MoveLadder();
    }

    public void MovePath()
    {

        if (curIndex <= points.Length - 1)
        {
            transform.position = Vector2.MoveTowards(transform.position, points[curIndex].transform.position, speed * Time.deltaTime);
            if (transform.position == points[curIndex].transform.position)
            {
                curIndex++;

                if (curIndex == 10)
                {
                    GameManager.instance.ChangeClothes("삼국시대");
                }
                else if (curIndex == 21)
                {
                    GameManager.instance.Flip(true);
                }
                else if (curIndex == 22)
                {
                    GameManager.instance.ChangeClothes("고려시대");

                }
                else if (curIndex == 33)
                {
                    GameManager.instance.Flip(false);
                }
                else if (curIndex == 42)
                {
                    GameManager.instance.ChangeClothes("조선시대");
                }
                else if (curIndex == 45)
                {
                    GameManager.instance.Flip(true);
                }
                else if (curIndex == 57)
                {
                    GameManager.instance.Flip(false);
                }
                else if (curIndex == 69)
                {
                    GameManager.instance.Flip(true);
                }
                else if (curIndex == 72)
                {
                    GameManager.instance.ChangeClothes("근현대");
                }
                else if (curIndex == 81)
                {
                    GameManager.instance.Flip(false);
                }
                //도착
                else if (curIndex == points.Length)
                {
                    GameManager.instance.player[GameManager.instance.controlPlayerIndexWithOrder].movingAllowed = false;
                    GameManager.instance.EndGame(GameManager.instance.controlPlayerIndexWithOrder);
                    SoundManager.instance.SoundPlayer("Finish");
                }

            }


        }

    }

    //사다리칸 이동
    public void MoveLadder()
    {
        //SoundManager.instance.SoundPlayer("Ladder"); 계단 오르는 소리 뭘로...
        switch (curIndex - 1)
        {
            case 7:
                ReachedLadderPoint(36, "고려시대", false, false);
                break;
            case 22:
                ReachedLadderPoint(48, "조선시대", false, false);
                break;
            case 53:
                GameManager.instance.Flip(false);
                ReachedLadderPoint(60, "null", false, false);
                break;
            case 64:
                GameManager.instance.Flip(true);
                ReachedLadderPoint(73, "근현대", false, true);
                break;
            case 76:
                ReachedLadderPoint(84, "null", true, false);
                break;
        }
        

    }

    public void Transport()
    {
        SoundManager.instance.SoundPlayer("Portal");
        switch (curIndex - 1)
        {
            case 15:
                ReachedTransportPoint(30, "고려시대", true, true);
                break;
            case 26:
                ReachedTransportPoint(38, "null", true, false);
                break;
            case 30:
                ReachedTransportPoint(15, "삼국시대", true, false);
                break;
            case 32:
                ReachedTransportPoint(80, "근현대", true, false);
                break;
            case 38:
                ReachedTransportPoint(26, "null", true, true);
                break;
            case 46:
                ReachedTransportPoint(67, "null", true, false);
                break;
            case 62:
                ReachedTransportPoint(90, "근현대", false, false);
                break;
            case 67:
                ReachedTransportPoint(46, "null", true, true);
                break;
            case 80:
                ReachedTransportPoint(32, "고려시대", true, false);
                break;
            case 90:
                ReachedTransportPoint(62, "조선시대", false, false);
                break;

        }
        
    }

    public void ReachedLadderPoint(int index, string age, bool needflip, bool flipState)
    {
        transform.position = Vector2.MoveTowards(transform.position, points[index].transform.position, speed * Time.deltaTime);
        GameManager.instance.playerStartPoint[GameManager.instance.controlPlayerIndexWithOrder] = index;

        if (transform.position == points[index].transform.position)
        {
            GameManager.instance.CheckPlayersPosition(GameManager.instance.controlPlayerIndexWithOrder);
            if (age != "null")
                GameManager.instance.ChangeClothes(age);
            if (needflip == true)
                GameManager.instance.Flip(flipState);
            curIndex = index + 1;
            manager.isLadder = false;
            manager.finishRound = true;
            GameManager.instance.UISmaller();
        }
    }

    public void ReachedTransportPoint(int index, string age, bool needflip, bool flipState)
    {
        transform.position = points[index].transform.position;
        GameManager.instance.playerStartPoint[GameManager.instance.controlPlayerIndexWithOrder] = index;
        if (transform.position == points[index].transform.position)
        {
            GameManager.instance.CheckPlayersPosition(GameManager.instance.controlPlayerIndexWithOrder);
            if (age != "null")
                GameManager.instance.ChangeClothes(age);
            if (needflip == true)
                GameManager.instance.Flip(flipState);
            curIndex = index + 1;
            manager.isTransport = false;
            manager.finishRound = true;
            GameManager.instance.UISmaller();
        }


    }

}
