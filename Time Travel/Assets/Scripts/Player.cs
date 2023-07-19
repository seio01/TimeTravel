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
    public int curIndex;
    public int ran;

    public List<GameManager.items> itemCards;
    void Start()
    {
        transform.position = points[curIndex].transform.position;
        itemCards = new List<GameManager.items>();
        itemCards.Add(GameManager.items.pass);
    }

    // Update is called once per frame
    void Update()
    {
        if (movingAllowed)
            MovePath();
        if (moveLadder)
            MoveLadder();

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
                    curIndex = 36;
                    manager.isLadder = false;
                    manager.finishRound = true;
                } 
                break;
            case 22:
                transform.position = Vector2.MoveTowards(transform.position, points[48].transform.position, speed * Time.deltaTime);
                GameManager.playerStartPoint = 48;

                if (transform.position == points[48].transform.position)
                {
                    curIndex = 48;
                    manager.isLadder = false;
                    manager.finishRound = true;
                }
                break;
            case 53:
                transform.position = Vector2.MoveTowards(transform.position, points[60].transform.position, speed * Time.deltaTime);
                GameManager.playerStartPoint = 60;

                if (transform.position == points[60].transform.position)
                {
                    curIndex = 60;
                    manager.isLadder = false;
                    manager.finishRound = true;
                }
                break;
            case 64:
                transform.position = Vector2.MoveTowards(transform.position, points[73].transform.position, speed * Time.deltaTime);
                GameManager.playerStartPoint = 73;

                if (transform.position == points[73].transform.position)
                {
                    curIndex = 73;
                    manager.isLadder = false;
                    manager.finishRound = true;
                }
                break;
            case 76:
                transform.position = Vector2.MoveTowards(transform.position, points[84].transform.position, speed * Time.deltaTime);
                GameManager.playerStartPoint = 84;

                if (transform.position == points[84].transform.position)
                {
                    curIndex = 84;
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
                if (transform.position == points[30].transform.position)
                {
                    curIndex = 31;
                    manager.isTransport = false;
                    manager.finishRound = true;
                }
                break;
            case 26:
                transform.position = points[38].transform.position;
                GameManager.playerStartPoint = 38;
                if (transform.position == points[38].transform.position)
                {
                    curIndex = 39;
                    manager.isTransport = false;
                    manager.finishRound = true;
                }
                break;
            case 30:
                transform.position = points[15].transform.position;
                GameManager.playerStartPoint = 15;
                if (transform.position == points[15].transform.position)
                {
                    curIndex = 16;
                    manager.isTransport = false;
                    manager.finishRound = true;
                }
                break;
            case 32:
                transform.position = points[80].transform.position;
                GameManager.playerStartPoint = 80;
                if (transform.position == points[80].transform.position)
                {
                    curIndex = 81;
                    manager.isTransport = false;
                    manager.finishRound = true;
                }
                break;
            case 38:
                transform.position = points[26].transform.position;
                GameManager.playerStartPoint = 26;
                if (transform.position == points[26].transform.position)
                {
                    curIndex = 27;
                    manager.isTransport = false;
                    manager.finishRound = true;
                }
                break;
            case 46:
                transform.position = points[46].transform.position;
                GameManager.playerStartPoint = 46;
                if (transform.position == points[46].transform.position)
                {
                    curIndex = 47;
                    manager.isTransport = false;
                    manager.finishRound = true;
                }
                break;
            case 62:
                transform.position = points[62].transform.position;
                GameManager.playerStartPoint = 62;
                if (transform.position == points[62].transform.position)
                {
                    curIndex = 63;
                    manager.isTransport = false;
                    manager.finishRound = true;
                }
                break;
            case 67:
                transform.position = points[67].transform.position;
                GameManager.playerStartPoint = 67;
                if (transform.position == points[67].transform.position)
                {
                    curIndex = 68;
                    manager.isTransport = false;
                    manager.finishRound = true;
                }
                break;
            case 80:
                transform.position = points[80].transform.position;
                GameManager.playerStartPoint = 80;
                if (transform.position == points[80].transform.position)
                {
                    curIndex = 81;
                    manager.isTransport = false;
                    manager.finishRound = true;
                }
                break;
            case 90:
                transform.position = points[90].transform.position;
                GameManager.playerStartPoint = 90;
                if (transform.position == points[90].transform.position)
                {
                    curIndex = 91;
                    manager.isTransport = false;
                    manager.finishRound = true;
                }
                break;
           
        }
    }
}
