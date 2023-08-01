using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class ControllableVerts : MonoBehaviour
{

    [SerializeField]
    float strength = .14f;

    [SerializeField]
    MovableEdge middleEdge;

    Camera mainCamera;

    bool trackingMousePos;

    float lerpValue;
    bool returning = false;

    Vector2 mousePos;
    Vector2 prevMovingVertPos;
    Vector2 movingVertPos;
    Vector2 clickPos;
    bool clicked;
    bool movedVert;

    MeshFilter meshFilter;

    EdgeCollider2D[] edgeColliders;

    Vector3[] basePos;

    public Vector2[] power { get; set; }

    float maxDist;

    bool justclicked;

    int[] fourthEdgeIndexs;


    private void Awake()
    {
        mainCamera = Camera.main;
        meshFilter = GetComponent<MeshFilter>();
        Mesh mesh = meshFilter.mesh;
        basePos = mesh.vertices;
        power = new Vector2[5];
        power[0] = Vector2.zero;
        power[1] = Vector2.zero;
        power[2] = Vector2.zero;
        power[3] = Vector2.zero; 
        power[4] = Vector2.zero;
        maxDist = 4f * transform.localScale.x;
        edgeColliders = GetComponentsInChildren<EdgeCollider2D>();
        fourthEdgeIndexs = new int[2];
        fourthEdgeIndexs[0] = 3;
        fourthEdgeIndexs[0] = 0;
    }

    private void Update()
    {
        if (!returning)
        {
            Vector2 m = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            if (trackingMousePos)
            {
                mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
                if (Vector2.Distance(mousePos, clickPos) > maxDist)
                {
                    mousePos = clickPos + ((mousePos - clickPos).normalized * maxDist);
                }
            }
            if (Input.GetMouseButtonDown(0))
            {
                mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
                clickPos = mousePos;
                clicked = true;
                trackingMousePos = true;
                justclicked = true;
            }
            if (Input.GetMouseButtonUp(0))
            {
                clicked = false;
                trackingMousePos = false;
                StartCoroutine(goBack());
            }
        }
    }

    private void FixedUpdate()
    {
        movedVert = false;
        Mesh mesh = meshFilter.mesh;
        Vector3[] verts = mesh.vertices;
        for (int i = 0; i < mesh.vertexCount; i++)
        {
            Vector2 vert = verts[i];
            Vector2 baseVert = basePos[i];
            Vector2 baseVertWorld = transform.TransformPoint(baseVert);
            if (Vector2.Distance(baseVertWorld, clickPos) <= 1f)
            {
                if (justclicked)
                {
                    if (i == 1 || i == 2)
                    {
                        fourthEdgeIndexs[0] = 2;
                        fourthEdgeIndexs[1] = 1;
                        middleEdge.index = fourthEdgeIndexs;
                        mesh.triangles = new int[] { 2, 1, 0, 2, 3, 1 };
                    }
                    else
                    {
                        fourthEdgeIndexs[0] = 0;
                        fourthEdgeIndexs[1] = 3;
                        middleEdge.index = fourthEdgeIndexs;
                        mesh.triangles = new int[] { 0, 3, 1, 3, 0, 2 };
                    }
                }
                justclicked = false;
                movedVert = true;
                prevMovingVertPos = vert;
                if (clicked)
                {
                    vert = transform.InverseTransformPoint(mousePos);
                }
                else
                {
                    vert = Vector2.Lerp(transform.InverseTransformPoint(mousePos), baseVert, lerpValue);
                }
                movingVertPos = vert;
                power[i] = (((movingVertPos - prevMovingVertPos) * strength) / Time.fixedDeltaTime);
            }
            else
            {
                power[i] = Vector2.zero;
                vert = baseVert;
            }
            verts[i] = new Vector3(vert.x, vert.y, basePos[i].z);
        }


        mesh.vertices = verts;
        edgeColliders[0].SetPoints(new List<Vector2> { verts[0], verts[1] });
        edgeColliders[1].SetPoints(new List<Vector2> { verts[0], verts[2] });
        edgeColliders[2].SetPoints(new List<Vector2> { verts[3], verts[1] });
        edgeColliders[3].SetPoints(new List<Vector2> { verts[3], verts[2] });
        edgeColliders[4].SetPoints(new List<Vector2> { verts[fourthEdgeIndexs[0]], verts[fourthEdgeIndexs[1]] });
    }

    IEnumerator goBack()
    {
        returning = true;
        lerpValue = 0;
        yield return new WaitUntil(goingBack);
        lerpValue = 1;
        clickPos = mousePos;
        returning = false;
    }


    public Vector2 getVertexPosition(int index)
    {
        Mesh mesh = meshFilter.mesh;
        Vector3[] verts = mesh.vertices;
        return transform.TransformPoint(verts[index]);
    }
    bool goingBack()
    {
        lerpValue += Time.deltaTime * 5f;
        return lerpValue >= 1;
    }

}
