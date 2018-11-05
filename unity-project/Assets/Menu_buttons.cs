using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu_buttons : MonoBehaviour { 
    public GameObject MenuPanel;
    public GameObject LobbySelectPanel;
    public GameObject LobbyCreatePanel;

    // Use this for initialization
    void Start () 
    {
        MenuPanel.SetActive(true);
        LobbySelectPanel.SetActive(false);
        LobbyCreatePanel.SetActive(false);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ShowLobbyPanel()
    {
        MenuPanel.SetActive(false);
        LobbySelectPanel.SetActive(true);
        LobbyCreatePanel.SetActive(false);
    }

    public void ShowMenuPanel()
    {
        MenuPanel.SetActive(true);
        LobbySelectPanel.SetActive(false);
        LobbyCreatePanel.SetActive(false);
    }

    public void ShowCreateLobbyPanel()
    {
        MenuPanel.SetActive(false);
        LobbySelectPanel.SetActive(false);
        LobbyCreatePanel.SetActive(true);
    }



}
