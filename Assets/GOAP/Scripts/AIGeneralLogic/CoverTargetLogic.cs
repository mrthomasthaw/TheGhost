using System.Collections.Generic;
using UnityEngine;

namespace MrThaw
{
    public class CoverTargetLogic : MonoBehaviour
    {
        public static Transform transform_static;

#if UNITY_EDITOR
        public static bool debugCoverCheck = true;
#endif

#if UNITY_EDITOR
        private static List<SphereTesterClass> gizmosTester = new List<SphereTesterClass>();
        private static LayerMask rayMask;

        private void OnDrawGizmos()
        {
            if (!debugCoverCheck)
                return;
            if (!Application.isPlaying)
                return;
            foreach (var groundHit in gizmosTester)
            {
                foreach (var sphere in groundHit.overlaps)
                {
                    Vector3 dir = Quaternion.Euler(0, sphere.angleAroundWallHit, 0) * groundHit.normalXDirection;
                    Vector3 centerOverlap = groundHit.groundHit.point + dir * sphere.sphereCenterDistFromWall + sphere.height * Vector3.up;

                    Gizmos.DrawSphere(centerOverlap, sphere.sphereCastRadius);

                    if (Physics.OverlapSphere(centerOverlap, sphere.sphereCastRadius, rayMask).Length > 0)
                    {
                        Gizmos.color = Color.green;
                        Color color = Color.green; color.a = .2f;
                        Gizmos.color = color;
                        Gizmos.DrawSphere(centerOverlap, sphere.sphereCastRadius);
                    }
                    else
                    {
                        Color color = Color.black; color.a = .2f;
                        Gizmos.color = color;
                        Gizmos.DrawSphere(centerOverlap, sphere.sphereCastRadius);
                    }
                }
            }
            gizmosTester = new List<SphereTesterClass>();
        }

#endif

        public static bool RequestCover(Ray ray, CoverPointCheckParams cp, float rayMaxDistance, ref Vector3 coverNormal, ref Vector3 groundHit, Vector3 minDistFromPosition)
        {
            List<SphereTesterClass> drawers = new List<SphereTesterClass>();
            SendRayCreateDrawers(ray, cp, rayMaxDistance, ref drawers);
            if (SendOverlaps(ref drawers, cp.rayMask) && ChooseCovers(ref drawers, ref coverNormal, ref groundHit, minDistFromPosition))
                return true;
            return false;
        }

        [System.Serializable]
        public class CoverCrouchCheckParams
        {
            public float characterHeight = 1.8f;
            public int crouchRayCheckCount = 10;
            public float crouchCheckStartHeight = 1.2f;
            public float rayMaxDistance = .3f;
        }

        public static float RequestCrouch(CoverCrouchCheckParams cp, LayerMask rayMask, Vector3 groundPoint, Vector3 wallNormal)
        {
            Vector3 startPos = groundPoint + Vector3.up * cp.crouchCheckStartHeight;
            int hitCount = 0;
            for (int i = 0; i < cp.crouchRayCheckCount; i++)
            {
                Ray ray = new Ray(
                    startPos + Vector3.up * i * (cp.characterHeight - cp.crouchCheckStartHeight) / cp.crouchRayCheckCount,
                    -wallNormal
                    );
                if (Physics.Raycast(ray, cp.rayMaxDistance, rayMask))
                    hitCount++;
            }
            return hitCount / ((float)cp.crouchRayCheckCount);
        }

        public static bool CheckCoverAroundPosition(CoverPointCheckParams cp, Vector3 position, ref Vector3 _coverWallNormalAround, ref Vector3 _coverGroundPositionAround, Vector3 minDistFromPosition, bool isCameraCover = false)
        {
            List<SphereTesterClass> drawers = new List<SphereTesterClass>();

            float step = 360 / ((float)cp.rayCheckCount360);
            Ray ray = new Ray();
            ray.origin = position + Vector3.up * cp.fromGroundPointStartUpHeight;
            for (int i = 0; i < cp.rayCheckCount360; i++)
            {
                ray.direction = Quaternion.Euler(0, step * i, 0) * Vector3.right;
                SendRayCreateDrawers(ray, cp, cp.rayMaxDistance, ref drawers);
            }
#if UNITY_EDITOR
            foreach (var item in drawers)
                gizmosTester.Add(item);
            rayMask = cp.rayMask;
#endif
            if (SendOverlaps(ref drawers, cp.rayMask) && ChooseCovers(ref drawers, ref _coverWallNormalAround, ref _coverGroundPositionAround, minDistFromPosition, isCameraCover))
            {
                return true;
            }
            else
                return false;
        }

