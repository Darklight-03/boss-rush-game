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