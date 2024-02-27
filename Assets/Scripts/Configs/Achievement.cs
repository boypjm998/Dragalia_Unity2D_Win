using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

[Serializable]
public class Achievement
{
    //public fields
    public int id;
    public int rarity;
    public string[] name = new string[3];
    public string[] description = new string[3];
    public int maxProgress = 1;
    public int hideCondition = 0;
    public AchievementSystem.ProgressType progressType;
    
    
    //private fields
    private string progressInfoStr;
    private string plainProgressInfo = "";
    
    //properties
    public bool IsFinished => IsCompleted();
    

    public string GetProgress()
    {
        if (progressInfoStr == "")
        {
            return "QUJDREVGRw==";
        }

        return progressInfoStr;
    }

    /// <summary>
    /// Set Encrypted Progress String
    /// </summary>
    /// <param name="str"></param>
    public void SetProgress(string str)
    {
        progressInfoStr = str;
    }
    
    public Achievement(int id, int rarity,
        string[] name, string[] description,
        AchievementSystem.ProgressType progressType,
        string progressInfoStr,int hideCondition,int maxProgress = 1)
    {
        this.id = id;
        this.rarity = rarity;
        this.name = name;
        this.description = description;
        this.progressType = progressType;
        this.progressInfoStr = progressInfoStr;
        this.hideCondition = hideCondition;
        this.maxProgress = maxProgress;
    }
    
    private bool IsCompleted()
    {
        bool finished = false;

        if (plainProgressInfo == String.Empty)
        {
            plainProgressInfo = AchievementSystem.DecryptAchievementProcessString(this);
        }


        string plainText = plainProgressInfo;
        
        
        switch (progressType)
        {
            case AchievementSystem.ProgressType.OneOffBoolean:
                //如果plainText不包含标志FINISHED，强行设置为FINISHED:FALSE
                if (!plainText.Contains("FINISHED=TRUE"))
                {
                    Debug.Log("强制设置为FALSE");
                    plainText = "FINISHED=FALSE";
                }
                else
                {
                    plainText = "FINISHED=TRUE";
                    finished = true;
                }
                //AchievementSystem.EncryptAchievementProcessString(achievement,plainText);
                break;
            
            case AchievementSystem.ProgressType.DistinctArray:
                if (!plainText.Contains("LIST=/"))
                {
                    plainText = "LIST=//";
                }
                
                int count = plainText.Count(ch => ch == ',') + 1;
                if (plainText.Contains("//"))
                {
                    count = 0;
                }
                
                if (count >= maxProgress && count>0)
                {
                    finished = true;
                }
                break;
            
            case AchievementSystem.ProgressType.RepeatInteger:
                if (!plainText.Contains("CURRENT="))
                {
                    plainText = "CURRENT=0";
                }
                else
                {
                    int current = int.Parse(plainText.Split('=')[1]);
                    if (current >= maxProgress)
                    {
                        finished = true;
                    }
                }

                
                break;


            default:
                throw new ArgumentOutOfRangeException();
        }

        return finished;
        
    }

    public bool UpdateProgress(int amount)
    {
        if (plainProgressInfo == String.Empty)
        {
            plainProgressInfo = AchievementSystem.DecryptAchievementProcessString(this);
        }

        bool finished = false;

        string plainText = plainProgressInfo;

        switch (progressType)
        {
            case AchievementSystem.ProgressType.OneOffBoolean:
            {
                plainText = "FINISHED=TRUE";
                finished = true;
                break;
            }
            case AchievementSystem.ProgressType.RepeatInteger:
            {
                //找到CURRENT=的位置，解析出=后面的数字，加上amount，重新加密
                // 提取出等号后面的数字
                int startIndex = plainText.IndexOf("=") + 1;
                
                int currentValue = int.Parse(plainText.Substring(startIndex));

                // 进行加法运算
                currentValue += amount;

                // 构造新的字符串并返回
                plainText = "CURRENT=" + currentValue.ToString();

                if (currentValue >= maxProgress)
                {
                    finished = true;
                }

                break;
            }
            case AchievementSystem.ProgressType.DistinctArray:
            {
                //plainText的格式为LIST=/data1,data2,data3/, 把amount作为新的data加入到LIST中
                // 找到最后一个"/"的位置
                int endIndex = plainText.LastIndexOf("/");

                if (endIndex == -1)
                {
                    plainText = "LIST=//";
                    endIndex = 6;
                }

                string newList;
                int count = 0;
                
                if (plainText.Contains("," + amount.ToString() + ",") ||
                    plainText.Contains("/" + amount.ToString() + ",") ||
                    plainText.Contains("," + amount.ToString() + "/") ||
                    plainText.Contains("/" + amount.ToString() + "/"))
                {
                    // 如果存在，那么直接返回
                    return false;
                }

                // 判断列表是否为空
                if (plainText == "LIST=//")
                {
                    // 如果列表为空，那么直接插入新的数据
                    newList = plainText.Insert(endIndex, amount.ToString());
                }
                else
                {
                    // 如果列表不为空，那么在新的数据前添加一个逗号，然后插入
                    newList = plainText.Insert(endIndex, "," + amount.ToString());
                    
                    count = newList.Count(ch => ch == ',') + 1;
                    
                    Debug.Log("COUNT:"+count);

                }
                
                if(count >= maxProgress)
                {
                    finished = true;
                }
                
                plainText = newList;
                break;
            }

        }
        
        AchievementSystem.EncryptAchievementProcessString(this,plainText);
        plainProgressInfo = AchievementSystem.DecryptAchievementProcessString(this);
        

        return finished;
        
    }


}

