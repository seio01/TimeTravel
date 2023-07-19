using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Space : MonoBehaviour
{
    public GameManager manager;
    public Player player;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DoAction(string category)
    {
        switch (category)
        {
            case "Nothing":
                manager.MovePlayer();
                manager.isLadder = false;
                manager.isTransport = false;
                break;
            case "Ladder":
                manager.MovePlayer();
                manager.isLadder = true;
                break;
            case "Portal":
                manager.MovePlayer();
                manager.isTransport = true;
                break;
            case "Problem":
                //문제 푸는 부분
                manager.showProblem();
                break;
        }

    }
}
