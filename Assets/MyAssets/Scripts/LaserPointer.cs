using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace tsunahiyo
{
    public class LaserPointer : MonoBehaviour
    {
        [SerializeField]
        private Transform _RightHandAnchor;

        [SerializeField]
        private Transform _LeftHandAnchor;

        [SerializeField]
        private Transform _CenterEyeAnchor;

        [SerializeField]
        private float _MaxDistance = 100.0f;

        [SerializeField]
        private LineRenderer _LaserPointerRenderer;

        [SerializeField]
        GameObject bulletPrefb;

        [SerializeField]
        Material blueMat;
        [SerializeField]
        Material redMat;

        bool canShoot()
        {
            return GameManager.Instance.canShoot;
        }

        private Transform Pointer
        {
            get
            {
                // 現在アクティブなコントローラーを取得
                var controller = OVRInput.GetActiveController();
                if (controller == OVRInput.Controller.RTrackedRemote
                    || controller == OVRInput.Controller.RTouch

                    )
                {
                    return _RightHandAnchor;
                }
                else if (controller == OVRInput.Controller.LTrackedRemote)
                {
                    return _LeftHandAnchor;
                }
                // どちらも取れなければ目の間からビームが出る
                return _CenterEyeAnchor;
            }
        }

        void Update()
        {
            var pointer = Pointer;
            if (pointer == null || _LaserPointerRenderer == null)
            {
                return;
            }
            // コントローラー位置からRayを飛ばす
            Ray pointerRay = new Ray(pointer.position, pointer.forward);

            // レーザーの起点
            _LaserPointerRenderer.SetPosition(0, pointerRay.origin);

            RaycastHit hitInfo;
            if (Physics.Raycast(pointerRay, out hitInfo, _MaxDistance))
            {
                // Rayがヒットしたらそこまで
                _LaserPointerRenderer.SetPosition(1, hitInfo.point);
            }
            else
            {
                // Rayがヒットしなかったら向いている方向にMaxDistance伸ばす
                _LaserPointerRenderer.SetPosition(1, pointerRay.origin + pointerRay.direction * _MaxDistance);
            }

            if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger))
            {
                //var bullet = Instantiate(bulletPrefb, pointer.position, Quaternion.identity);
                //bullet.GetComponent<Rigidbody>().AddForce(pointerRay.direction * 1000f);

                _LaserPointerRenderer.material = redMat;
                StartCoroutine(SetColor(_LaserPointerRenderer));

                RaycastHit hit;
                if (Physics.Raycast(pointerRay, out hit, 100f))
                {
                    if (hit.transform.gameObject.CompareTag("Dummy"))
                    {
                        hit.transform.gameObject.GetComponent<DummyBlock>().ChangeFace();

                        /*
                        if (hit.transform.gameObject.GetComponent<DummyBlock>().canDelete == true)
                        {
                            hit.transform.gameObject.GetComponent<DummyBlock>().ChangeFace();
                        }
                        */
                    }
                }
            }
        }

        IEnumerator SetColor(LineRenderer lr)
        {
            yield return new WaitForSeconds(0.1f);
            lr.material = blueMat;
        }
    }
}