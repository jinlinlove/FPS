using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class LoginUI : MonoBehaviour,IConnectionCallbacks
{
    // Start is called before the first frame update
    private Button startBtn;
    private Button quitBtn;
    void Start()
    {
        startBtn=transform.Find("startBtn").GetComponent<Button>();
        startBtn.onClick.AddListener(onStartBtn);
        quitBtn= transform.Find("quitBtn").GetComponent<Button>();
        quitBtn.onClick.AddListener(onQuitBtn);
    }


    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);//注册pun2事件
    }

    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);//注销pun2事件
    }

    public void onStartBtn()
    {
        Game.uiManager.ShowUI<MaskUI>("MaskUI").ShowMsg("连接服务器...");


        //连接pun2服务器
        PhotonNetwork.ConnectUsingSettings();//成功后会执行OnConnectedToMaster函数
    }

    public void onQuitBtn()
    {
        Application.Quit();
    }

    public void OnConnected()
    {
        
    }
    
    //连接成功后的执行函数
    public void OnConnectedToMaster()
    {
        //关闭所有界面
        Game.uiManager.CloseAllUI();
        //显示大厅界面
        //Debug.Log("成功");
        Game.uiManager.ShowUI<LobbyUI>("LobbyUI");
    }

    //断开服务器执行的函数
    public void OnDisconnected(DisconnectCause cause)
    {
        Game.uiManager.CloseUI("MaskUI");
    }

    public void OnRegionListReceived(RegionHandler regionHandler)
    {
       
    }

    public void OnCustomAuthenticationResponse(Dictionary<string, object> data)
    {
        
    }

    public void OnCustomAuthenticationFailed(string debugMessage)
    {
       
    }
}
