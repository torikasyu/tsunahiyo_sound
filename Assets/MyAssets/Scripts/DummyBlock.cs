using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyBlock : MonoBehaviour
{
    //[SerializeField]
    //Sprite PlacedSprite;
    //[SerializeField]
    //GameObject bulletPrefab;

    [SerializeField]
    Material brokenMaterial;

    public bool canDelete { get; set; } = false;
    public bool isDeleted { get; set; } = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ChangeFace()
    {
        print("ChangeFace");

        if (isDeleted || !canDelete)
        {
            print("Cant delete");
            return;
        }

        //TODO:changeface
        gameObject.GetComponent<Renderer>().material = brokenMaterial;
        canDelete = false;
        isDeleted = true;
        gameObject.transform.parent.gameObject.GetComponent<BlockParent>().LuckCount--;

        /*
        var changed = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        changed.transform.SetParent(gameObject.transform.parent.transform);
        gameObject.transform.parent.gameObject.GetComponent<BlockParent>().LuckCount--;
        gameObject.SetActive(false);
        */
    }
}
