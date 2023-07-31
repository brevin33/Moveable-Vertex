using UnityEditor.UIElements;
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
            ControllableVerts mv = other.GetComponent<ControllableVerts>();
            Vector3 avgNormal = Vector3.zero;
            ContactPoint[] contacts = new ContactPoint[collision.contactCount];
            collision.GetContacts(contacts);
            for (int i = 0; i < contacts.Length; i++)
            {
                ContactPoint contact = contacts[i];
                avgNormal += contact.normal;
            }
            avgNormal = avgNormal / contacts.Length;
            Debug.Log(Mathf.Max(Vector3.Dot(mv.power.normalized, avgNormal), 0));
            rb.AddForce(mv.power *  Mathf.Max(Vector3.Dot(mv.power.normalized, avgNormal),0));
        }
    }
}
