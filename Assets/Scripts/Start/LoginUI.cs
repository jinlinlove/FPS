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
        PhotonNetwork.AddCallbackTarget(this);//ע��pun2�¼�
    }

    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);//ע��pun2�¼�
    }

    public void onStartBtn()
    {
        Game.uiManager.ShowUI<MaskUI>("MaskUI").ShowMsg("���ӷ�����...");


        //����pun2������
        PhotonNetwork.ConnectUsingSettings();//�ɹ����ִ��OnConnectedToMaster����
    }

    public void onQuitBtn()
    {
        Application.Quit();
    }

    public void OnConnected()
    {
        
    }
    
    //���ӳɹ����ִ�к���
    public void OnConnectedToMaster()
    {
        //�ر����н���
        Game.uiManager.CloseAllUI();
        //��ʾ��������
        //Debug.Log("�ɹ�");
        Game.uiManager.ShowUI<LobbyUI>("LobbyUI");
    }

    //�Ͽ�������ִ�еĺ���
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
