using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class ClassSelectManager : MonoBehaviour {
    SocketNetworkManager snm;
    public GameObject gm;
    public GameObject buttonArcher;
    public GameObject buttonKnight;
    public GameObject buttonPriest;
    public GameObject buttonUndo;
    public Text areaText;
    public Dictionary<string, int> plord = new Dictionary<string, int>();

    private Dictionary<string, GameObject> clToObj = new Dictionary<string, GameObject>();
    private int count = 0;
    private GameObject selected = null;

	void Start () {
        snm = new SocketNetworkManager();
        clToObj["Archer"] = buttonArcher;
        clToObj["Knight"] = buttonKnight;
        clToObj["Priest"] = buttonPriest;
        clToObj["None"] = null;
        plord[SocketNetworkManager.id] = 0;
        foreach (KeyValuePair<string, newPly> a in SocketNetworkManager.newplayers)
        {
            Debug.Log("update from start");
            if (a.Value.theirid != SocketNetworkManager.id)
                StartCoroutine(UpdateClassHandle(a.Value.theirid, a.Value._plclass));
        }
    }

    private void OnEnable()
    {
        SocketNetworkManager.SelectClassHandle += SelectClassHandle;
        SocketNetworkManager.UpdateClassHandle += UpdateClassHandle;
    }
    private void OnDisable()
    {
        SocketNetworkManager.SelectClassHandle -= SelectClassHandle;
        SocketNetworkManager.UpdateClassHandle -= UpdateClassHandle;
    }

    IEnumerator SelectClassHandle(string ret, string plclass)
    {
        Debug.Log("class select handle");
        if (ret == "fail")
        {
            areaText.text = "Failed to select that class";
        }
        else
        {
            areaText.text = "";
            SocketNetworkManager.newplayers[SocketNetworkManager.id]._plclass = plclass;
            Debug.Log("You " + plclass);
            GameObject o = clToObj[plclass];
            if (selected != null)
                selected.transform.GetChild(1).gameObject.GetComponent<Text>().text = "";
            selected = o;
            if (o == null)
            {
            }
            else
            {
                o.transform.GetChild(1).gameObject.GetComponent<Text>().text = "You";
            }
        }
        yield return null;
    }

    IEnumerator UpdateClassHandle(string player, string plclass)
    {
        GameObject o = clToObj[plclass];
        if (!plord.ContainsKey(player))
        {
            count++;
            plord[player] = count;
        }
        if (SocketNetworkManager.newplayers[player]._plclass != null)
            if (clToObj[SocketNetworkManager.newplayers[player]._plclass] != null)
                clToObj[SocketNetworkManager.newplayers[player]._plclass].transform.GetChild(1).gameObject.GetComponent<Text>().text = "";
        SocketNetworkManager.newplayers[player]._plclass = plclass;
        Debug.Log(player + " " + plclass);
        if (o != null)
            o.transform.GetChild(1).gameObject.GetComponent<Text>().text = "Player " + plord[player];
        yield return null;
    }

    // Update is called once per frame
    void Update () {
		
	}

    public void OnButtonArcherClick()
    {
        Debug.Log("archer send");
        snm.selectClass("Archer");
    }

    public void OnButtonKnightClick()
    {
        Debug.Log("knight send");
        snm.selectClass("Knight");
    }

    public void OnButtonPriestClick()
    {
        Debug.Log("priest send");
        snm.selectClass("Priest");
    }

    public void OnButtonUndoClick()
    {
        Debug.Log("none send");
        snm.selectClass("None");
    }
}
