using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus;

public class Gun : MonoBehaviour
{

    bool canShoot()
    {
        return GameManager.Instance.canShoot;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
}
