using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#pragma warning disable CS0649

namespace MinigamesDemo
{
    public class Smoke : MonoBehaviour
    {
        [SerializeField]
        private int smokeParticlesCount = 5;

        [SerializeField]
        private GameObject smokePrototype;

        [SerializeField]
        private float minHeight;

        [SerializeField]
        private float maxHeight;

        [SerializeField]
        private float minSize;

        [SerializeField]
        private float maxSize;

        [SerializeField]
        private float minSpeed;

        [SerializeField]
        private float maxSpeed;

        [SerializeField]
        private float spawnShift;

        [SerializeField]
        private Gradient colorOverRatio;

        [SerializeField]
        private AnimationCurve sizeOverRatio;

        private List<GameObject> createdSmokeParticles = new List<GameObject>();

        private void OnEnable()
        {
            foreach (var particle in createdSmokeParticles)
                Destroy(particle);

            createdSmokeParticles.Clear();

            smokePrototype.SetActive(false);
            for (var index = 0; index < smokeParticlesCount; index++)
            {
                var instance = Instantiate(smokePrototype, smokePrototype.transform.parent);
                var rectTransform = (instance.transform as RectTransform);
                instance.SetActive(true);
                createdSmokeParticles.Add(instance);
                StartCoroutine(UpdateSmoke(rectTransform));
            }
        }

        private IEnumerator UpdateSmoke(RectTransform smoke)
        {
            var initialPosition = smoke.anchoredPosition;
            var smokeImage = smoke.GetComponent<Image>();
            while (true)
            {
                var firstPosition = initialPosition + initialPosition + Random.insideUnitCircle * Random.Range(0.0f, spawnShift);
                smoke.SetAsFirstSibling();
                var size = Random.Range(minSize, maxSize);

                var height = Random.Range(minHeight, maxHeight);
                var targetPosition = initialPosition + Vector2.up * height;

                var speed = Random.Range(minSpeed, maxSpeed);
            
                var ratio = 0.0f;
                while (ratio < 1.0f)
                {
                    ratio += Time.deltaTime * speed;

                    var sizeScaler = sizeOverRatio.Evaluate(ratio);

                    smoke.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size * sizeScaler);
                    smoke.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size * sizeScaler);
                    smoke.anchoredPosition = Vector3.Lerp(firstPosition, targetPosition, ratio);
                    smokeImage.color = colorOverRatio.Evaluate(ratio);

                    yield return null;
                }
            }
        }
    }
}