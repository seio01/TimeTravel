using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class BSetItemCard : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    bool isSelected;
    bool canNotUse;
    public GameObject textPanel;
    public TMP_Text itemText;
    public string spriteName;
    public BsetItemUsePanel panelScript;
    // Start is called before the first frame update
    void Start()
    {
        spriteName = this.gameObject.GetComponent<Image>().sprite.name;
        textPanel = transform.parent.parent.GetChild(3).gameObject;
        itemText = textPanel.transform.GetChild(0).GetComponent<TMP_Text>();
        textPanel.gameObject.SetActive(false);
        //setItemText();
        if (spriteName == "카드빼앗기" && canStealCard() == false)
        {
            changeColorBlack();
            canNotUse = true;
        }
        else
        {
            isSelected = false;
            canNotUse = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (spriteName == "카드빼앗기" && canStealCard() == false)
        {
            canNotUse = true;
            changeColorBlack();
        }
        else
        {
            if (isSelected == false && RpcManager.instance.currentTurnUsedItemOfLocalPlayer != "")
            {
                canNotUse = true;
                changeColorBlack();
            }
            if (isSelected == false && RpcManager.instance.currentTurnUsedItemOfLocalPlayer == "")
            {
                canNotUse = false;
                changeColorWhite();
            }
            if (RpcManager.instance.isSomeoneUseCardSteal == true)
            {
                canNotUse = true;
                changeColorBlack();
            }
        }
    }

    void OnDisable()
    {
        if (isSelected == true)
        {
            if (spriteName == "운명공동체")
            {
                RpcManager.instance.useBsetItemCard(DontDestroyObjects.items.bind);
                RpcManager.instance.makeIsUsedBindTrue(GameManager.instance.localPlayerIndexWithOrder);
            }
            else if (spriteName == "카드빼앗기")
            {
                RpcManager.instance.useBsetItemCard(DontDestroyObjects.items.cardSteal);
            }
            else
            {
                RpcManager.instance.useBsetItemCard(DontDestroyObjects.items.timeSteal);
                RpcManager.instance.setIsThisTurnTimeStealTrue();
            }
        }
        Destroy(this.gameObject);
    }

    void setItemText()
    {
        string spriteName = this.gameObject.GetComponent<Image>().sprite.name;
        if (spriteName == "카드빼앗기")
        {
            itemText.text = "상대방의 카드 1장을 빼앗아 가져옵니다.\n자신의 카드가 4장이면 쓸 수 없습니다.\n";
        }
        else if (spriteName == "운명공동체")
        {
            itemText.text = "상대방이 문제를 맞힌다면 자신도 \n주사위 눈에 해당하는 칸만큼 이동합니다.\n";
        }
        else
        {
            itemText.text = "상대방의 문제 푸는 시간을 줄일 수 있습니다. \n ox 문제는 8초, 4지선다 문제는 25초로 줄어듭니다.\n";
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
        if (isSelected== true)
        {
            changeColorWhite();
            isSelected = false;
            transform.GetChild(0).gameObject.SetActive(false);
            RpcManager.instance.currentTurnUsedItemOfLocalPlayer = "";
            if (spriteName == "카드빼앗기")
            {
                RpcManager.instance.setCardStealBool();
            }
            return;
        }
        if (canNotUse == true || RpcManager.instance.currentTurnUsedItemOfLocalPlayer != "")
        {
            return;
        }
        Color usedColor = new Color(200 / 255f, 200 / 255f, 200 / 255f);
        this.gameObject.GetComponent<Image>().color = usedColor;
        isSelected= true;
        transform.GetChild(0).gameObject.SetActive(true);
        RpcManager.instance.currentTurnUsedItemOfLocalPlayer = spriteName;
        if (spriteName == "카드빼앗기")
        {
            RpcManager.instance.setCardStealBool();
        }
    }

    bool canStealCard()
    {
        List<DontDestroyObjects.items> playerCards = DontDestroyObjects.instance.playerItems[GameManager.instance.localPlayerIndexWithOrder];
        List<DontDestroyObjects.items> controlPlayerCards = DontDestroyObjects.instance.playerItems[GameManager.instance.controlPlayerIndexWithOrder];
        if (playerCards.Count == 4 || controlPlayerCards.Count == 0)
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

    void changeColorWhite()
    {
        Color whiteColor = new Color(255 / 255f, 255 / 255f, 255 / 255f);
        this.gameObject.GetComponent<Image>().color = whiteColor;
    }
}
