using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class Menu_buttons : MonoBehaviour { 
    public GameObject MenuPanel;
    public GameObject LobbySelectPanel;
    public GameObject LobbyCreatePanel;
    public GameObject dropdown;
    public GameObject LobbySelectErrorText;
    public GameObject LobbyCreateErrorText;
    public GameObject DropdownLabel;
    Dropdown droplist;
    int droplistprev = 55;
    SocketNetworkManager snm;
		string lobbyname = "";

    // Use this for initialization
    void Start () 
    {
        snm = GetComponent<SocketNetworkManager>();
        MenuPanel.SetActive(true);
        LobbySelectPanel.SetActive(false);
        LobbyCreatePanel.SetActive(false);
        droplist = dropdown.GetComponent<Dropdown>();
    }

    private void OnEnable()
    {
        SocketNetworkManager.GetLobbiesHandle += GetLobbiesHandle;
        SocketNetworkManager.CreateLobbyHandle += CreateLobbyHandle;
        SocketNetworkManager.JoinLobbyHandle += JoinLobbyHandle;
    }

    private void OnDisable()
    {
        SocketNetworkManager.GetLobbiesHandle -= GetLobbiesHandle;
        SocketNetworkManager.CreateLobbyHandle -= CreateLobbyHandle;
        SocketNetworkManager.JoinLobbyHandle -= JoinLobbyHandle;
    }

    // Update is called once per frame
    void Update()
    {
        dropdownchk();
    }

    public void dropdownchk()
    {
        if (droplist.value != droplistprev)
        {
            droplistprev = droplist.value;
            selectvalue(droplist.value);
        }
    }



    public void ShowLobbyPanel()
    {
        snm.getLobbies();
        MenuPanel.SetActive(false);
        LobbySelectPanel.SetActive(true);
        LobbyCreatePanel.SetActive(false);
    }

    IEnumerator GetLobbiesHandle(lobbyInfo[] listoflobbies)
    {
        //Debug.Log("get lobby handling");
        for (int i = 0; i < listoflobbies.Length; i++)
        {
            //Debug.Log(listoflobbies[i].players);
            if (droplist.options.Count == i)
            {
                droplist.options.Add(new Dropdown.OptionData());
            }
            string newtext = "";
            newtext += (i + 1) + ". " + listoflobbies[i].name + ": " + listoflobbies[i].players + "/3 players";
            droplist.options[i].text = newtext;
        }
        if (droplist.options.Count > listoflobbies.Length)
        {
            int diff = droplist.options.Count - listoflobbies.Length;
            droplist.options.RemoveRange(listoflobbies.Length, diff);
        }
        selectvalue(0);
        yield break;
    }

    IEnumerator JoinLobbyHandle(int lobbyid, int playernum, string ret)
    {
        if (ret == "fail")
        {
            Text lset = LobbySelectErrorText.GetComponent<Text>();
            lset.text = "Failed to join lobby";
        }
        else
        {
            SocketNetworkManager.lobbyid = lobbyid;
            SocketNetworkManager.playernum = playernum;
            SocketNetworkManager.isHost = false;
            SceneManager.LoadScene("SampleScene");
        }
        yield break;
    }

    void selectvalue(int newvalue)
    {
        if (droplist.options.Count <= newvalue)
        {
            return;
        }
        Text labeltext = DropdownLabel.GetComponent<Text>();
        labeltext.text = droplist.options[newvalue].text;
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

    public void GoToNewLobby()
    {
				if (lobbyname == "")
				{
						Text lcre = LobbyCreateErrorText.GetComponent<Text>();
            lcre.text = "Please enter a name";
				}
				else
				{
        		snm.createLobby();
				}
    }

		public void OnEndEdit(Text afteredit)
		{
				lobbyname = afteredit.text;
		}

    public void GoToSelectedLobby()
    {
        snm.joinLobby(droplist.value);
    }

    IEnumerator CreateLobbyHandle(int lobbyid, int playernum)
    {
        if (lobbyid == -1)
        {
            Text lcre = LobbyCreateErrorText.GetComponent<Text>();
            lcre.text = "Failed to create lobby";
        }
        else
        {
            SocketNetworkManager.lobbyid = lobbyid;
            SocketNetworkManager.playernum = playernum;
            SceneManager.LoadScene("SampleScene");
        }
        yield break;
    }


}
