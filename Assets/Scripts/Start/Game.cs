using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Game : MonoBehaviour
{
    public static UIManager uiManager;
    public static bool isLoaded=false;
    private void Awake()
    {
        if (isLoaded == true)
        {
            Destroy(gameObject);
        }
        else
        {
            isLoaded = true;
            DontDestroyOnLoad(gameObject);//��ת����������
            uiManager=new UIManager();
            uiManager.Init();

            //���÷��� ������ϢƵ�� �����ӳ�
            PhotonNetwork.SendRate = 50;
            PhotonNetwork.SerializationRate = 50;
        }
    }

    void Start()
    {
        //��ʾ��½����   
        uiManager.ShowUI<LoginUI>("LoginUI");
    }
}
