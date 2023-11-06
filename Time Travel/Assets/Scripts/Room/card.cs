using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;

public class card : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject textPanel;
    public TMP_Text cardDescriptionText;

    // Start is called before the first frame update
    void Start()
    {
        textPanel.gameObject.SetActive(false);
        setItemText();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void setItemText()
    {
        Sprite itemSprite = GetComponent<Image>().sprite;
        if (itemSprite == RoomManager.instance.itemImg[0])
        {
            cardDescriptionText.text = "모르는 문제의 힌트를 얻을 수 있습니다. \n";
        }
        else if (itemSprite == RoomManager.instance.itemImg[1])
        {
            cardDescriptionText.text = "모르는 문제를 패스할 수 있습니다.\n문제는 정답 처리되고 모든 사람에게 답이 공개됩니다.";
        }
        else if (itemSprite == RoomManager.instance.itemImg[2])
        {
            cardDescriptionText.text = "오답 중 하나의 선택지를 제거합니다.\n4지선다형 문제에만 사용이 가능합니다.";
        }
        else if (itemSprite == RoomManager.instance.itemImg[3])
        {
            cardDescriptionText.text = "상대방이 문제 푸는 시간을 줄일 수 있습니다.\n상대방의 턴 도중, 아이템 선택 시간에만\n이 카드를 쓸 수 있습니다.";
        }
        else if (itemSprite == RoomManager.instance.itemImg[4])
        {
            cardDescriptionText.text = "상대방이 문제를 맞힌다면 자신도 \n주사위 눈에 해당하는 칸만큼 나아갑니다.\n상대방의 턴 도중, 아이템 선택 시간에만\n이 카드를 쓸 수 있습니다.";
        }
        else
        {
            cardDescriptionText.text = "상대방의 카드 1장을 빼앗아 가져옵니다.\n내 카드가 4장이면 쓸 수 없습니다.\n상대방의 턴 도중, 아이템 선택 시간에만\n이 카드를 쓸 수 있습니다.";
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        textPanel.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        textPanel.SetActive(false);
    }
}
