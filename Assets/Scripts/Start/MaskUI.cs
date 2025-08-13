using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


//’⁄’÷ΩÁ√Ê
public class MaskUI : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void ShowMsg(string msg)
    {
        transform.Find("msg/bg/Text").GetComponent<Text>().text = msg;
    }
}
