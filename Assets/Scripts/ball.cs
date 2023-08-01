using UnityEditor.UIElements;
using UnityEngine;

public class ball : MonoBehaviour
{
    Rigidbody2D rb;

    float lastHitTime;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        lastHitTime += Time.deltaTime;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        GameObject other = collision.gameObject;
        if (other.tag == "movingVert")
        {
            ControllableVerts mv = other.GetComponentInParent<ControllableVerts>();
            int[] edgeIndex = other.GetComponent<MovableEdge>().index;
            Vector2 avgNormal = Vector3.zero;
            ContactPoint2D[] contacts = new ContactPoint2D[collision.contactCount];
            collision.GetContacts(contacts);
            for (int i = 0; i < contacts.Length; i++)
            {
                ContactPoint2D contact = contacts[i];
                avgNormal += contact.normal;
            }
            avgNormal = avgNormal / contacts.Length;
            float distfromVert1 = Vector2.Distance( mv.getVertexPosition(edgeIndex[0]), contacts[0].point);
            float distfromVert2 = Vector2.Distance(mv.getVertexPosition(edgeIndex[1]), contacts[0].point);
            float lerpValue = distfromVert1/(distfromVert1+distfromVert2);
            Vector2 power = Vector2.Lerp(mv.power[edgeIndex[0]], mv.power[edgeIndex[1]],lerpValue);
            rb.AddForce(power * Mathf.Max(Vector3.Dot(power.normalized, avgNormal), 0));
            lastHitTime = 0;
        }
    }
}
