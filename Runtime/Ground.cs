using UnityEngine;

namespace OojuXRPlugin
{
    [RequireComponent(typeof(Collider))]
    public class Ground : MonoBehaviour
    {
        private void OnCollisionStay(Collision collision)
        {
            // Prevent objects from going through the ground
            Vector3 position = collision.transform.position;
            position.y = Mathf.Max(position.y, transform.position.y + collision.collider.bounds.extents.y);
            collision.transform.position = position;
        }
    }
} 