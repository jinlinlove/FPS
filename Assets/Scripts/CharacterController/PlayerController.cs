using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;


//��ɫ������
public class PlayerController : MonoBehaviourPun,IPunObservable
{

    //���
    public Animator ani;
    public Rigidbody body;
    public Transform camTF;//����������

    //��ֵ
    public int CurHp = 10;
    public int MaxHp = 10;
    public float MoveSpeed = 3.5f;

    public float H;//ˮƽֵ
    public float V;//��ֱֵ
    public Vector3 dir;//�ƶ�����

    public Vector3 offset;//��������ɫ֮���ƫ��ֵ

    public float Mouse_X;
    public float Mouse_Y;
    public float scroll;
    public float Angle_X;
    public float Angle_Y;

    public Quaternion camRotation;//�������ת����Ԫ��

    public Gun gun;

    //����
    public AudioClip reloadClip;
    public AudioClip shootClip;

    public bool isDie = false;

    public Vector3 currentPos;
    public Quaternion currentRotaion;

    void Start()
    {

        Angle_X=transform.eulerAngles.x;
        Angle_Y = transform.eulerAngles.y;

        ani= GetComponent<Animator>();
        body= GetComponent<Rigidbody>();

        gun= GetComponentInChildren<Gun>();
        camTF=Camera.main.transform;
        currentPos=transform.position;
        currentRotaion=transform.rotation;
        if (photonView.IsMine)
        {
            Game.uiManager.GetUI<FightUI>("FightUI").UpdateHp(CurHp,MaxHp);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //�ж��ǲ��Ǳ�����ң�ֻ�����������
        if (photonView.IsMine)
        {
            if (isDie == true)
            {
                return;
            }

            UpdatePosition();
            UpdateRotation();
            InputCtl();
        }
        else
        {
            UpdateLogic();
        }
    }

    //������Ҹ��·�����������(λ�ã���ת)
    public void UpdateLogic()
    {
        transform.position = Vector3.Lerp(transform.position, currentPos, Time.deltaTime*MoveSpeed*10);
        transform.rotation = Quaternion.Slerp(transform.rotation, currentRotaion, Time.deltaTime * 500);
    }

    private void LateUpdate()
    {
        ani.SetFloat("Horizontal", H);
        ani.SetFloat("Vertical", V);
        ani.SetBool("isDie",isDie);
    }

    public void UpdatePosition()
    {
        H = Input.GetAxisRaw("Horizontal");
        V = Input.GetAxisRaw("Vertical");
        dir = camTF.forward * V + camTF.right * H;
        body.MovePosition(transform.position + dir * Time.deltaTime * MoveSpeed);
    }

    //������ת(ͬʱ�����������λ�õ���תֵ)
    public void UpdateRotation()
    {

        Mouse_X = Input.GetAxisRaw("Mouse X");
        Mouse_Y = Input.GetAxisRaw("Mouse Y");
        scroll= Input.GetAxis("Mouse ScrollWheel");

        Angle_X = Angle_X - Mouse_Y;
        Angle_Y=Angle_Y + Mouse_X;

        Angle_X = ClampAngle(Angle_X, -60, 60);
        Angle_Y = ClampAngle(Angle_Y, -360, 360);

        camRotation = Quaternion.Euler(Angle_X, Angle_Y, 0);

        camTF.rotation = camRotation;

        offset.z += scroll;
        
        camTF.position = transform.position + camTF.rotation*offset;

        transform.eulerAngles = new Vector3(0, camTF.eulerAngles.y, 0);
    }


    //��ɫ����
    public void InputCtl()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //������Ż����������ܿ�ǹ

            if (ani.GetCurrentAnimatorStateInfo(1).IsName("Reload"))
            {
                return;
            }

            //�ж��ӵ�����
            if (gun.BulletCount > 0)
            {
                gun.BulletCount--;
                Game.uiManager.GetUI<FightUI>("FightUI").UpdateBulletCount(gun.BulletCount);

                //���ſ��𶯻�
                ani.Play("Fire", 1, 0);
                
                StopAllCoroutines();
                StartCoroutine(AttackCo());
            }
        }
        if(Input.GetKeyDown(KeyCode.R))
        {
            //����ӵ�
            AudioSource.PlayClipAtPoint(reloadClip,transform.position);//���Ż�������
            ani.Play("Reload");
            gun.BulletCount = 10;
            Game.uiManager.GetUI<FightUI>("FightUI").UpdateBulletCount(gun.BulletCount);
        }
    }

    //����Эͬ����
    IEnumerator AttackCo()
    {
        //�ӳ�1�뷢���ӵ�
        yield return new WaitForSeconds(0.1f);

        //���������Ч
        AudioSource.PlayClipAtPoint(shootClip,transform.position);

        //���߼�� ������ķ�������
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, Input.mousePosition.z));
        //���߿��Ըĳ���ǹ��λ����ʼ�㣬�������ߵ�����

        RaycastHit hit;

        if(Physics.Raycast(ray,out hit, 10000, LayerMask.GetMask("Player")))
        {
            Debug.Log("�䵽��ɫ");
            hit.transform.GetComponent<PlayerController>().GetHit();
        }

        photonView.RPC("AttackRpc", RpcTarget.All);//�������ִ��AttackRpc����       
    }

    [PunRPC]
    public void AttackRpc()
    {
        gun.Attack();
    }

    //����
    public void GetHit()
    {
        if (isDie == true)
        {
            return;
        }

        //ͬ�����н�ɫ����
        photonView.RPC("GetHitRPC", RpcTarget.All);
    }

    [PunRPC]
    public void GetHitRPC()
    {
        CurHp -= 1;//��Ѫ

        if (CurHp <= 0)
        {
            CurHp = 0;
            isDie = true;
            
        }

        if (photonView.IsMine)
        {
            Game.uiManager.GetUI<FightUI>("FightUI").UpdateHp(CurHp, MaxHp);
            Game.uiManager.GetUI<FightUI>("FightUI").UpdateBlood();

            if (CurHp == 0)
            {
                Invoke("gameOver", 3);//3�����ʾʧ�ܽ���
            }
        }
        
    }

    private void gameOver()
    {
        //��ʾ���
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        //��ʾʧ�ܽ���
        Game.uiManager.ShowUI<LossUI>("LossUI").onClickCallBack= OnReset;
    }

    //����
    public void OnReset()
    {
        //�������
        Cursor.visible= false;
        Cursor.lockState = CursorLockMode.Locked;

        photonView.RPC("OnResetRPC", RpcTarget.All);
    }

    [PunRPC]
    public void OnResetRPC()
    {
        isDie = false;
        CurHp=MaxHp;
        if(photonView.IsMine)
        {
            Game.uiManager.GetUI<FightUI>("FightUI").UpdateHp(CurHp,MaxHp);
        }
    }

    //���ƽǶ�
    public float ClampAngle(float val, float min, float max)
    {
        if (val > 360)
        {
            val -= 360;
        }
        if(val < -360)
        {
            val += 360;
        }
        return Mathf.Clamp(val, min, max);
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if (ani != null)
        {
            Vector3 angle=ani.GetBoneTransform(HumanBodyBones.Chest).localEulerAngles;
            angle.x=Angle_X; 
            ani.SetBoneLocalRotation(HumanBodyBones.Chest, Quaternion.Euler(angle));
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting)
        {
            //��������
            stream.SendNext(H);
            stream.SendNext(V);
            stream.SendNext(Angle_X);
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else
        {
            //��������
            H=(float)stream.ReceiveNext();
            V=(float)stream.ReceiveNext();
            Angle_X = (float)stream.ReceiveNext();
            currentPos=(Vector3)stream.ReceiveNext();
            currentRotaion=(Quaternion)stream.ReceiveNext();
        }
    }
}
