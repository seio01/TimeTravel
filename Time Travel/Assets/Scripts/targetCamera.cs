using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class targetCamera : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void LateUpdate()
    {
        Player player = GameManager.instance.player[GameManager.instance.controlPlayerIndexWithOrder];
        Vector3 targetPos = new Vector3(player.gameObject.transform.position.x, player.gameObject.transform.position.y, this.transform.position.z);
        transform.position = targetPos;
    }
}
