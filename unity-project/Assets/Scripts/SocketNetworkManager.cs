using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SocketNetworkManager : MonoBehaviour
{
    static WebSocket w;
    static int lobbyid = -1;
    static bool contli;
    static bool started = false;
    public static string id;
    public static bool isHost = true;
    public static int playernum;
    public static string serverurl = "ws://teamproject1.ddns.net:3000/";

    // events
    public delegate IEnumerator OtherPlayerPos(string id, float x, float y, float rx, float ry);
    public static event OtherPlayerPos UpdateOtherPlayerPos;

    public delegate IEnumerator CreateLobbyRes(int lobbyid);
    public static event CreateLobbyRes CreateLobbyHandle;

    public delegate IEnumerator JoinLobbyRes(string ret);
    public static event JoinLobbyRes JoinLobbyHandle;

    public delegate IEnumerator NewPlayerRes(string id, int cl, int num);
    public static event NewPlayerRes NewPlayerHandle;

    public delegate IEnumerator StartGameRes();
    public static event StartGameRes StartGameHandle;

    public delegate IEnumerator TakeDamageRes(string sender, float dmg);
    public static event TakeDamageRes TakeDamageHandle;

    public delegate IEnumerator DealDamageRes(string sender, float dmg, Vector2 dir);
    public static event DealDamageRes DealDamageHandle;

    public delegate IEnumerator BossPositionRes(float x, float y, float rx, float ry);
    public static event BossPositionRes UpdateBossPositionHandle;

    public delegate IEnumerator PlayerAnimRes(string sender, string name);
    public static event PlayerAnimRes PlayerAnimHandle;
    
    public delegate IEnumerator BossAnimRes(string name);
    public static event BossAnimRes BossAnimHandle;

    public delegate IEnumerator SpawnProjRes(string sender, string name, Vector2 pos, Vector2 dir);
    public static event SpawnProjRes SpawnProjHandle;

    public delegate IEnumerator BossDeadRes();
    public static event BossDeadRes BossDeadHandle;


    // Use this for initialization
    IEnumerator Start()
    {
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
                            StartCoroutine(NewPlayerHandle(np.theirid, np.cl, np.theirnum));
                        break;
                    case "create lobby":
                        creLobby crel = JsonUtility.FromJson<creLobby>(msgo.content);
                        lobbyid = crel.lobbyid;
                        playernum = crel.playernum;
                        if (CreateLobbyHandle != null)
                            CreateLobbyHandle(crel.lobbyid);
                        break;

                    case "join lobby":
                        joinLobby jnl = JsonUtility.FromJson<joinLobby>(msgo.content);
                        isHost = false;
                        playernum = jnl.playernum;
                        if (JoinLobbyHandle != null)
                            JoinLobbyHandle(jnl.ret);
                        break;

                    case "general message":
                        genMess gms = JsonUtility.FromJson<genMess>(msgo.content);
                        switch (gms.ct)
                        {
                            case "pp": // player position
                                playerPos pp = JsonUtility.FromJson<playerPos>(gms.content);
                                if (UpdateOtherPlayerPos != null)
                                    StartCoroutine(UpdateOtherPlayerPos(gms.sender, pp.x, pp.y, pp.rx, pp.ry));
                                break;

                            case "sg": // start game
                                if (StartGameHandle != null)
                                    StartCoroutine(StartGameHandle());
                                break;

                            case "td": // take damage (from boss)
                                opTakeDam otd = JsonUtility.FromJson<opTakeDam>(gms.content);
                                if (TakeDamageHandle != null)
                                    StartCoroutine(TakeDamageHandle(gms.sender, otd.dmg));
                                break;

                            case "dd": // deal damage (to boss)
                                opDealDam odd = JsonUtility.FromJson<opDealDam>(gms.content);
                                if (DealDamageHandle != null)
                                    StartCoroutine(DealDamageHandle(gms.sender, odd.dmg, new Vector2(odd.dirx, odd.diry)));
                                break;

                            case "bp": // boss position
                                bossPos bp = JsonUtility.FromJson<bossPos>(gms.content);
                                if (UpdateBossPositionHandle != null)
                                    StartCoroutine(UpdateBossPositionHandle(bp.x, bp.y, bp.rx, bp.ry));
                                break;

                            case "pa": // player animation
                                playerAnim pa = JsonUtility.FromJson<playerAnim>(gms.content);
                                if (PlayerAnimHandle != null)
                                    StartCoroutine(PlayerAnimHandle(gms.sender, pa.name));
                                break;

                            case "ba": // boss animation
                                bossAnim ba = JsonUtility.FromJson<bossAnim>(gms.content);
                                if (BossAnimHandle != null)
                                    StartCoroutine(BossAnimHandle(ba.name));
                                break;

                            case "sp": // spawn projectile
                                spawnProj sp = JsonUtility.FromJson<spawnProj>(gms.content);
                                if (SpawnProjHandle != null)
                                    StartCoroutine(SpawnProjHandle(gms.sender, sp.name, new Vector2(sp.x, sp.y), new Vector2(sp.rx, sp.ry)));
                                break;

                            case "bd": // boss dead


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
