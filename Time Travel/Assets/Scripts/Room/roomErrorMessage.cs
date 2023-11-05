using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class roomErrorMessage : MonoBehaviour
{

    void OnEnable()
    {
        StartCoroutine("setTimerForPanel");
    }

    IEnumerator setTimerForPanel()
    {
        yield return new WaitForSeconds(1.5f);
        RoomManager.instance.LeaveRoom();
        this.gameObject.SetActive(false);
        yield return null;
    }
}