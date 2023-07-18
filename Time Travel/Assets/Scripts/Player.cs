using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public GameManager manager;
    public Transform[] points;

    public float speed = 15f;
    public bool movingAllowed;
    public bool moveLadder;
    public bool moveTransport;
    public int curIndex;
    public int ran;

    public List<GameManager.items> itemCards;
    void Start()
    {
        transform.position = points[curIndex].transform.position;
        itemCards = new List<GameManager.items>();
        itemCards.Add(GameManager.items.hint);
    }

    // Update is called once per frame
    void Update()
    {
        if (movingAllowed)
            MovePath();
        if (moveLadder)
            MoveLadder();
        if (moveTransport)
            Transport();

    }

    public void MovePath()
    {
        if(curIndex <= points.Length - 1)
        {
            transform.position = Vector2.MoveTowards(transform.position, points[curIndex].transform.position, speed * Time.deltaTime);
            if (transform.position == points[curIndex].transform.position)
            {
                curIndex++;
            }
        }

        
    }


    //»ç´Ù¸®Ä­ ÀÌµ¿
    public void MoveLadder()
    {

        switch (curIndex - 1)
        {
            case 7:
                transform.position = Vector2.MoveTowards(transform.position, points[36].transform.position, speed * Time.deltaTime);
                GameManager.playerStartPoint = 36;
                
                if(transform.position == points[36].transform.position)
                {
                    curIndex = 37;
                    manager.isLadder = false;
                    manager.finishRound = true;
                } 
                break;
            case 22:
                transform.position = Vector2.MoveTowards(transform.position, points[48].transform.position, speed * Time.deltaTime);
                GameManager.playerStartPoint = 48;

                if (transform.position == points[48].transform.position)
                {
                    curIndex = 49;
                    manager.isLadder = false;
                    manager.finishRound = true;
                }
                break;
            case 53:
                transform.position = Vector2.MoveTowards(transform.position, points[60].transform.position, speed * Time.deltaTime);
                GameManager.playerStartPoint = 60;

                if (transform.position == points[60].transform.position)
                {
                    curIndex = 61;
                    manager.isLadder = false;
                    manager.finishRound = true;
                }
                break;
            case 64:
                transform.position = Vector2.MoveTowards(transform.position, points[73].transform.position, speed * Time.deltaTime);
                GameManager.playerStartPoint = 73;

                if (transform.position == points[73].transform.position)
                {
                    curIndex = 74;
                    manager.isLadder = false;
                    manager.finishRound = true;
                }
                break;
            case 76:
                transform.position = Vector2.MoveTowards(transform.position, points[84].transform.position, speed * Time.deltaTime);
                GameManager.playerStartPoint = 84;

                if (transform.position == points[84].transform.position)
                {
                    curIndex = 85;
                    manager.isLadder = false;
                    manager.finishRound = true;
                }
                break;
        }

    }

    //Æ÷ÅÐÄ­ ÀÌµ¿ ¼öÁ¤ÇÊ¿ä
    public void Transport()
    {
        switch (curIndex - 1)
        {
            case 15:
                transform.position = points[30].transform.position;
                GameManager.playerStartPoint = 30;
                break;
            case 26:
                transform.position = points[38].transform.position;
                GameManager.playerStartPoint = 38;
                break;
            case 29:
                transform.position = points[15].transform.position;
                GameManager.playerStartPoint = 15;
                break;
            case 37:
                transform.position = points[26].transform.position;
                GameManager.playerStartPoint = 26;
                break;

        }
        manager.isTransport = false;
        manager.finishRound = true;
    }
}
