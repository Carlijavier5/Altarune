using UnityEngine;

namespace AllIn1ShaderNodes
{
    public class DemoExpositor : MonoBehaviour
    {
        private Camera cam;
        private DemoElementMovementState movementState;

        private DemoExpositorMovement demoExpositorMovement;
        private float lerpSpeed;
		 
        public PropertyTweenCollection tweenCollection;

        public Vector3 bounds = new Vector3(10f, 5f, 10f);

        public DemoElement[] demoElements;

#if UNITY_EDITOR
        public bool drawGizmos;
#endif

        private bool initialized;

        [Header("Config")]
        public string expositorName;
        [TextArea] public string expositorDescription;

        public void Init(Camera cam, float lerpSpeed)
        {
            this.cam = cam;
            this.lerpSpeed = lerpSpeed;

            this.demoExpositorMovement = new DemoExpositorMovement(transform);

            tweenCollection.Init();

            if(demoElements != null)
            {
                for(int i = 0; i < demoElements.Length; i++)
                {
                    demoElements[i].Init();
                }
            }

            initialized = true;
        }

        public void Show()
        {
        }

        public void Hide()
        {
        }

        private void Update()
        {
            if(!initialized)
            {
                return;
            }

            float deltaTime = Time.deltaTime;

            demoExpositorMovement.Update(deltaTime);

            tweenCollection.Update(deltaTime);
        }

        public void MoveToLeft()
        {
            this.demoExpositorMovement.StartMoving(transform.position, GetRightOutPosition(), lerpSpeed, true);
        }

        public void MoveToRight()
        {
            this.demoExpositorMovement.StartMoving(transform.position, GetLeftOutPosition(), lerpSpeed, true);
        }

        public void MoveToCenter()
        {
            this.demoExpositorMovement.StartMoving(transform.position, Vector3.zero, lerpSpeed, false);
        }

        public Vector3 GetCenterToCameraDir()
        {
            Vector3 res = (cam.transform.position - transform.position).normalized;
            return res;
        }

        public Vector3 GetRightBoundPosition()
        {
            Vector3 res = transform.position + cam.transform.right * bounds.x * 0.5f;
            return res;
        }

        public Vector3 GetLeftBoundPosition()
        {
            Vector3 res = transform.position - cam.transform.right * bounds.x * 0.5f;
            return res;
        }

        public Vector3 GetRightOutPosition()
        {
            Vector3 res = transform.position + cam.transform.right * bounds.x * 2f;
            return res;
        }

        public Vector3 GetLeftOutPosition()
        {
            Vector3 res = transform.position - cam.transform.right * bounds.x * 2f;
            return res;
        }

        public void PlaceNextTo(DemoExpositor previousDemoElement, DemoMovementDir dir)
        {
            if(dir == DemoMovementDir.RIGHT)
            {
                Vector3 previousLeftPos = previousDemoElement.GetRightBoundPosition();

                Vector3 newCenter = previousLeftPos + cam.transform.right * bounds.x * 2f;
                transform.position = newCenter;
            }
            else if(dir == DemoMovementDir.LEFT)
            {
                Vector3 previousRightPos = previousDemoElement.GetLeftBoundPosition();

                Vector3 newCenter = previousRightPos - cam.transform.right * bounds.x * 2f;
                transform.position = newCenter;
            }
        }
        
        [ContextMenu("Set Expositor Name to GameObject Name")]
        private void SetExpositorNameToGameObjectName()
        {
            expositorName = gameObject.name;
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if(!drawGizmos)
            {
                return;
            }

            if(Application.isPlaying && cam == null)
            {
                return;
            }


            Color boxCol = Color.cyan;
            boxCol.a = 0.1f;

            Matrix4x4 lastMatrix = Gizmos.matrix;
            Gizmos.color = boxCol;
            if(Application.isPlaying)
            {
                Gizmos.matrix = Matrix4x4.TRS(transform.position, cam.transform.rotation, Vector3.one);
                Gizmos.DrawCube(Vector3.zero, bounds);
            }
            else
            {
                Gizmos.DrawCube(transform.position, bounds);
            }

            Gizmos.matrix = lastMatrix;


            if(cam != null)
            {
                Vector3 rightBoundPosition = GetRightBoundPosition();
                Vector3 leftBoundPosition = GetLeftBoundPosition();
                Gizmos.color = Color.blue;
                Gizmos.DrawSphere(rightBoundPosition, 0.25f);
                Gizmos.DrawSphere(leftBoundPosition, 0.25f);
            }
        }
#endif
    }
}