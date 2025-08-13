using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class RoomItem : MonoBehaviour
{
    public int owerId;//��ұ��

    public bool IsReady=false;//�Ƿ�׼��

    // Start is called before the first frame update
    void Start()
    {
        if (owerId == PhotonNetwork.LocalPlayer.ActorNumber)
        {
            transform.Find("Button").GetComponent<Button>().onClick.AddListener(OnReadyBtn);
        }
        else
        {
            transform.Find("Button").GetComponent<Image>().color = Color.black;
        }

        ChangeReady(IsReady);
    }

    public void OnReadyBtn()
    {
        IsReady = !IsReady;

        ExitGames.Client.Photon.Hashtable table=new ExitGames.Client.Photon.Hashtable();
        //����һ�� Photon �Զ���Ĺ�ϣ��Hashtable�������ڴ洢Ҫͬ��������Զ�������
        table.Add("IsReady", IsReady);
        //���ϣ�������һ����ֵ��
        PhotonNetwork.LocalPlayer.SetCustomProperties(table);//�����Զ������
        //���� Photon API����������ҵ��Զ������ԣ��� isReady�����ò�ͬ����������
        //���з����ڵ�������Ҷ����յ�������Ա����֪ͨ��ͨ�� OnPlayerPropertiesUpdate �ص���
        //PhotonNetwork.LocalPlayer ��ʾ��ǰ�ͻ��������Ƶ���Ҷ���
        ChangeReady(IsReady);
    }

    public void ChangeReady(bool isReady)
    {
        transform.Find("Button/Text").GetComponent<Text>().text = isReady == true ? "��׼��" : "δ׼��";
    }
}
