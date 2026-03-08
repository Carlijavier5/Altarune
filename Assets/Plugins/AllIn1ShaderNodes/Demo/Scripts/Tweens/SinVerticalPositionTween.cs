using System;
using UnityEngine;

namespace AllIn1ShaderNodes
{
    public class SinVerticalPositionTween : MonoBehaviour
    {
       private float timer;

       public float speed = 5;
       public float minY = -1;
       public float maxY = 1;

       public Transform target;

       private void OnEnable()
       {
          timer = 0f;
       }

       private void OnDisable()
       {
          timer = 0f;
       }

       public void Update()
       {
          timer += Time.deltaTime;

          float sin01 = (Mathf.Sin(timer * speed) + 1f) * 0.5f;
          Vector3 localPos = target.localPosition;
          localPos.y = Mathf.Lerp(minY, maxY, sin01);

          target.localPosition = localPos;
       }

       private void Reset()
       {
          if(target == null) target = transform;
       }

       private void OnDrawGizmosSelected()
       {
          if(target == null) return;

          Vector3 worldPos = target.position;
          Vector3 parentScale = target.parent != null ? target.parent.lossyScale : Vector3.one;
          
          // Convert local min/max to world space
          float worldMinY = worldPos.y - (target.localPosition.y * parentScale.y) + (minY * parentScale.y);
          float worldMaxY = worldPos.y - (target.localPosition.y * parentScale.y) + (maxY * parentScale.y);

          // Draw range indicators
          Gizmos.color = Color.green;
          Vector3 minPoint = new Vector3(worldPos.x, worldMinY, worldPos.z);
          Vector3 maxPoint = new Vector3(worldPos.x, worldMaxY, worldPos.z);
          
          // Draw min/max positions as small spheres
          Gizmos.DrawWireSphere(minPoint, 0.1f);
          Gizmos.DrawWireSphere(maxPoint, 0.1f);
          
          // Draw connecting line to show range
          Gizmos.color = Color.yellow;
          Gizmos.DrawLine(minPoint, maxPoint);
          
          // Draw current position
          Gizmos.color = Color.red;
          Gizmos.DrawWireSphere(worldPos, 0.05f);
       }
    }
}