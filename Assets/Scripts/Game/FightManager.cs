using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class FightManager : MonoBehaviour
{
    private void Awake()
    {
        //����ͼ��
        Cursor.lockState= CursorLockMode.Locked;
        Cursor.visible= false;

        //�ر����н���
        Game.uiManager.CloseAllUI();
        //��ʾս������
        Game.uiManager.ShowUI<FightUI>("FightUI");

        Transform poinTF = GameObject.Find("Point").transform;

        Vector3 pos=poinTF.GetChild(Random.Range(0,poinTF.childCount)).position;

        //ʵ������ɫ
        PhotonNetwork.Instantiate("Player",pos,Quaternion.identity);//ʵ��������ԴҪ����Resources�ļ���

    }
}
