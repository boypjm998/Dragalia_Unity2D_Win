using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalControllerTestScene : GlobalController
{
    // Start is called before the first frame update
    void Start()
    {
        GameLanguage = Language.ZHCN;
        currentGameState = GameState.Inbattle;
        AssetBundle ab = AssetBundle.LoadFromFile(Application.streamingAssetsPath + "/voice_c001");
        loadedBundles.Add("voice_c001",ab);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
