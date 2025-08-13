using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class FightManager : MonoBehaviour
{
    private void Awake()
    {
        //隐藏图标
        Cursor.lockState= CursorLockMode.Locked;
        Cursor.visible= false;

        //关闭所有界面
        Game.uiManager.CloseAllUI();
        //显示战斗界面
        Game.uiManager.ShowUI<FightUI>("FightUI");

        Transform poinTF = GameObject.Find("Point").transform;

        Vector3 pos=poinTF.GetChild(Random.Range(0,poinTF.childCount)).position;

        //实例化角色
        PhotonNetwork.Instantiate("Player",pos,Quaternion.identity);//实例化的资源要放在Resources文件夹

    }
}
