using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllableVerts : MonoBehaviour
{

    [SerializeField]
    float strength = .14f;

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

    MeshCollider collider;

    Vector3[] basePos;

    public Vector2 power { get; set; }

    float maxDist;


    private void Awake()
    {
        mainCamera = Camera.main;
        collider = GetComponent<MeshCollider>();
        meshFilter = GetComponent<MeshFilter>();
        Mesh mesh = meshFilter.mesh;
        basePos = mesh.vertices;
        power = Vector2.zero;
        maxDist = .67f * transform.localScale.x;
    }

    private void Update()
    {
        if (!returning)
        {
            Vector2 m = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            if (trackingMousePos)
            {
                mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
                if (Vector2.Distance(mousePos,clickPos) > maxDist)
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
                movedVert = true;
                prevMovingVertPos = vert;
                if (clicked)
                {
                    vert = transform.InverseTransformPoint(mousePos );
                }
                else
                {
                    vert = Vector2.Lerp(transform.InverseTransformPoint(mousePos), baseVert,lerpValue);
                }
                movingVertPos = vert;
            }
            else
            {
                vert = baseVert;
            }
            verts[i] = new Vector3(vert.x, vert.y, basePos[i].z);
        }
        mesh.vertices = verts;
        collider.sharedMesh = mesh;
        if (movedVert)
        {
            power = (((movingVertPos - prevMovingVertPos) * strength) / Time.fixedDeltaTime);
        }
        else
        {
            power = Vector2.zero;
        }
    }

    IEnumerator goBack()
    {
        returning = true;
        power = ((clickPos - mousePos) * strength)*5f;
        lerpValue = 0;
        yield return new WaitUntil(goingBack);
        lerpValue = 1;
        clickPos = mousePos;
        returning = false;
    }

    bool goingBack()
    {
        lerpValue += Time.deltaTime * 5f;
        return lerpValue >= 1;
    }

}
