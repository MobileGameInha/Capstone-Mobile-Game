using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MinigamesDemo
{
    public class Lever : MonoBehaviour, IDragHandler, IEndDragHandler
    {
        [SerializeField]
        private float rotationScaler = 0.2f;

        [SerializeField]
        private float maxAngle = 20.0f;

        public Vector2 Input
        {
            get
            {
                var input = Vector3.Cross(Vector3.up, transform.up);

                return new Vector2(input.z, input.x) / Mathf.Sin(maxAngle * Mathf.Deg2Rad);
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            transform.rotation = 
                Quaternion.AngleAxis(-eventData.delta.x * rotationScaler, Vector3.forward) *
                Quaternion.AngleAxis(-eventData.delta.y * rotationScaler, Vector3.right) * 
                transform.rotation;

            var currentAngle = Vector3.Angle(Vector3.up, transform.up);
            if (currentAngle > maxAngle)
                transform.rotation = Quaternion.Slerp(Quaternion.identity, transform.rotation, 1.0f - ((currentAngle - maxAngle) / maxAngle));
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            transform.rotation = Quaternion.identity;
        }
    }
}