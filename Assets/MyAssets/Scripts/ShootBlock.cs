using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ShootBlock : MonoBehaviour
{
    Vector3 pos = Vector3.zero;
    //bool isMoveRight = true;

    Rigidbody rd;

    // Start is called before the first frame update
    void Start()
    {
        speed = GameManager.Instance.ShootBlockSpeed;
        rd = GetComponent<Rigidbody>();
    }
    float speed = 0;

    void Update()
    {
        /*
        if (isMoveRight)
        {
            pos = new Vector3(pos.x + speed * Time.deltaTime, pos.y, pos.z);
            transform.position = pos;
        }
        */

        pos = transform.position;

        if (pos.x > GameManager.Instance.RightLimitPos
            || pos.x < GameManager.Instance.LeftLimitPos)
        {
            GameManager.Instance.canShoot = true;
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (gameObject.CompareTag("Bullet") && other.gameObject.CompareTag("Dummy"))
        {
            other.gameObject.GetComponent<DummyBlock>().ChangeFace();
            GameManager.Instance.canShoot = true;
            GameManager.Instance.PlaySound(3);
            Destroy(gameObject);
        }
        else if (other.gameObject.CompareTag("Block"))
        {
            //isMoveRight = false;
            gameObject.tag = "Untagged";
            GameManager.Instance.canShoot = true;
            GameManager.Instance.PlaySound(4);

            // 反射ベクトルを計算する.
            var moveDir = rd.velocity;
            
            var pos = transform.position;
            //var newPos = new Vector3(pos.x - 1f, pos.y - 2f);
            var newPos = pos - moveDir/4f;

            gameObject.transform.DOMove(newPos, 0.05f).SetEase(Ease.OutQuad).OnComplete(() =>
               {
                   Destroy(gameObject);
               });

        }
    }
}
