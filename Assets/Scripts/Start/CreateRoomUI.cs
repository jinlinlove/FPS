using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class CreateRoomUI : MonoBehaviourPunCallbacks
{
    InputField roomNameInput;//��������
    // Start is called before the first frame update
    void Start()
    {
        transform.Find("bg/title/closeBtn").GetComponent<Button>().onClick.AddListener(onCloseBtn);
        transform.Find("bg/okBtn").GetComponent<Button>().onClick.AddListener(onCreateBtn);
        roomNameInput = transform.Find("bg/InputField").GetComponent<InputField>();

        //���һ����������
        roomNameInput.text = "room_" + Random.Range(1, 9999);
    }


    //��������
    public void onCreateBtn()
    {
        Game.uiManager.ShowUI<MaskUI>("MaskUI").ShowMsg("������...");
        RoomOptions room=new RoomOptions();
        room.MaxPlayers = 8;//��������������
        PhotonNetwork.CreateRoom(roomNameInput.text, room);//1.������2.����Ĳ�������
    }

    public void onCloseBtn()
    {
        Game.uiManager.CloseUI(gameObject.name);
    }

    //�����ɹ��ص�
    public override void OnCreatedRoom()
    {
        Debug.Log("�����ɹ�");
        Game.uiManager.CloseAllUI();
        //��ʾ����ui
        Game.uiManager.ShowUI<RoomUI>("RoomUI");
    }

    //����ʧ��
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Game.uiManager.CloseUI("MaskUI"); 
    }
}
