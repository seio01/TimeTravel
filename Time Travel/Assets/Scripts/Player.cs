using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Transform[] points;
    public float speed = 15f;
    public bool movingAllowed;
    public bool transporting;
    public int curIndex;
    public int ran;


    void Start()
    {
        transform.position = points[curIndex].transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (movingAllowed)
            MovePath();
        if (transporting)
        {
            Transport();
        }
            
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

    //»ç´Ù¸®Ä­
    //¹®Á¦Ä­
    //Æ÷ÅÐÄ­

    public void Transport()
    {
        switch (curIndex - 1)
        {
            case 6:
                transform.position = Vector2.MoveTowards(transform.position, points[35].transform.position, speed * Time.deltaTime);
                GameManager.playerStartPoint = 35;
                break;
            case 14:
                transform.position = points[29].transform.position;
                GameManager.playerStartPoint = 29;
                break;

        }
        
    }
}
