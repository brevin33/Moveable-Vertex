using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ball : MonoBehaviour
{
    Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        GameObject other = collision.gameObject;
        if (other.tag == "movingVert")
        {
            Vector3 avgNormal = Vector3.zero;
            ContactPoint[] contacts = new ContactPoint[collision.contactCount];
            collision.GetContacts(contacts);
            for (int i = 0; i < contacts.Length; i++)
            {
                ContactPoint contact = contacts[i];
                avgNormal += contact.normal;
            }
            avgNormal = avgNormal / contacts.Length;
            ControllableVerts mv = other.GetComponent<ControllableVerts>();
            if(mv.inbox) {
                Debug.Log(Vector3.Dot(mv.power.normalized, avgNormal.normalized));
                rb.AddForce( mv.power * Vector3.Dot(mv.power.normalized,avgNormal));
            }
        }
    }
}
