using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    public GameObject explanationPanel;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowExplanation()
    {
        explanationPanel.SetActive(true);
    }
    public void gameExit()
    {
        Application.Quit();
    }
}