public class AchievementSystem
{
    private const string IVString = "abcdefghij";

    public enum ProgressType
    {
        OneOffBoolean,
        RepeatInteger,
        DistinctArray
    }

    

    public static void EncryptAchievementProcessString(Achievement achievement, string plainText)
    {
        var type = achievement.progressType.ToString();
        
        
        var keyBytes = Encoding.UTF8.GetBytes(type + achievement.id.ToString("D3"));
        var ivBytes = Encoding.UTF8.GetBytes(achievement.id.ToString("D3")+IVString + achievement.id.ToString("D3"));
        Debug.Log(keyBytes);

        achievement.SetProgress(EncryptString(plainText,keyBytes,ivBytes));
    }
    
    public static string DecryptAchievementProcessString(Achievement achievement)
    {
        var type = achievement.progressType.ToString();
        
        var keyBytes = Encoding.UTF8.GetBytes(type + achievement.id.ToString("D3"));
        var ivBytes = Encoding.UTF8.GetBytes(achievement.id.ToString("D3")+IVString + achievement.id.ToString("D3"));

        if (achievement.GetProgress() == string.Empty)
        {
            if (achievement.progressType == ProgressType.DistinctArray)
            {
                EncryptAchievementProcessString(achievement,"LIST=//");
            }else if (achievement.progressType == ProgressType.RepeatInteger)
            {
                EncryptAchievementProcessString(achievement, "CURRENT=0");
            }
            else
            {
                EncryptAchievementProcessString(achievement, "FINISHED=FALSE");
            }
        }


        return DecryptString(achievement.GetProgress(), keyBytes, ivBytes);
    }
    
    private static string EncryptString(string plainText, byte[] key, byte[] iv)
    {
        using (AesCryptoServiceProvider aes = new AesCryptoServiceProvider())
        {
            aes.Key = key;
            aes.IV = iv;
 
            ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
 
            byte[] encryptedBytes = null;
 
            using (var ms = new System.IO.MemoryStream())
            {
                using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                {
                    byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
                    cs.Write(plainBytes, 0, plainBytes.Length);
                }
                encryptedBytes = ms.ToArray();
            }
 
            return Convert.ToBase64String(encryptedBytes);
        }
    }
    
    
    private static string DecryptString(string encryptedText, byte[] key, byte[] iv)
    {
        Debug.Log(encryptedText);
        
        using (AesCryptoServiceProvider aes = new AesCryptoServiceProvider())
        {
            aes.Key = key;
            aes.IV = iv;
 
            ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
 
            byte[] encryptedBytes = Convert.FromBase64String(encryptedText);
            byte[] decryptedBytes;

            try
            {
                using (var ms = new System.IO.MemoryStream(encryptedBytes))
                {
                    using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                    {
                        byte[] buffer = new byte[encryptedBytes.Length];
                        int bytesRead = cs.Read(buffer, 0, buffer.Length);
                        decryptedBytes = new byte[bytesRead];
                        Array.Copy(buffer, decryptedBytes, bytesRead);
                        //Array.Copy(buffer, 0, decryptedBytes, 0, bytesRead);
                    }
                }
            }
            catch
            {
                return "DEFAULT";
            }

            return Encoding.UTF8.GetString(decryptedBytes);
        }
    }
}


[Serializable]
public class AchievementInfo
{
    public int id;
    public string progressStr;
    
    public AchievementInfo(int id, string progressStr)
    {
        this.id = id;
        this.progressStr = progressStr;
    }
}