using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_DPS_PauseMenu : UI_PauseMenu
{
    public Slider totalDamageSlider;
    public Image standardAttackImage;
    public Image skill1Image;
    public Image skill2Image;
    public Image skill3Image;
    public Image skill4Image;
    public Image forceImage;
    public Image dModeImage;
    public Image otherImage;
    public Image dotImage;
    // ... 其他伤害类别的Image

    private int totalDmg;  
    private int[] dmgData;
    
    [SerializeField] TextMeshProUGUI totalDamageText;
    [SerializeField] TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI detailedDamageInfoText;
    [SerializeField] private GameObject legends;

    public EnemyMoveController_E9001 dmgSource;

    private string[] catagoryName_EN = new string[]
    {
        "Standard Attack",
        "Skill 1",
        "Skill 2",
        "Skill 3",
        "Skill 4",
        "Force",
        "D-Mode",
        "Other",
        "DOT",
        "Skill(Total)"
    };
    
    private string[] catagoryName_ZHCN = new string[]
    {
        "普通攻击",
        "技能1",
        "技能2",
        "技能3",
        "技能4",
        "爆发攻击",
        "龙化",
        "其他",
        "持续伤害",
        "技能(总计)"
    };
    
    private string[] damageText = new string[]
    {
        "伤害",
        "Damage"
    };

    private string[] timeTextInfo = new string[]
    {
        "时间",
        "Time"
    };

    private void RefreshStatistic()
    {
        var dmgData = dmgSource.GetDPSData();
        
        var dmgText = GlobalController.Instance.GameLanguage == GlobalController.Language.ZHCN ? damageText[0] : damageText[1];
        var timeTextInfo = GlobalController.Instance.GameLanguage == GlobalController.Language.ZHCN ? this.timeTextInfo[0] : this.timeTextInfo[1];
        
        
        var totalDmg = dmgData[0] + dmgData[1] + dmgData[2] +
                       dmgData[3] + dmgData[4] + dmgData[5] +
                       dmgData[6] + dmgData[7] + dmgData[8];

        this.totalDmg = totalDmg;
        if (this.totalDmg == 0)
        {
            this.totalDmg = 1;
        }

        var totalTime = dmgSource.GetTime();

        if (totalDmg == 0)
        {
            standardAttackImage.color = new Color(standardAttackImage.color.r, standardAttackImage.color.g, standardAttackImage.color.b, 0);
            skill1Image.color = new Color(skill1Image.color.r, skill1Image.color.g, skill1Image.color.b, 0);
            skill2Image.color = new Color(skill2Image.color.r, skill2Image.color.g, skill2Image.color.b, 0);
            skill3Image.color = new Color(skill3Image.color.r, skill3Image.color.g, skill3Image.color.b, 0);
            skill4Image.color = new Color(skill4Image.color.r, skill4Image.color.g, skill4Image.color.b, 0);
            forceImage.color = new Color(forceImage.color.r, forceImage.color.g, forceImage.color.b, 0);
            dModeImage.color = new Color(dModeImage.color.r, dModeImage.color.g, dModeImage.color.b, 0);
            otherImage.color = new Color(otherImage.color.r, otherImage.color.g, otherImage.color.b, 1);
            dotImage.color = new Color(dotImage.color.r, dotImage.color.g, dotImage.color.b, 0);
            
            totalDamageText.text = $"{dmgText}: 0 / s";
            timeText.text = $"{timeTextInfo}: {(int)totalTime}s";
            
            return;
        }
        else
        {
            //Set the color of the images to Alpha 1
            standardAttackImage.color = new Color(standardAttackImage.color.r, standardAttackImage.color.g, standardAttackImage.color.b, 1);
            skill1Image.color = new Color(skill1Image.color.r, skill1Image.color.g, skill1Image.color.b, 1);
            skill2Image.color = new Color(skill2Image.color.r, skill2Image.color.g, skill2Image.color.b, 1);
            skill3Image.color = new Color(skill3Image.color.r, skill3Image.color.g, skill3Image.color.b, 1);
            skill4Image.color = new Color(skill4Image.color.r, skill4Image.color.g, skill4Image.color.b, 1);
            forceImage.color = new Color(forceImage.color.r, forceImage.color.g, forceImage.color.b, 1);
            dModeImage.color = new Color(dModeImage.color.r, dModeImage.color.g, dModeImage.color.b, 1);
            otherImage.color = new Color(otherImage.color.r, otherImage.color.g, otherImage.color.b, 1);
            dotImage.color = new Color(dotImage.color.r, dotImage.color.g, dotImage.color.b, 1);
        }


        var totalAmount = 0f;
        
        var images = new Image[] {standardAttackImage, skill1Image, skill2Image, skill3Image, skill4Image, forceImage, dModeImage, otherImage, dotImage};
        
        

        float[] scales = { (float)dmgData[0]/totalDmg, (float)dmgData[1]/totalDmg, (float)dmgData[2]/totalDmg, (float)dmgData[3]/totalDmg, (float)dmgData[4]/totalDmg, (float)dmgData[5]/totalDmg, (float)dmgData[6]/totalDmg, (float)dmgData[7]/totalDmg, (float)dmgData[8]/totalDmg };
        
        

        float startPos = -200f;

        for (int i = 0; i < images.Length; i++)
        {
            Image image = images[i];
            RectTransform rectTransform = image.GetComponent<RectTransform>();

            // 设置Image的长度
            Vector3 scale = image.transform.localScale;
            scale.x = scales[i];
            image.transform.localScale = scale;

            // 获取Image的实际长度
            float actualWidth = rectTransform.rect.width * scale.x;

            // 设置Image的位置
            Vector2 anchoredPosition = rectTransform.anchoredPosition;
            anchoredPosition.x = startPos + actualWidth / 2;
            rectTransform.anchoredPosition = anchoredPosition;

            // 更新下一个Image的起始位置
            startPos += actualWidth;
        }




        totalDamageText.text = $"{dmgText}: {(int)(totalDmg/totalTime)} / s";
        timeText.text = $"{timeTextInfo}: {(int)totalTime}s";

        this.dmgData = dmgData;
        

    }

    public void DisplayInfo(int id)
    {
        var catagoryName = GlobalController.Instance.GameLanguage == GlobalController.Language.ZHCN
            ? catagoryName_ZHCN
            : catagoryName_EN;
        
        
        
        switch (id)
        {
            /*case 1:
                detailedDamageInfoText.text = $"普通攻击: {(int)dmgSource.GetDPSData()[0]}({(float)(dmgData[0]/totalDmg): F1}%)";
                break;
            case 2:
                detailedDamageInfoText.text = $"技能1: {(int)dmgSource.GetDPSData()[1]}({(float)(dmgData[1]/totalDmg): F1}%)";
                break;
            case 3:
                detailedDamageInfoText.text = $"技能2: {(int)dmgSource.GetDPSData()[2]}({(float)(dmgData[2]/totalDmg): F1}%)";
                break;
            case 4:
                detailedDamageInfoText.text = $"技能3: {(int)dmgSource.GetDPSData()[3]}({(float)(dmgData[3]/totalDmg): F1}%)";
                break;
            case 5:
                detailedDamageInfoText.text = $"技能4: {(int)dmgSource.GetDPSData()[4]}({(float)(dmgData[4]/totalDmg): F1}%)";
                break;
            case 6:
                detailedDamageInfoText.text = $"爆发攻击: {(int)dmgSource.GetDPSData()[5]}({(float)(dmgData[5]/totalDmg): F1}%)";
                break;
            case 7:
                detailedDamageInfoText.text = $"龙化: {(int)dmgSource.GetDPSData()[6]}({(float)(dmgData[6]/totalDmg): F1}%)";
                break;
            case 8:
                detailedDamageInfoText.text = $"其他: {(int)dmgSource.GetDPSData()[7]}({(float)(dmgData[7]/totalDmg): F1}%)";
                break;*/
            case 1:
                detailedDamageInfoText.text = $"{catagoryName[0]}: {(int)(dmgSource.GetDPSData()[0]/dmgSource.GetTime())}({((float)100*dmgData[0]/totalDmg).ToString("F1")}%)";
                break;
            case 2:
                detailedDamageInfoText.text = $"{catagoryName[1]}: {(int)(dmgSource.GetDPSData()[1]/dmgSource.GetTime())}({((float)100*dmgData[1]/totalDmg).ToString("F1")}%)";
                break;
            case 3:
                detailedDamageInfoText.text = $"{catagoryName[2]}: {(int)(dmgSource.GetDPSData()[2]/dmgSource.GetTime())}({((float)100*dmgData[2]/totalDmg).ToString("F1")}%)";
                break;
            case 4:
                detailedDamageInfoText.text = $"{catagoryName[3]}: {(int)(dmgSource.GetDPSData()[3]/dmgSource.GetTime())}({((float)100*dmgData[3]/totalDmg).ToString("F1")}%)";
                break;
            case 5:
                detailedDamageInfoText.text = $"{catagoryName[4]}: {(int)(dmgSource.GetDPSData()[4]/dmgSource.GetTime())}({((float)100*dmgData[4]/totalDmg).ToString("F1")}%)";
                break;
            case 6:
                detailedDamageInfoText.text = $"{catagoryName[5]}: {(int)(dmgSource.GetDPSData()[5]/dmgSource.GetTime())}({((float)100*dmgData[5]/totalDmg).ToString("F1")}%)";
                break;
            case 7:
                detailedDamageInfoText.text = $"{catagoryName[6]}: {(int)(dmgSource.GetDPSData()[6]/dmgSource.GetTime())}({((float)100*dmgData[6]/totalDmg).ToString("F1")}%)";
                break;
            case 8:
                detailedDamageInfoText.text = $"{catagoryName[7]}: {(int)(dmgSource.GetDPSData()[7]/dmgSource.GetTime())}({((float)100*dmgData[7]/totalDmg).ToString("F1")}%)";
                break;
            case 9:
                detailedDamageInfoText.text = $"{catagoryName[8]}: {(int)(dmgSource.GetDPSData()[8]/dmgSource.GetTime())}({((float)100*dmgData[8]/totalDmg).ToString("F1")}%)";
                break;
            
        }

        if (id >= 2 & id <= 5)
        {
            var totalDmgSkill = dmgSource.GetDPSData()[1] + dmgSource.GetDPSData()[2] + dmgSource.GetDPSData()[3] + dmgSource.GetDPSData()[4];
            detailedDamageInfoText.text += $"\n{catagoryName[9]}:{totalDmgSkill/dmgSource.GetTime()}({((float)100*totalDmgSkill/totalDmg).ToString("F1")}%)";
        }
        
        
        
        
    }

    private void OnEnable()
    {
        RefreshStatistic();
        quest_name = GlobalController.Instance.GameLanguage == GlobalController.Language.ZHCN ? "伤害统计" : "DPS Statistics";
        
    }
}
