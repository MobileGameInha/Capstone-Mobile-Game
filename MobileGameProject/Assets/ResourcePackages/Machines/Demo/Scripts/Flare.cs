using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MinigamesDemo
{
    public class Flare : MonoBehaviour
    {
        [SerializeField]
        private float maxPositionRadiusVectorLength = 500.0f;

        [SerializeField]
        private float minMovementDuration = 3.0f;

        [SerializeField]
        private float maxMovementDuration = 6.0f;

        [SerializeField]
        private float minMovementSpeed = 50.0f;

        [SerializeField]
        private float maxMovementSpeed = 250.0f;

        [SerializeField]
        private float rotationSpeed = 30.0f;

        [SerializeField]
        private AnimationCurve alphaOverLifeTime = AnimationCurve.Linear(0, 1, 1, 1);

        private Image image;

        private float movementTimeLeft = 0.0f;
    
        private Vector2 movementDirection;

        private float movementSpeed;

        private RectTransform rectTransform;

        private float shift = 0.0f;

        private void Start()
        {
            shift = Random.Range(0, 1000);

            image = GetComponent<Image>();

            rectTransform = transform as RectTransform;
            RandomizePosition();
        }

        private void RandomizePosition()
        {
            rectTransform.anchoredPosition = Random.insideUnitCircle * Random.Range(0, maxPositionRadiusVectorLength);
        }

        private void Update()
        {
            if (movementTimeLeft <= 0.0f)
            {
                movementDirection = Random.insideUnitCircle.normalized;
                movementTimeLeft = Random.Range(minMovementDuration, maxMovementDuration);
                movementSpeed = Random.Range(minMovementSpeed, maxMovementSpeed);
            }

            rectTransform.anchoredPosition += movementDirection * movementSpeed * Time.deltaTime;
            if (rectTransform.anchoredPosition.magnitude >= maxPositionRadiusVectorLength && Vector3.Dot(movementDirection, rectTransform.anchoredPosition) > 0.0f)
            {
                rectTransform.anchoredPosition = Vector2.ClampMagnitude(rectTransform.anchoredPosition, maxPositionRadiusVectorLength);
                movementDirection = -movementDirection;
            }

            var color = image.color;
            color.a = alphaOverLifeTime.Evaluate(Time.time + shift);
            image.color = color;

            if (color.a <= 0.01f)
                RandomizePosition();

            movementTimeLeft -= Time.deltaTime;

            rectTransform.Rotate(new Vector3(0, 0, rotationSpeed * Time.deltaTime));
        }
    }
}