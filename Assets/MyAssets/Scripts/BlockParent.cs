using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockParent : MonoBehaviour
{
    public GameObject[] members;
    [HideInInspector]
    public int LuckCount = int.MaxValue;

    Vector3 pos = Vector3.zero;
    float speed { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        pos = transform.position;
    }

    [SerializeField]
    Material lightMat;
    [SerializeField]
    Material normalMatForBlock;
    [SerializeField]
    Material normalMatForTarget;
    [SerializeField]
    Material brokenMatForTarget;

    bool isDestroyCommanded = false;
    int beatCount = 0;
    int thisLoopBeat = 0;
    int prevBeat = 0;

    // Update is called once per frame
    void Update()
    {
        beatCount = GameManager.Instance.beatCount;
        thisLoopBeat = beatCount % 64;  //TODO:可変化
        if (thisLoopBeat != prevBeat)
        {
            if (members[thisLoopBeat].CompareTag("Dummy"))
            {
                if (members[thisLoopBeat].GetComponent<DummyBlock>().isDeleted)
                {
                    //既に壊されているので色を変えるだけ
                    members[thisLoopBeat].GetComponent<Renderer>().material = lightMat;
                }
                else
                {
                    //まだ壊されていないので色を変えて破壊可能とする
                    members[thisLoopBeat].GetComponent<Renderer>().material = lightMat;
                    members[thisLoopBeat].GetComponent<DummyBlock>().canDelete = true;
                }
            }
            else
            {
                members[thisLoopBeat].GetComponent<Renderer>().material = lightMat;
            }

            if (members[prevBeat].CompareTag("Dummy"))
            {
                if (members[prevBeat].GetComponent<DummyBlock>().isDeleted)
                {
                    //既に壊されているので色だけ戻す
                    members[prevBeat].GetComponent<Renderer>().material = brokenMatForTarget;
                }
                else
                {
                    members[prevBeat].GetComponent<DummyBlock>().canDelete = false;
                    members[prevBeat].GetComponent<Renderer>().material = normalMatForTarget;
                }
            }
            else
            {
                members[prevBeat].GetComponent<Renderer>().material = normalMatForBlock;
            }
            prevBeat = thisLoopBeat;
        }

        speed = GameManager.Instance.EnemyBlockSpeed;

        if (pos.x < GameManager.Instance.LeftLimitPos) Destroy(gameObject);

        if (!isDestroyCommanded)
        {
            pos = new Vector3(pos.x - speed * Time.deltaTime, pos.y, pos.z);
            transform.position = pos;

            if (LuckCount <= 0 && !isDestroyCommanded)
            {
                DoDestroy();
                isDestroyCommanded = true;
            }
        }
    }

    void DoDestroy()
    {
        GameManager.Instance.AddScore(1);
        Destroy(gameObject);

        //TODO:Block destroy effect
        /*
        foreach (Transform child in gameObject.transform)
        {
            child.gameObject.GetComponent<SpriteRenderer>().sprite = hit;
            Destroy(child.gameObject.GetComponent<BoxCollider2D>());
        }
        Destroy(gameObject, 0.5f);
        */
    }

}
