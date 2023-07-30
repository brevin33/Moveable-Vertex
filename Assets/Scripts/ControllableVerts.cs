using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllableVerts : MonoBehaviour
{

    Camera mainCamera;

    bool trackingMousePos;

    float lerpValue;
    bool returning = false;

    Vector2 mousePos;
    Vector2 clickPos;
    bool clicked;

    MeshFilter meshFilter;

    MeshCollider collider;

    Vector3[] basePos;


    private void Awake()
    {
        mainCamera = Camera.main;
        collider = GetComponent<MeshCollider>();
        meshFilter = GetComponent<MeshFilter>();
        Mesh mesh = meshFilter.mesh;
        basePos = mesh.vertices;
    }

    private void Update()
    {
        if (!returning)
        {
            Vector2 m = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            if (trackingMousePos)
            {
                mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
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

        Mesh mesh = meshFilter.mesh;
        Vector3[] verts = mesh.vertices;
        for (int i = 0; i < mesh.vertexCount; i++)
        {
            Vector2 vert = verts[i];
            Vector2 baseVert = basePos[i];
            Vector2 baseVertWorld = transform.TransformPoint(baseVert);
            if (Vector2.Distance(baseVertWorld, clickPos) <= .6f)
            {
                if (clicked)
                {
                    vert = transform.InverseTransformPoint(mousePos );
                }
                else
                {
                    vert = Vector2.Lerp(transform.InverseTransformPoint(mousePos), baseVert,lerpValue);
                }
            }
            else
            {
                vert = baseVert;
            }
            verts[i] = new Vector3(vert.x, vert.y, basePos[i].z);
        }
        mesh.vertices = verts;
        collider.sharedMesh = mesh;
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

    bool goingBack()
    {
        lerpValue += Time.deltaTime * .5f;
        return lerpValue >= 1;
    }

}
