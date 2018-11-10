using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class PlayerLog : MonoBehaviour
{
    // Private VARS
    private SocketNetworkManager snm;
    private List<string> Eventlog = new List<string>();
    private string temptext = "";
    private Text textbox;

    // Public VARS
    public int maxLines = 10;
    public GameObject plLog;


    private void Start()
    {
        snm = GetComponent<SocketNetworkManager>();
        textbox = this.gameObject.GetComponent<Text>();
        if (SocketNetworkManager.isHost)
        {
            snm.logText("You are host");
            snm.logText("Waiting for more players (1/3)");
        }
        else
        {
            snm.logText("Waiting for more players (" + (SocketNetworkManager.numberofplayers + 1) + "/3)");
            if (SocketNetworkManager.numberofplayers == 3)
            {
                snm.logText("Ready to start");
            }
        }
    }

    IEnumerator NewPlayerHandle(newPly newplayer)
    {
        
        if (SocketNetworkManager.numberofplayers == 3)
        {
            snm.logText("Ready to start (3/3)");
        }
        else
        {
            snm.logText("Waiting for more players (" + (SocketNetworkManager.numberofplayers + 1) + "/3)");
        }
        yield break;
    }

    private void OnEnable()
    {
        SocketNetworkManager.NewPlayerHandle += NewPlayerHandle;
    }

    private void OnDisable()
    {
        SocketNetworkManager.NewPlayerHandle -= NewPlayerHandle;
    }

    public void AddEvent(string eventString)
    {
        Eventlog.Add(eventString);

        if (Eventlog.Count >= maxLines)
            Eventlog.RemoveAt(0);

        temptext = "";

        foreach (string logEvent in Eventlog)
        {
            temptext += logEvent;
            temptext += "\n";
        }
        textbox.text = temptext;
    }
}