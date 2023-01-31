using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillUINormal : SkillUIBase
{
    // Start is called before the first frame update
    [SerializeField] protected Sprite ImageNormal;
    [SerializeField] protected Sprite ImageBW;
    protected override void Start()
    {
        base.Start();
        
        //这里需要根据角色id加载UI图片
        
        var skill = transform.Find("IconBody").Find("Mask").Find("skill");
        skill.transform.GetChild(0).GetComponent<Image>().sprite = ImageNormal;
        skill.transform.GetChild(1).GetComponent<Image>().sprite = ImageBW;
        skill.transform.GetChild(1).gameObject.SetActive(false);
    }
    
}
