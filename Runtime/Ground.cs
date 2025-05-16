using UnityEngine;
using OojuInteractionPlugin;

namespace OojuXRPlugin
{
    [RequireComponent(typeof(Collider))]
    public class Ground : MonoBehaviour
    {
        private void OnCollisionEnter(Collision collision)
        {
            // Check if the colliding object has an ObjectAutoAnimator
            var animator = collision.gameObject.GetComponent<ObjectAutoAnimator>();
            if (animator != null)
            {
                // (Removed) Stop the animation when the object hits the ground
                // animator.StopAnimation();
            }
        }

        private void OnCollisionStay(Collision collision)
        {
            // Prevent objects from going through the ground
            var animator = collision.gameObject.GetComponent<ObjectAutoAnimator>();
            if (animator != null)
            {
                // Keep the object above the ground
                Vector3 position = collision.transform.position;
                position.y = Mathf.Max(position.y, transform.position.y + collision.collider.bounds.extents.y);
                collision.transform.position = position;
            }
        }
    }
} 