using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

#pragma warning disable CS0649

namespace MinigamesDemo
{
    public class FortuneWheel : MonoBehaviour
    {
        [System.Serializable]
        public class WheelItem
        {
            [SerializeField]
            private string id;
            public string Id
            {
                get
                {
                    return id;
                }
            }

            [SerializeField]
            private Image image;
            public Image Image
            {
                get
                {
                    return image;
                }
            }

            [SerializeField]
            private Animator appearFX;
            public Animator AppearFX
            {
                get
                {
                    return appearFX;
                }
            }

            public UnityEvent OnChoosen;
        }

        [SerializeField]
        private UnityEvent onRotationComplete;

        [SerializeField]
        private Animator lanterns;

        [SerializeField]
        private Transform rotationRoot;

        [SerializeField]
        private List<WheelItem> items;

        [SerializeField]
        private float minAngle = 360.0f;

        [SerializeField]
        private float maxAngle = 1080.0f;

        [SerializeField]
        private float rotationSpeed = 50.0f;

        [SerializeField]
        private float appearDelay = 0.25f;

        [SerializeField]
        private float itemShowDelay = 0.1f;

        [SerializeField]
        private float itemScaleAmount = 1.1f;

        [SerializeField]
        private float itemScaleDuration = 0.25f;

        [SerializeField]
        private AnimationCurve spinCurve = AnimationCurve.Linear(0, 0, 1, 1);

        [SerializeField]
        private AudioSource rotationSource;

        [SerializeField]
        private float angleToProduceSound = 5.0f;

        private Coroutine spinCoroutine;

        public WheelItem GetWheelItem(int index)
        {
            return items[index];
        }

        public WheelItem GetWheelItem(string id)
        {
            return items.Find(x => x.Id == id);
        }

        private void OnEnable()
        {
            StopAllCoroutines();
            StartCoroutine(Appear());
        }

        [ContextMenu("Spin")]
        public void Spin()
        {
            if (spinCoroutine != null)
                StopCoroutine(spinCoroutine);

            spinCoroutine = StartCoroutine(SpinWheel());
        }

        private IEnumerator Appear()
        {
            lanterns.Play("Idle");

            foreach (var item in items)
            {
                item.Image.transform.localScale = Vector3.zero;
            }

            foreach (var item in items)
            {
                item.AppearFX.Play("Highlight appear");

                StartCoroutine(ShowItem(item));
                yield return new WaitForSeconds(appearDelay);
            }
        }

        private IEnumerator ShowItem(WheelItem item)
        {
            yield return new WaitForSeconds(itemShowDelay);

            item.Image.transform.localScale = Vector3.one;
        }

        private IEnumerator SpinWheel()
        {
            lanterns.Play("Idle");

            foreach (var itemToReset in items)
                itemToReset.Image.transform.localScale = Vector3.one;

            var targetAngle = Random.Range(minAngle, maxAngle);
            var initialRotation = rotationRoot.rotation;

            var currentValue = 0.0f;
            var angleToTheNextSoundLeft = angleToProduceSound;

            var previousRotation = 0.0f;
            while (currentValue <= 1.0f)
            {
                currentValue += Time.deltaTime * rotationSpeed / targetAngle;
                var currentRotation = spinCurve.Evaluate(currentValue) * targetAngle;
                angleToTheNextSoundLeft -= Mathf.Abs(currentRotation - previousRotation);
                if (angleToTheNextSoundLeft <= 0.0f)
                {
                    rotationSource.Play();
                    angleToTheNextSoundLeft = angleToProduceSound;
                }

                previousRotation = currentRotation;

                rotationRoot.rotation = Quaternion.AngleAxis(currentRotation, Vector3.forward) * initialRotation;

                yield return null;
            }

            var totalAngle = initialRotation.eulerAngles.z + targetAngle;
            totalAngle -= 360.0f * (int)(totalAngle / 360.0f);
            var step = 360.0f / items.Count;
            var itemIndex = (int)(totalAngle / step);

            onRotationComplete.Invoke();

            lanterns.Play("Win");

            var item = items[itemIndex];
            var currentTimePassed = 0.0f;
            while (currentTimePassed <= itemScaleDuration)
            {
                item.Image.transform.localScale = Vector3.Lerp(Vector3.one, Vector3.one * itemScaleAmount, currentTimePassed / itemScaleDuration);
                currentTimePassed += Time.deltaTime;

                yield return null;
            }

            item.OnChoosen.Invoke();

            spinCoroutine = null;
        }
    }
}