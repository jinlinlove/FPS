using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
//大厅界面
public class LobbyUI : MonoBehaviourPunCallbacks
{
    private Button CloseBtn;
    private Button CreateBtn;
    private Button UpdateBtn;
    TypedLobby lobby;//大厅对象

    private Transform contentTf;
    private GameObject roomPrefab;

    // Start is called before the first frame update
    void Start()
    {
        CloseBtn=transform.Find("content/title/closeBtn").GetComponent<Button>();
        CloseBtn.onClick.AddListener(onCloseBtn);

        CreateBtn = transform.Find("content/createBtn").GetComponent<Button>();
        CreateBtn.onClick.AddListener(onCreateRoomBtn);

        UpdateBtn = transform.Find("content/updateBtn").GetComponent<Button>();
        UpdateBtn.onClick.AddListener(onUpdateRoomBtn);

        contentTf = transform.Find("content/Scroll View/Viewport/Content");
        roomPrefab = transform.Find("content/Scroll View/Viewport/item").gameObject;

        lobby = new TypedLobby("fpsLobby", LobbyType.SqlLobby);//1.大厅名字，2.大厅类型(可搜索)
        //进入大厅
        PhotonNetwork.JoinLobby(lobby);
    }

    //进入大厅回调
    public override void OnJoinedLobby()
    {
        Debug.Log("进入大厅...");
    }

    //创建房间
    public void onCreateRoomBtn()
    {
        Game.uiManager.ShowUI<CreateRoomUI>("CreateRoomUI");
    }

    //关闭大厅界面
    public void onCloseBtn()
    {
        PhotonNetwork.Disconnect();
        Game.uiManager.CloseUI(gameObject.name);
        //显示登录界面
        Game.uiManager.ShowUI<LoginUI>("LoginUI");
    }

    //刷新房间列表
    public void onUpdateRoomBtn()
    {
        Game.uiManager.ShowUI<MaskUI>("MaskUI").ShowMsg("刷新中...");

        PhotonNetwork.GetCustomRoomList(lobby, "1");//执行该方法后会触发OnRoomListUpdate回调
    }


    //清楚已经存在的房间物体
    private void ClearRoomList()
    {
        while (contentTf.childCount != 0)
        {
            DestroyImmediate(contentTf.GetChild(0).gameObject);
        }
    }

    //刷新房间后的回调
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Game.uiManager.CloseUI("MaskUI");
        //Debug.Log("房间刷新");

        ClearRoomList();
        //Game.uiManager.ShowUI<MaskUI>("MaskUI").ShowMsg(roomList.Count.ToString());
        //Game.uiManager.CloseUI("MaskUI");
        for (int i = 0; i < roomList.Count; i++)
        {
            GameObject obj = Instantiate(roomPrefab, contentTf);
            obj.SetActive(true);
            string roomName = roomList[i].Name;//房间名字
            obj.transform.Find("roomName").GetComponent<Text>().text = roomName;
            obj.transform.Find("joinBtn").GetComponent<Button>().onClick.AddListener(delegate ()
            {
                Debug.Log(roomName);
                //加入房间
                Game.uiManager.ShowUI<MaskUI>("MaskUI").ShowMsg("加入中...");

                PhotonNetwork.JoinRoom(roomName);//加入房间
            });
        }
    }

    //加入房间成功回调
    public override void OnJoinedRoom()
    {
        Game.uiManager.CloseAllUI();
        Game.uiManager.ShowUI<RoomUI>("RoomUI");
    }

    //加入房间失败回调
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Game.uiManager.CloseUI("MaskUI");
    }
}