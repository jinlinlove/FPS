using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
//��������
public class LobbyUI : MonoBehaviourPunCallbacks
{
    private Button CloseBtn;
    private Button CreateBtn;
    private Button UpdateBtn;
    TypedLobby lobby;//��������

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

        lobby = new TypedLobby("fpsLobby", LobbyType.SqlLobby);//1.�������֣�2.��������(������)
        //�������
        PhotonNetwork.JoinLobby(lobby);
    }

    //��������ص�
    public override void OnJoinedLobby()
    {
        Debug.Log("�������...");
    }

    //��������
    public void onCreateRoomBtn()
    {
        Game.uiManager.ShowUI<CreateRoomUI>("CreateRoomUI");
    }

    //�رմ�������
    public void onCloseBtn()
    {
        PhotonNetwork.Disconnect();
        Game.uiManager.CloseUI(gameObject.name);
        //��ʾ��¼����
        Game.uiManager.ShowUI<LoginUI>("LoginUI");
    }

    //ˢ�·����б�
    public void onUpdateRoomBtn()
    {
        Game.uiManager.ShowUI<MaskUI>("MaskUI").ShowMsg("ˢ����...");

        PhotonNetwork.GetCustomRoomList(lobby, "1");//ִ�и÷�����ᴥ��OnRoomListUpdate�ص�
    }


    //����Ѿ����ڵķ�������
    private void ClearRoomList()
    {
        while (contentTf.childCount != 0)
        {
            DestroyImmediate(contentTf.GetChild(0).gameObject);
        }
    }

    //ˢ�·����Ļص�
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Game.uiManager.CloseUI("MaskUI");
        //Debug.Log("����ˢ��");

        ClearRoomList();
        //Game.uiManager.ShowUI<MaskUI>("MaskUI").ShowMsg(roomList.Count.ToString());
        //Game.uiManager.CloseUI("MaskUI");
        for (int i = 0; i < roomList.Count; i++)
        {
            GameObject obj = Instantiate(roomPrefab, contentTf);
            obj.SetActive(true);
            string roomName = roomList[i].Name;//��������
            obj.transform.Find("roomName").GetComponent<Text>().text = roomName;
            obj.transform.Find("joinBtn").GetComponent<Button>().onClick.AddListener(delegate ()
            {
                Debug.Log(roomName);
                //���뷿��
                Game.uiManager.ShowUI<MaskUI>("MaskUI").ShowMsg("������...");

                PhotonNetwork.JoinRoom(roomName);//���뷿��
            });
        }
    }

    //���뷿��ɹ��ص�
    public override void OnJoinedRoom()
    {
        Game.uiManager.CloseAllUI();
        Game.uiManager.ShowUI<RoomUI>("RoomUI");
    }

    //���뷿��ʧ�ܻص�
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Game.uiManager.CloseUI("MaskUI");
    }
}