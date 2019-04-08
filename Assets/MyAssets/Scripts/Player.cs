using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Player : MonoBehaviour
{
    public GameObject blockA;

    bool canMove = true;
    GameObject HoldBallet;
    bool canShoot()
    {
        return GameManager.Instance.canShoot;
    }
    int TopLimitPos;
    int BottomLimitPos;
    int ZLimit;

    // Start is called before the first frame update
    void Start()
    {
        //HoldBallet = transform.Find("BulletBlock").gameObject;
        TopLimitPos = GameManager.Instance.YLimit;
        BottomLimitPos = 0;
        ZLimit = GameManager.Instance.ZLimit;
    }

    bool move = false;
    float posY = 0;
    float posZ = 0;

    bool isDead = false;
    // Update is called once per frame
    void Update()
    {
        posY = transform.position.y;
        posZ = transform.position.z;

        if (canMove && !isDead)
        {
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            {
                move = true;
                posY++;
                if (posY > TopLimitPos)
                {
                    posY = TopLimitPos;
                    move = false;
                }
            }
            else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            {
                move = true;
                posY--;
                if (posY < BottomLimitPos)
                {
                    posY = BottomLimitPos;
                    move = false;
                }
            }
            else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            {
                move = true;
                posZ++;
                if (posZ > ZLimit)
                {
                    posZ = ZLimit;
                    move = false;
                }
            }
            else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            {
                move = true;
                posZ--;
                if (posZ < -ZLimit)
                {
                    posZ = -ZLimit;
                    move = false;
                }
            }

            if (move)
            {
                canMove = false;
                transform.DOMove(new Vector3(transform.position.x, posY, posZ), 0.1f).OnComplete(() =>
                   {
                       canMove = true;
                   });
            }
        }

        if (canShoot() && !isDead)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                var pos = transform.position;
                pos = new Vector3(pos.x + 1, Snap(pos.y), pos.z);
                Instantiate(blockA, pos, Quaternion.identity);
                GameManager.Instance.canShoot = false;
                GameManager.Instance.PlaySound(0);
            }
        }
        if (!isDead)
        {
            //HoldBallet.SetActive(canShoot());
        }
        else
        {
            //HoldBallet.SetActive(false);
        }
    }

    float Snap(float value)
    {
        return Mathf.Floor(value / 1.0f) * 1.0f;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Block"))
        {
            //TODO:Player killed effect
            //GetComponent<Animator>().SetBool("isHurt", true);
            isDead = true;
            GameManager.Instance.GameOverProcess();
        }
    }
}

