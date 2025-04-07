using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

#pragma warning disable CS0649

namespace MinigamesDemo
{
    public class SlotMachine : MonoBehaviour
    {
        [System.Serializable]
        public class Option
        {
            public Transform Visuals;
        }

        [SerializeField]
        private UnityEvent onRewardDroppedOff;

        [SerializeField]
        private List<Option> options;

        [SerializeField]
        private int shufflesCount = 5;

        [SerializeField]
        private float shufflesInterval = 0.2f;

        [SerializeField]
        private float minShuffleSpeed;

        [SerializeField]
        private float maxShuffleSpeed;

        [SerializeField]
        private AnimationCurve powerOverIteration;

        [SerializeField]
        private AnimationCurve dropAnimationCurve;

        [SerializeField]
        private float dropSpeed = 1.0f;

        [SerializeField]
        private Transform startDropPosition;

        [SerializeField]
        private Transform endDropPosition;

        [SerializeField]
        private Image shadow;

        [SerializeField]
        private AnimationCurve shadowAlphaOverDrop;

        [SerializeField]
        private RewardWindow rewardWindow;

        [SerializeField]
        private AudioSource shuffleSource;

        [SerializeField]
        private AudioClip hitSound;

        [SerializeField]
        private float minShuffleInterval = 0.15f;

        [SerializeField]
        private float maxShuffleInterval = 0.25f;

        [SerializeField]
        private float minShufflePitch = 0.8f;

        [SerializeField]
        private float maxShufflePitch = 1.1f;

        private List<Rigidbody2D> balls = new List<Rigidbody2D>();

        private Dictionary<Rigidbody2D, float> gravityByBody = new Dictionary<Rigidbody2D, float>();

        private Coroutine rewardCoroutine;

        private void Start()
        {
            GetComponentsInChildren(balls);

            foreach (var option in options)
                option.Visuals.gameObject.SetActive(false);

            HideShadow();
        }

        private void HideShadow()
        {
            var shadowColor = shadow.color;
            shadowColor.a = 0.0f;
            shadow.color = shadowColor;
        }

        public void Engage()
        {
            if (rewardCoroutine != null)
                return;

            rewardCoroutine = StartCoroutine(GetReward());
        }

        private IEnumerator PlayShuffleSounds()
        {
            while (true)
            {
                shuffleSource.pitch = Random.Range(minShufflePitch, maxShufflePitch);
                shuffleSource.PlayOneShot(hitSound);

                yield return new WaitForSeconds(Random.Range(minShuffleInterval, maxShuffleInterval));
            }
        }

        private IEnumerator GetReward()
        {
            HideShadow();

            foreach (var option in options)
                option.Visuals.gameObject.SetActive(false);

            gravityByBody.Clear();
            foreach (var body in balls)
            {
                gravityByBody.Add(body, body.gravityScale);
                body.gravityScale = 0.0f;
            }

            var shuffleSoundCoroutine = StartCoroutine(PlayShuffleSounds());

            for (var index = 0; index < shufflesCount; index++)
            {
                foreach (var body in balls)
                    if (body.bodyType != RigidbodyType2D.Static)
                        body.velocity += Random.insideUnitCircle.normalized * Random.Range(minShuffleSpeed, maxShuffleSpeed) * powerOverIteration.Evaluate((float)index / shufflesCount);

                yield return new WaitForSeconds(shufflesInterval);
            }

            StopCoroutine(shuffleSoundCoroutine);

            var selectedOption = options[Random.Range(0, options.Count)];

            selectedOption.Visuals.gameObject.SetActive(true);
            var initialOptionPosition = selectedOption.Visuals.localPosition;

            foreach (var body in balls)
                body.gravityScale = gravityByBody[body];

            var dropRatio = 0.0f;
            while (dropRatio < 1.0f)
            {
                dropRatio += Time.deltaTime * dropSpeed;

                var dropPosition = dropAnimationCurve.Evaluate(dropRatio);

                selectedOption.Visuals.position = Vector3.Lerp(startDropPosition.position, endDropPosition.position, dropPosition);

                var shadowColor = shadow.color;
                shadowColor.a = shadowAlphaOverDrop.Evaluate(dropPosition);
                shadow.color = shadowColor;

                yield return null;
            }

            HideShadow();

            onRewardDroppedOff.Invoke();

            var complete = false;
            rewardWindow.ShowReward(selectedOption.Visuals.gameObject, () =>
                {
                    complete = true;
                });

            while (!complete)
                yield return null;

            var savedScale = selectedOption.Visuals.localScale;
            selectedOption.Visuals.localScale = Vector3.zero;

            yield return null;

            selectedOption.Visuals.localScale = savedScale;

            selectedOption.Visuals.gameObject.SetActive(false);
            selectedOption.Visuals.localPosition = initialOptionPosition;

            rewardCoroutine = null;
        }
    }
}