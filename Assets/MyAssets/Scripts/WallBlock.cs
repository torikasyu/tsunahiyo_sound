using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallBlock : MonoBehaviour
{
    //float speed = 0;
    Vector3 currentPos = Vector3.zero;
    float scrollPosX = 0;
    //float limitX = 0;

    // Start is called before the first frame update
    void Start()
    {
        //currentPos = transform.position;
        //limitX = GameManager.Instance.LeftLimitPos;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        currentPos = transform.position;
        scrollPosX = currentPos.x - GameManager.Instance.EnemyBlockSpeed * Time.deltaTime;

        if (scrollPosX <= GameManager.Instance.LeftLimitPos)
        {
            scrollPosX = GameManager.Instance.RightLimitPos;
        }

        transform.position = new Vector3(scrollPosX, currentPos.y, currentPos.z);
    }
}
