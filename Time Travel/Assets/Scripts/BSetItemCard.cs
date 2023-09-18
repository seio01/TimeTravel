using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class BSetItemCard : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    bool isUsed;
    bool canNotUse;
    public GameObject textPanel;
    public TMP_Text itemText;

    public BsetItemUsePanel panelScript;
    // Start is called before the first frame update
    void Start()
    {
        string spriteName = this.gameObject.GetComponent<Image>().sprite.name;
        textPanel = transform.parent.parent.GetChild(3).gameObject;
        itemText = textPanel.transform.GetChild(0).GetComponent<TMP_Text>();

        //setItemText();
        if (spriteName == "카드빼앗기" && canStealCard() == false)
        {
            changeColorBlack();
            canNotUse = true;
        }
        else
        {
            isUsed = false;
            canNotUse = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.currentTurnBSetItem > 0)
        {
            canNotUse = true;
            changeColorBlack();
        }
        if (RpcManager.instance.isSomeoneUseCardSteal == true)
        {
            canNotUse = true;
            changeColorBlack();
        }
    }

    void OnDisable()
    {
        Destroy(this.gameObject);
    }

    void setItemText()
    {
        string spriteName = this.gameObject.GetComponent<Image>().sprite.name;
        if (spriteName == "카드빼앗기")
        {
            itemText.text = "상대방의 카드 1장을 빼앗아 가져옵니다.\n상대방의 카드가 없거나 내 카드가 4장이면 쓸 수 없습니다.\n";
        }
        else if (spriteName == "운명공동체")
        {
            itemText.text = "상대방이 문제를 맞힌다면 나도 주사위 눈에 해당하는 칸만큼 나아갑니다.\n";
        }
        else
        {
            itemText.text = "상대방이 문제 푸는 시간을 줄일 수 있습니다. \n ox 문제는 8초, 4지선다 문제는 15초로 줄어듭니다.\n";
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        setItemText();
        textPanel.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        textPanel.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isUsed == true || canNotUse==true || GameManager.instance.currentTurnBSetItem > 0)
        {
            return;
        }
        string spriteName = this.gameObject.GetComponent<Image>().sprite.name;
        changeColorBlack();
        isUsed = true;
        RpcManager.instance.currentTurnUsedItemOfLocalPlayer = spriteName;
        if (spriteName == "운명공동체")
        {
            RpcManager.instance.useBsetItemCard(DontDestroyObjects.items.bind);
            RpcManager.instance.makeIsUsedBindTrue(GameManager.instance.localPlayerIndexWithOrder);
        }
        else if (spriteName == "카드빼앗기")
        {
            if (RpcManager.instance.isSomeoneUseCardSteal == false)
            {
                RpcManager.instance.useBsetItemCard(DontDestroyObjects.items.cardSteal);
            }
            else
            {
                return;
            }

        }
        else
        {
            RpcManager.instance.useBsetItemCard(DontDestroyObjects.items.timeSteal);
            RpcManager.instance.setIsThisTurnTimeStealTrue();
        }
        GameManager.instance.currentTurnBSetItem++;
    }

    bool canStealCard()
    {
        if (DontDestroyObjects.instance.playerItems[GameManager.instance.localPlayerIndexWithOrder].Count == 4 || DontDestroyObjects.instance.playerItems[GameManager.instance.controlPlayerIndexWithOrder].Count == 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    void changeColorBlack()
    {
        Color usedColor = new Color(100 / 255f, 100 / 255f, 100 / 255f);
        this.gameObject.GetComponent<Image>().color = usedColor;
    }
}