        private static void SendRayCreateDrawers(Ray ray, CoverPointCheckParams cp, float rayMaxDistance, ref List<SphereTesterClass> drawers)
        {
            if (drawers == null)
                drawers = new List<SphereTesterClass>();

            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, rayMaxDistance, cp.rayMask))
            {
                if (!transform_static)
                {
                    GameObject newGo = new GameObject();
                    transform_static = newGo.transform;
                }
                transform_static.position = hit.point;
                transform_static.forward = hit.normal;
                Vector3 dirNormalrotXIgnored = Quaternion.AngleAxis(transform_static.eulerAngles.x * -1, transform_static.TransformDirection(Vector3.right)) * hit.normal;

                Ray rayBFromHit = new Ray(hit.point + dirNormalrotXIgnored * cp.afterHitBackDist, Vector3.down);
#if UNITY_EDITOR
                if (debugCoverCheck)
                    Debug.DrawRay(rayBFromHit.origin, rayBFromHit.direction, Color.black);
#endif
                RaycastHit toGroundHit;

                if (Physics.Raycast(rayBFromHit, out toGroundHit, 1.8f, cp.rayGroundCheckMask) && (Vector3.Angle(Vector3.up, toGroundHit.normal) < cp.maxAllowedGroundNormalAngle))
                {
                    drawers.Add(new SphereTesterClass());
                    drawers[drawers.Count - 1].groundHit = toGroundHit;
                    drawers[drawers.Count - 1].normalXDirection = dirNormalrotXIgnored;

                    foreach (var x in cp.overlapSpheres)
                    {
                        drawers[drawers.Count - 1].overlaps.Add(x);
                    }
                }
            }
        }

        // Eliminates wrong drawers
        public static bool SendOverlaps
        (ref List<SphereTesterClass> drawers, LayerMask rayMask)
        {
            for (int i = 0; i < drawers.Count; i++)
            {
                int check = 0;
                var groundHit = drawers[i];
                foreach (var sphere in groundHit.overlaps)
                {
                    Vector3 dir = Quaternion.Euler(0, sphere.angleAroundWallHit, 0) * groundHit.normalXDirection;
                    Vector3 centerOverlap = groundHit.groundHit.point + dir * sphere.sphereCenterDistFromWall + sphere.height * Vector3.up;

                    Collider[] colz = Physics.OverlapSphere(centerOverlap, sphere.sphereCastRadius, rayMask);
                    if ((sphere.shouldHit && colz.Length > 0) || (!sphere.shouldHit && colz.Length == 0))
                        check++;
                }
                if (check != groundHit.overlaps.Count)
                {
                    drawers.RemoveAt(i);
                    i--;
                }
            }
            if (drawers.Count == 0)
                return false;
            return true;
        }

        private static bool ChooseCovers(
        ref List<SphereTesterClass> drawers, ref Vector3 coverWallNormal, ref Vector3 coverGroundPosition, Vector3 minDistFromPosition, bool isCameraCover = false, float minAllowedCameraCoverDist = 1.5f
        )
        {
            if (drawers == null || drawers.Count == 0)
                return false;

            float minDistC = Mathf.Infinity;
            foreach (var item in drawers)
            {
                float dist = Vector3.Distance(minDistFromPosition, item.groundHit.point);
                if (isCameraCover && dist < minAllowedCameraCoverDist)
                    continue;
                if (dist < minDistC)
                {
                    minDistC = dist;
                    coverWallNormal = item.normalXDirection;
                    coverGroundPosition = item.groundHit.point;
                }
            }

            return true;
        }

        public class SphereTesterClass
        {
            public List<OverlapSphereClass> overlaps;
            public RaycastHit groundHit;
            public Vector3 normalXDirection;

            public SphereTesterClass()
            {
                overlaps = new List<OverlapSphereClass>();
            }

            public SphereTesterClass(SphereTesterClass sp)
            {
                overlaps = sp.overlaps;
            }
        }

        [System.Serializable]
        public class OverlapSphereClass
        {
            public float sphereCenterDistFromWall = .2f;
            public float angleAroundWallHit = 45f;
            public float sphereCastRadius = .1f;
            public float height = .5f;
            public bool shouldHit = true;
        }

        [System.Serializable]
        public class CoverPointCheckParams
        {
            public LayerMask rayMask;
            public LayerMask rayGroundCheckMask;
            public float fromGroundPointStartUpHeight = .75f;
            public int rayCheckCount360 = 40;
            public float afterHitBackDist = .04f;
            public float maxAllowedGroundNormalAngle = 25f;

            public float rayMaxDistance = 1f;

            public List<OverlapSphereClass> overlapSpheres;
        }
    }
}