using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class resultPanel : MonoBehaviour
{
    TMP_Text resultText;
    public GameObject problemCanvas;
    // Start is called before the first frame update

    void Awake()
    {
        resultText = transform.GetChild(0).gameObject.GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnEnable()
    {
        Color backGroundColor = this.gameObject.GetComponent<Image>().color;
        Color textColor = new Color(65f/255f, 39f/255f, 27f/255f);
        backGroundColor.a = 1.0f;
        textColor.a = 1.0f;
        this.gameObject.GetComponent<Image>().color = backGroundColor;
        resultText.color = textColor;
        StartCoroutine("setAlphaValue");
    }

    IEnumerator setAlphaValue()
    {
        Color backGroundColor = this.gameObject.GetComponent<Image>().color;
        Color textColor = resultText.color;
        yield return new WaitForSeconds(2.0f);
        while (backGroundColor.a >= 0)
        {
            backGroundColor.a -= 0.01f;
            textColor.a -= 0.01f;
            yield return new WaitForSeconds(0.01f);
            resultText.color = textColor;
            this.gameObject.GetComponent<Image>().color = backGroundColor;
        }
        yield return new WaitForSeconds(0.75f);
        problemCanvas.SetActive(false);
        this.gameObject.SetActive(false);
    }
    
}
