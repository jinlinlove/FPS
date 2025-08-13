using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public int BulletCount = 10;

    public GameObject bulletPrefab;
    public GameObject casingPrefab;

    public Transform bulletTf;
    public Transform casingTf;

    // Start is called before the first frame update
    void Start()
    {
        
    }

   public void Attack()
    {
        GameObject bulletObj = Instantiate(bulletPrefab);
        bulletObj.transform.position=bulletTf.transform.position;
        bulletObj.GetComponent<Rigidbody>().AddForce(transform.forward * 500, ForceMode.Impulse);
        //�ӵ��ɿ�Щ �����Ĵ����ǹ��λ�ÿ������е����������ƫ��ֵ

        GameObject casingObj=Instantiate(casingPrefab);
        casingObj.transform.position=casingTf.transform.position;
    }
}
