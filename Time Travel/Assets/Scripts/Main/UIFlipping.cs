using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIFlipping : MonoBehaviour
{
    public GameObject[] pages;
    public int currentPage = 0;
    public int totalPage;
    public Button leftBtn;
    public Button rightBtn;

    // Start is called before the first frame update
    void Start()
    {
        totalPage = 4;
        leftBtn.onClick.AddListener(() => { PrevPage(); });
        rightBtn.onClick.AddListener(() => { NextPage(); });
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PrevPage()
    {
        if (currentPage == 0)
        {
            return;
        }
        
        pages[currentPage].SetActive(false);
        currentPage--;
        pages[currentPage].SetActive(true);
        if (currentPage == 0)
        {
            leftBtn.interactable = false;
        }
        else
            leftBtn.interactable = true;
    }

    public void NextPage()
    {
        if(currentPage == totalPage-1)
        {
            return;
        }
        pages[currentPage].SetActive(false);
        currentPage++;
        pages[currentPage].SetActive(true);
        if (currentPage == totalPage-1)
        {
            rightBtn.interactable = false;
        }
        else
            rightBtn.interactable = true;

    }
}
