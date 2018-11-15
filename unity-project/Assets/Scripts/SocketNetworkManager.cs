using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SocketNetworkManager : MonoBehaviour
{
    public static WebSocket w;
    public static int lobbyid = -1;
    static bool contli;
    static bool started = false;
    public static string id;
    public static bool isHost = true;
    public static int playernum;
    private static int instances = 0;
    public static string serverurl = "ws://teamproject1.ddns.net:3000/";
    public static Queue<newPly> newplayers = new Queue<newPly>();
    public static int numberofplayers = 1;
    private static Queue<string> logqueue = new Queue<string>();
    private static PlayerLog eventLog;

    // events
    public delegate void OtherPlayerPos(string sender, float x, float y, float rx, float ry);
    public static event OtherPlayerPos UpdateOtherPlayerPos;

    public delegate IEnumerator CreateLobbyRes(int lobbyid, int playernum);
    public static event CreateLobbyRes CreateLobbyHandle;

    public delegate IEnumerator JoinLobbyRes(int lobbyid, int playernum, string ret);
    public static event JoinLobbyRes JoinLobbyHandle;

    public delegate IEnumerator GetLobbiesRes(lobbyInfo[] list); 
    public static event GetLobbiesRes GetLobbiesHandle;

    public delegate IEnumerator NewPlayerRes(newPly newplayer);
    public static event NewPlayerRes NewPlayerHandle;

    public delegate IEnumerator StartGameRes();
    public static event StartGameRes StartGameHandle;

    public delegate void TakeDamageRes(string sender, float dmg);
    public static event TakeDamageRes TakeDamageHandle;

    public delegate void DealDamageRes(string sender, float dmg, Vector2 dir);
    public static event DealDamageRes DealDamageHandle;

    public delegate IEnumerator BossPositionRes(float x, float y, float rx, float ry);
    public static event BossPositionRes UpdateBossPositionHandle;

    public delegate void PlayerAnimRes(string sender, string name);
    public static event PlayerAnimRes PlayerAnimHandle;
    
    public delegate IEnumerator BossAnimRes(string name);
    public static event BossAnimRes BossAnimHandle;

    public delegate void SpawnProjRes(string sender, string name, Vector2 pos, Vector2 dir);
    public static event SpawnProjRes SpawnProjHandle;

    public delegate IEnumerator BossDeadRes();
    public static event BossDeadRes BossDeadHandle;




    // Use this for initialization
    IEnumerator Start()
    {
        instances++;
        if (!started)
        {
            started = true;
            w = new WebSocket(new Uri("ws://168.61.48.136:3000/"));
            yield return StartCoroutine(w.Connect());
            contli = true;
            StartCoroutine(listener());
            yield return null;
        }
    }

    void OnApplicationQuit()
    {
        contli = false;
        w.Close();
    }

    public void createLobby(string lobbyname)
    {
        w.SendString("{ \"msgtype\":\"create lobby\", \"name\": \"" + lobbyname + "\" }");
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

    public void getLobbies()
    {
        w.SendString("{ \"msgtype\":\"get lobbies\" }");
    }

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    public void logText(string text)
    {
        if (eventLog == null)
        {
            GameObject log = GameObject.Find("PlayerLog");
            if (log != null)
            {
                eventLog = log.GetComponent<PlayerLog>();
            }
        }
        if (eventLog == null)
        {
            logqueue.Enqueue(text);
        }
        else
        {
            while (logqueue.Count > 0)
            {
                eventLog.AddEvent(logqueue.Dequeue());
            }
            eventLog.AddEvent(text);
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
                switch (msgo.msgtype)
                {
                    case "new connection":
                        newCon nc = JsonUtility.FromJson<newCon>(msgo.content);
                        SocketNetworkManager.id = nc.yourid;
                        break;

                    case "new player":
                        newPly np = JsonUtility.FromJson<newPly>(msgo.content);
                        newplayers.Enqueue(np);
                        numberofplayers++;
                        if (NewPlayerHandle != null)
                            StartCoroutine(NewPlayerHandle(np));
                        break;
                    case "create lobby":
                        creLobby crel = JsonUtility.FromJson<creLobby>(msgo.content);
                        if (CreateLobbyHandle != null)
                            StartCoroutine(CreateLobbyHandle(crel.lobbyid, crel.playernum));
                        break;

                    case "join lobby":
                        joinLobby jnl = JsonUtility.FromJson<joinLobby>(msgo.content);
                        if (JoinLobbyHandle != null)
                            StartCoroutine(JoinLobbyHandle(jnl.lobbyid, jnl.playernum, jnl.ret));
                        break;

                    case "get lobbies":
                        lobbyList ll = JsonUtility.FromJson<lobbyList>(msgo.content);
                        if (GetLobbiesHandle != null)
                        {
                            StartCoroutine(GetLobbiesHandle(ll.lobbiesInfo));
                        }
                        break;

                    case "general message":
                        genMess gms = JsonUtility.FromJson<genMess>(msgo.content);
                        switch (gms.ct)
                        {
                            case "playerposition": // player position
                                playerPos pp = JsonUtility.FromJson<playerPos>(gms.content);
                                if (UpdateOtherPlayerPos != null)
                                    UpdateOtherPlayerPos(gms.sender, pp.x, pp.y, pp.rx, pp.ry);
                                break;

                            case "startgame": // start game
                                if (StartGameHandle != null)
                                    StartCoroutine(StartGameHandle());
                                break;

                            case "takedamage": // (from boss)
                                opTakeDam otd = JsonUtility.FromJson<opTakeDam>(gms.content);
                                if (TakeDamageHandle != null)
                                    TakeDamageHandle(gms.sender, otd.dmg);
                                break;

                            case "dealdamage": // (to boss)
                                opDealDam odd = JsonUtility.FromJson<opDealDam>(gms.content);
                                if (DealDamageHandle != null)
                                    DealDamageHandle(gms.sender, odd.dmg, new Vector2(odd.dirx, odd.diry));
                                break;

                            case "bossposition": // boss position
                                bossPos bp = JsonUtility.FromJson<bossPos>(gms.content);
                                if (UpdateBossPositionHandle != null)
                                    StartCoroutine(UpdateBossPositionHandle(bp.x, bp.y, bp.rx, bp.ry));
                                break;

                            case "playeranimation": // player animation
                                playerAnim pa = JsonUtility.FromJson<playerAnim>(gms.content);
                                if (PlayerAnimHandle != null)
                                    PlayerAnimHandle(gms.sender, pa.name);
                                break;

                            case "bossanimation": // boss animation
                                bossAnim ba = JsonUtility.FromJson<bossAnim>(gms.content);
                                if (BossAnimHandle != null)
                                    StartCoroutine(BossAnimHandle(ba.name));
                                break;

                            case "spawnprojectile": // spawn projectile
                                spawnProj sp = JsonUtility.FromJson<spawnProj>(gms.content);
                                if (SpawnProjHandle != null)
                                    SpawnProjHandle(gms.sender, sp.name, new Vector2(sp.x, sp.y), new Vector2(sp.rx, sp.ry));
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
    public int theirnum;
    public string theirid;
    public int cl;
}

[Serializable]
public class creLobby
{
    public int playernum;
    public int lobbyid;
}

[Serializable]
public class joinLobby
{
    public int lobbyid;
    public int playernum;
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
public class lobbyInfo
{
    public int players;
		public string name;
}

[Serializable]
public class lobbyList
{
    public lobbyInfo[] lobbiesInfo;
}

[Serializable]
public class playerPos
{
    public float x;
    public float y;
    public float rx;
    public float ry;
}

[Serializable]
public class opTakeDam
{
    public float dmg;
}

[Serializable]
public class opDealDam
{
    public float dmg;
    public float dirx;
    public float diry;
}

[Serializable]
public class bossPos
{
    public float x;
    public float y;
    public float rx;
    public float ry;
}

[Serializable]
public class playerAnim
{
    public string name;
}

[Serializable]
public class bossAnim
{
    public string name;
}

[Serializable]
public class spawnProj
{
    public string name;
    public float x;
    public float y;
    public float rx;
    public float ry;
}
