using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LossUI : MonoBehaviour
{
    public System.Action onClickCallBack;

    void Start()
    {
        transform.Find("resetBtn").GetComponent<Button>().onClick.AddListener(OnclickBtn);
    }

    public void OnclickBtn()
    {
        if(onClickCallBack != null)
        {
            onClickCallBack();
        }
        Game.uiManager.CloseUI(gameObject.name);
    }
}
