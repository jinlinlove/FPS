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
            DontDestroyOnLoad(gameObject);//跳转场景不销毁
            uiManager=new UIManager();
            uiManager.Init();

            //设置发送 接收消息频率 降低延迟
            PhotonNetwork.SendRate = 50;
            PhotonNetwork.SerializationRate = 50;
        }
    }

    void Start()
    {
        //显示登陆界面   
        uiManager.ShowUI<LoginUI>("LoginUI");
    }
}
