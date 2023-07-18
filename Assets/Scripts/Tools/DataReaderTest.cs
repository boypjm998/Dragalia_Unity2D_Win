using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using LitJson;




public class DataReaderTest : MonoBehaviour
{
    void GetJsonInfo()
    {
        string path = Application.streamingAssetsPath + "/test.json";
        StreamReader sr = new StreamReader(path);
        var str = sr.ReadToEnd();

        var jsmp = JsonMapper.ToObject(str);
        
        print(jsmp["CHARA_NAME_1"]["ZH"]);

        //JsonMapper.ToObject()


    }

    void Start()
    {
        //GetJsonInfo();
        //print(GetJsonInfo());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
