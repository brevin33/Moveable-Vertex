using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MovableVertex : MonoBehaviour
{
    [SerializeField]
    Material material;

    [SerializeField]
    Camera mainCamera;

    bool trackingMousePos;

    float lerpValue;
    bool returning = false;

    private void Update()
    {
        if (returning)
        {
            return;
        }
        Vector2 m = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Debug.Log(m);
        if (trackingMousePos)
        {
            Vector2 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            material.SetVector("_MousePos", mousePos);
        }
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            material.SetVector("_ClickPos", mousePos);
            material.SetInt("_Clicked", 1);
            material.SetVector("_MousePos", mousePos);
            trackingMousePos = true;
        }
        if (Input.GetMouseButtonUp(0))
        {
            material.SetInt("_Clicked", 0);
            trackingMousePos = false;
            StartCoroutine(goBack());
        }
    }


    IEnumerator goBack()
    {
        returning = true;
        material.SetFloat("_LerpBack", 0);
        lerpValue = 0;
        yield return new WaitUntil(goingBack);
        material.SetFloat("_LerpBack", 1);
        material.SetVector("_ClickPos", new Vector2(-9999, -9999));
        returning = false;
    }

    bool goingBack()
    {
        lerpValue += Time.deltaTime;
        material.SetFloat("_LerpBack", lerpValue);
        return lerpValue >= 1;
    }
}
