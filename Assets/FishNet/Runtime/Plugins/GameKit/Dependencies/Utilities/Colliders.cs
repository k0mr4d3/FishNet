using System;
using UnityEngine;

namespace GameKit.Dependencies.Utilities
{

    public static class ColliderExtensions
    {
        public static void GetBoxCastParams(this BoxCollider boxCollider, out Vector3 center, out Vector3 halfExtents)
        {
            Transform cachedTransform = boxCollider.transform;

            // DO NOT USE UNITY'S VECTOR OPERATIONS IN HOT PATHS, UNITY DOESN'T OPTIMISE THEM

            center = cachedTransform.TransformPoint(boxCollider.center);

            Vector3 lossyScale = cachedTransform.lossyScale;

            Vector3 size = boxCollider.size;

            float x = size.x * 0.5f * lossyScale.x;
            float y = size.y * 0.5f * lossyScale.y;
            float z = size.z * 0.5f * lossyScale.z;

            halfExtents = new Vector3(x, y, z);
        }

        public static void GetCapsuleCastParams(this CapsuleCollider capsuleCollider, out Vector3 point1, out Vector3 point2, out float radius)
        {
            Transform cachedTransform = capsuleCollider.transform;

            Vector3 lossyScale = cachedTransform.lossyScale;

            // Use System.Math instead of UnityEngine.Mathf because it's much faster.

            float absX = Math.Abs(lossyScale.x);
            float absY = Math.Abs(lossyScale.y);
            float absZ = Math.Abs(lossyScale.z);

            float height;

            Vector3 direction;

            switch (capsuleCollider.direction)
            {
                case 1:
                    {
                        radius = capsuleCollider.radius * Math.Max(absX, absZ);

                        height = capsuleCollider.height * absY;

                        direction = Vector3.up;

                        break;
                    }

                case 2:
                    {
                        radius = capsuleCollider.radius * Math.Max(absX, absY);

                        height = capsuleCollider.height * absZ;

                        direction = Vector3.forward;

                        break;
                    }

                default:
                    {
                        // Falling back to X is Unity's default behaviour.

                        radius = capsuleCollider.radius * Math.Max(absY, absZ);

                        height = capsuleCollider.height * absX;

                        direction = Vector3.right;

                        break;
                    }
            }

            Vector3 center = cachedTransform.TransformPoint(capsuleCollider.center);

            Vector3 offset = height < radius * 2.0f ? Vector3.zero : cachedTransform.TransformDirection(direction * (height * 0.5f - radius));

            // DO NOT USE UNITY'S  VECTOR OPERATIONS IN HOT PATHS, UNITY DOESN'T OPTIMISE THEM

            float x1 = center.x + offset.x;
            float y1 = center.y + offset.y;
            float z1 = center.z + offset.z;

            float x2 = center.x - offset.x;
            float y2 = center.y - offset.y;
            float z2 = center.z - offset.z;

            point1 = new Vector3(x1, y1, z1);

            point2 = new Vector3(x2, y2, z2);
        }

        public static void GetSphereCastParams(this SphereCollider sphereCollider, out Vector3 center, out float radius)
        {
            Transform cachedTransform = sphereCollider.transform;

            center = cachedTransform.TransformPoint(sphereCollider.center);

            Vector3 lossyScale = cachedTransform.lossyScale;

            // Use System.Math instead of UnityEngine.Mathf because it's much faster.

            float x = Math.Abs(lossyScale.x);
            float y = Math.Abs(lossyScale.y);
            float z = Math.Abs(lossyScale.z);

            // Two calls of Math.Max are faster than a single Mathf.Max call because Math.Max doesn't allocate memory and doesn't use loops.

            radius = sphereCollider.radius * Math.Max(Math.Max(x, y), z);
        }
    }

}