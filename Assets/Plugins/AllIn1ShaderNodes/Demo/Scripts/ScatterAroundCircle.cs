using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AllIn1ShaderNodes
{
    public class ScatterAroundCircle : MonoBehaviour
    {
        [SerializeField] private float radius = 5f;
        [SerializeField] private float height = 0f;

        #if UNITY_EDITOR
        [ContextMenu("Scatter Children")]
        public void ScatterChildren()
        {
            int childCount = transform.childCount;
            if(childCount == 0) return;

            float angleStep = 360f / childCount;
            
            for(int i = 0; i < childCount; i++)
            {
                Transform child = transform.GetChild(i);
                // Start from -90 degrees to place child 0 at X=0 (forward)
                float angle = (i * angleStep - 90f) * Mathf.Deg2Rad;
                
                Vector3 localPosition = new Vector3(
                    Mathf.Cos(angle) * radius,
                    height,
                    Mathf.Sin(angle) * radius
                );
                
                // Apply parent rotation to the position
                Vector3 rotatedPosition = transform.rotation * localPosition;
                child.position = transform.position + rotatedPosition;
                child.forward = -Vector3.forward;
                EditorUtility.SetDirty(child);
            }
        }
        #endif

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            
            Vector3 center = transform.position + transform.rotation * Vector3.up * height;
            
            // Draw circle at the specified height
            int segments = 32;
            float angleStep = 360f / segments;
            Vector3 firstLocalPoint = new Vector3(Mathf.Cos(0) * radius, 0f, Mathf.Sin(0) * radius);
            Vector3 previousPoint = center + transform.rotation * firstLocalPoint;
            
            for(int i = 1; i <= segments; i++)
            {
                float angle = i * angleStep * Mathf.Deg2Rad;
                Vector3 localPoint = new Vector3(
                    Mathf.Cos(angle) * radius,
                    0f,
                    Mathf.Sin(angle) * radius
                );
                Vector3 currentPoint = center + transform.rotation * localPoint;
                
                Gizmos.DrawLine(previousPoint, currentPoint);
                previousPoint = currentPoint;
            }
            
            // Draw height indicator
            if(height != 0f)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(transform.position, center);
            }
            
            // Draw child positions
            if(transform.childCount > 0)
            {
                Gizmos.color = Color.red;
                float childAngleStep = 360f / transform.childCount;
                
                for(int i = 0; i < transform.childCount; i++)
                {
                    // Match the same angle calculation as ScatterChildren
                    float angle = (i * childAngleStep - 90f) * Mathf.Deg2Rad;
                    Vector3 localChildPos = new Vector3(
                        Mathf.Cos(angle) * radius,
                        height,
                        Mathf.Sin(angle) * radius
                    );
                    Vector3 worldChildPos = transform.position + transform.rotation * localChildPos;
                    
                    Gizmos.DrawWireSphere(worldChildPos, 0.2f);
                }
            }
        }
    }
}