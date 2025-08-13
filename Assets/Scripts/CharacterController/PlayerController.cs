using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;


//角色控制器
public class PlayerController : MonoBehaviourPun,IPunObservable
{

    //组件
    public Animator ani;
    public Rigidbody body;
    public Transform camTF;//跟随的摄像机

    //数值
    public int CurHp = 10;
    public int MaxHp = 10;
    public float MoveSpeed = 3.5f;

    public float H;//水平值
    public float V;//垂直值
    public Vector3 dir;//移动方向

    public Vector3 offset;//摄像机与角色之间的偏移值

    public float Mouse_X;
    public float Mouse_Y;
    public float scroll;
    public float Angle_X;
    public float Angle_Y;

    public Quaternion camRotation;//摄像机旋转的四元数

    public Gun gun;

    //声音
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
        //判断是不是本机玩家，只操作本机玩家
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

    //其他玩家更新发过来的数据(位置，旋转)
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

    //更新旋转(同时设置摄像机的位置的旋转值)
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


    //角色操作
    public void InputCtl()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //如果播放换弹动作不能开枪

            if (ani.GetCurrentAnimatorStateInfo(1).IsName("Reload"))
            {
                return;
            }

            //判断子弹数量
            if (gun.BulletCount > 0)
            {
                gun.BulletCount--;
                Game.uiManager.GetUI<FightUI>("FightUI").UpdateBulletCount(gun.BulletCount);

                //播放开火动画
                ani.Play("Fire", 1, 0);
                
                StopAllCoroutines();
                StartCoroutine(AttackCo());
            }
        }
        if(Input.GetKeyDown(KeyCode.R))
        {
            //填充子弹
            AudioSource.PlayClipAtPoint(reloadClip,transform.position);//播放换弹声音
            ani.Play("Reload");
            gun.BulletCount = 10;
            Game.uiManager.GetUI<FightUI>("FightUI").UpdateBulletCount(gun.BulletCount);
        }
    }

    //攻击协同程序
    IEnumerator AttackCo()
    {
        //延迟1秒发射子弹
        yield return new WaitForSeconds(0.1f);

        //播放射击音效
        AudioSource.PlayClipAtPoint(shootClip,transform.position);

        //射线检测 鼠标中心发送射线
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, Input.mousePosition.z));
        //射线可以改成在枪口位置起始点，避免射线到自身

        RaycastHit hit;

        if(Physics.Raycast(ray,out hit, 10000, LayerMask.GetMask("Player")))
        {
            Debug.Log("射到角色");
            hit.transform.GetComponent<PlayerController>().GetHit();
        }

        photonView.RPC("AttackRpc", RpcTarget.All);//所有玩家执行AttackRpc函数       
    }

    [PunRPC]
    public void AttackRpc()
    {
        gun.Attack();
    }

    //受伤
    public void GetHit()
    {
        if (isDie == true)
        {
            return;
        }

        //同步所有角色受伤
        photonView.RPC("GetHitRPC", RpcTarget.All);
    }

    [PunRPC]
    public void GetHitRPC()
    {
        CurHp -= 1;//扣血

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
                Invoke("gameOver", 3);//3秒后显示失败界面
            }
        }
        
    }

    private void gameOver()
    {
        //显示鼠标
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        //显示失败界面
        Game.uiManager.ShowUI<LossUI>("LossUI").onClickCallBack= OnReset;
    }

    //复活
    public void OnReset()
    {
        //隐藏鼠标
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

    //限制角度
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
            //发送数据
            stream.SendNext(H);
            stream.SendNext(V);
            stream.SendNext(Angle_X);
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else
        {
            //接收数据
            H=(float)stream.ReceiveNext();
            V=(float)stream.ReceiveNext();
            Angle_X = (float)stream.ReceiveNext();
            currentPos=(Vector3)stream.ReceiveNext();
            currentRotaion=(Quaternion)stream.ReceiveNext();
        }
    }
}
