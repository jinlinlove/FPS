using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FightUI : MonoBehaviour
{
    private Image bloodImg;
    void Start()
    {
        bloodImg=transform.Find("blood").GetComponent<Image>();
    }

    //更新子弹个数显示
    // Update is called once per frame
    public void UpdateBulletCount(int count)
    {
        transform.Find("bullet/Text").GetComponent<Text>().text = count.ToString();
    }

    //更新血量
    public void UpdateHp(float cur, float max)
    {
        transform.Find("hp/fill").GetComponent<Image>().fillAmount = cur / max;
        transform.Find("hp/Text").GetComponent<Text>().text = cur + "/" + max;
    }

    public void UpdateBlood()
    {
        StopAllCoroutines();
        StopCoroutine(UpdateBloodCo());
    }

    public IEnumerator UpdateBloodCo()
    {
        bloodImg.color= Color.white;
        Color color=bloodImg.color;
        float t = 0.35f;
        while (t >= 0)
        {
            t-=Time.deltaTime;
            color.a=Mathf.Abs(Mathf.Sin(Time.realtimeSinceStartup));
            bloodImg.color = color;
            
            yield return null;
        }
        
        color.a=0;
        bloodImg.color=color;
    }
}
