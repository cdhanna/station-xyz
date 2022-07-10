using System.Collections;
using System.Collections.Generic;
using StationXYZ.Core;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameScenes : MonoBehaviour
{
    public TextMeshProUGUI roomName;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadGame()
    {
        MultiplayerDriver.useStaticName = true;
        MultiplayerDriver.staticRoomName = roomName.text;
        SceneManager.LoadScene("Movement");
    }
}
