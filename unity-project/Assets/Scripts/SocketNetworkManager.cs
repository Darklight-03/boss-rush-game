using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SocketNetworkManager : MonoBehaviour
{
    static WebSocket w;
    static int lobbyid = -1;
    public static string id;
    static bool contli;
    static bool started = false;
    public string serverurl = "ws://168.61.48.136:3000/";

    // events
    public delegate void OtherPlayerPos(string id, float x, float y, float rx, float ry);
    public static event OtherPlayerPos UpdateOtherPlayerPos;

    public delegate void CreateLobbyRes(int lobbyid);
    public static event CreateLobbyRes CreateLobbyHandle;

    public delegate void JoinLobbyRes(string ret);
    public static event JoinLobbyRes JoinLobbyHandle;

    public delegate void NewPlayerRes(string id, int cl);
    public static event NewPlayerRes NewPlayerHandle;


    // Use this for initialization
    IEnumerator Start()
    {
        if (!started)
        {
            started = true;
            Debug.Log(serverurl);
            w = new WebSocket(new Uri("ws://168.61.48.136:3000/"));
            yield return StartCoroutine(w.Connect());
            //Debug.Log(w.Connect());
            contli = true;
            StartCoroutine(listener());
            Debug.Log(w.error);
            yield return null;
        }
    }

    void OnDestroy()
    {
        contli = false;
        w.Close();
    }

    public void createLobby()
    {
        w.SendString("{ \"msgtype\":\"create lobby\" }");
    }

    public void joinLobby(int lobbyid)
    {
        SocketNetworkManager.lobbyid = lobbyid;
        w.SendString("{ \"msgtype\":\"join lobby\", \"lobbyid\": " + lobbyid.ToString() + " }");
    }

    public void sendMessage(string contenttype, string content)
    {
        if (lobbyid != -1)
        {
            w.SendString("{ \"msgtype\":\"general message\", \"lobbyid\": " + lobbyid.ToString() + ", \"ct\": \"" + contenttype + "\", \"content\": " + content + " }");
        }
    }

    IEnumerator listener()
    {
        string msg;
        while (contli)
        {
            msg = w.RecvString();
            if (msg != null)
            {
                //Debug.Log("raw: " + msg);
                mess msgo = JsonUtility.FromJson<mess>(msg);
                //Debug.Log("content " + msgo.content);
                switch (msgo.msgtype)
                {
                    case "new connection":
                        newCon nc = JsonUtility.FromJson<newCon>(msgo.content);
                        SocketNetworkManager.id = nc.yourid;
                        break;

                    case "new player":
                        newPly np = JsonUtility.FromJson<newPly>(msgo.content);
                        if (NewPlayerHandle != null)
                            NewPlayerHandle(np.theirid, np.cl);
                        break;
                    case "create lobby":
                        creLobby crel = JsonUtility.FromJson<creLobby>(msgo.content);
                        lobbyid = crel.lobbyid;
                        Debug.Log("created lobby");
                        if (CreateLobbyHandle != null)
                            CreateLobbyHandle(crel.lobbyid);
                        break;

                    case "join lobby":
                        joinLobby jnl = JsonUtility.FromJson<joinLobby>(msgo.content);
                        Debug.Log("joined lobby");
                        if (JoinLobbyHandle != null)
                            JoinLobbyHandle(jnl.ret);
                        break;

                    case "general message":
                        genMess gms = JsonUtility.FromJson<genMess>(msgo.content);
                        switch (gms.ct)
                        {
                            case "pp":
                                playerPos pp = JsonUtility.FromJson<playerPos>(gms.content);
                                if (UpdateOtherPlayerPos != null)
                                    UpdateOtherPlayerPos(gms.sender, pp.x, pp.y, pp.rx, pp.ry);
                                break;
                            default:
                                Debug.Log("unknown general message type");
                                break;
                        }
                        break;

                    default:
                        Debug.Log("unknown message type");
                        break;
                }
                msg = null;
            }
            yield return new WaitForEndOfFrame();
        }
    }
}


[Serializable]
public class mess
{
    public string msgtype;
    public string content;
}

[Serializable]
public class newCon
{
    public string yourid;
}

[Serializable]
public class newPly
{
    public string theirid;
    public int cl;
}

[Serializable]
public class creLobby
{
    public int lobbyid;
}

[Serializable]
public class joinLobby
{
    public string ret;
}

[Serializable]
public class genMess
{
    public string sender;
    public string ct;
    public string content;
}

[Serializable]
public class playerPos
{
    public float x;
    public float y;
    public float rx;
    public float ry;
}
