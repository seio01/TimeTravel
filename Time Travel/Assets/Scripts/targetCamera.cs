using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class targetCamera : MonoBehaviour
{

    public float smoothing = 0.2f;
    public Vector2 minCameraBoundary;
    public Vector2 maxCameraBoundary;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Player player = GameManager.instance.player[GameManager.instance.controlPlayerIndexWithOrder];
        Vector3 targetPos = new Vector3(player.gameObject.transform.position.x, player.gameObject.transform.position.y, this.transform.position.z);
        targetPos.x = Mathf.Clamp(targetPos.x, minCameraBoundary.x, maxCameraBoundary.x);
        targetPos.y = Mathf.Clamp(targetPos.y, minCameraBoundary.y, maxCameraBoundary.y);

        transform.position = Vector3.Lerp(transform.position, targetPos, smoothing);
    }

    /*private void LateUpdate()
    {
        Player player = GameManager.instance.player[GameManager.instance.controlPlayerIndexWithOrder];
        Vector3 targetPos = new Vector3(player.gameObject.transform.position.x, player.gameObject.transform.position.y, this.transform.position.z);
        transform.position = targetPos;
    }*/
}
