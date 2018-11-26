using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DaoGuangAnimation : MonoBehaviour {

    private LineRenderer renderer;
    public Material rendererMaterial;
    private List<Vector3> pointLists;
    private bool isDrawing = false;
    public Transform wepon;
    public bool state;


    // Use this for initialization
    void Start()
    {
        pointLists = new List<Vector3>();
        renderer = new LineRenderer();
    }

    // Update is called once per frame
    void Update()
    {
        if (state)
        {
            GameObject lineObj = new GameObject();
            lineObj.transform.SetParent(this.transform);
            renderer = lineObj.AddComponent<LineRenderer>();
            renderer.material = rendererMaterial;
            renderer.startColor = Color.red;
            renderer.endColor = Color.red;
            //renderer.widthMultiplier = 0.1f;
            renderer.startWidth = 0.1f;
            renderer.endWidth = 0.1f;


            DrawLineByPoint();

            isDrawing = true;

            Debug.Log("start painting");
        }

        if (!state)
        {
            isDrawing = false;
            pointLists.Clear();

            Debug.Log("end painting");
        }

        if (isDrawing)
        {
            DrawLineByPoint();

            renderer.positionCount = pointLists.Count;
            renderer.SetPositions(pointLists.ToArray());

            Debug.Log("painting");
        }
    }

    private void DrawLineByPoint()
    {
        Ray ray = Camera.main.ScreenPointToRay(wepon.localPosition);
        RaycastHit hit;
        bool isHit = Physics.Raycast(ray, out hit);
        if (isHit)
        {
            Vector3 linePoint = new Vector3(hit.point.x, 0, hit.point.z);
            pointLists.Add(linePoint);
        }
    }
}
