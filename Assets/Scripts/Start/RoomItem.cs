using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class RoomItem : MonoBehaviour
{
    public int owerId;//玩家编号

    public bool IsReady=false;//是否准备

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
        //创建一个 Photon 自定义的哈希表（Hashtable），用于存储要同步的玩家自定义属性
        table.Add("IsReady", IsReady);
        //向哈希表中添加一个键值对
        PhotonNetwork.LocalPlayer.SetCustomProperties(table);//设置自定义参数
        //调用 Photon API，将本地玩家的自定义属性（即 isReady）设置并同步到服务器
        //所有房间内的其他玩家都会收到这个属性变更的通知（通过 OnPlayerPropertiesUpdate 回调）
        //PhotonNetwork.LocalPlayer 表示当前客户端所控制的玩家对象
        ChangeReady(IsReady);
    }

    public void ChangeReady(bool isReady)
    {
        transform.Find("Button/Text").GetComponent<Text>().text = isReady == true ? "已准备" : "未准备";
    }
}
