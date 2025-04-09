using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#pragma warning disable CS0649

namespace MinigamesDemo
{
    public class ClawCrane : MonoBehaviour
    {
        [System.Serializable]
        public class Option
        {
            public Transform Visualization;
        }

        [SerializeField]
        private List<Option> options;

        [SerializeField]
        private Transform claw;

        [SerializeField]
        private Transform clawExtention;

        [SerializeField]
        private Transform firstPosition;

        [SerializeField]
        private Transform secondPosition;

        [SerializeField]
        private Transform rewardHandPosition;

        [SerializeField]
        private Transform rewardTargetPosition;

        [SerializeField]
        private Transform leftClaw;

        [SerializeField]
        private Transform rightClaw;

        [SerializeField]
        private Transform optionsRoot;

        [SerializeField]
        private float clawGrabAngle = 30.0f;

        [SerializeField]
        private Lever lever;

        [SerializeField]
        private float moveSpeed = 10.0f;

        [SerializeField]
        private float speedOverDepth = 10.0f;

        [SerializeField]
        private float rotationScaler = 3.0f;

        [SerializeField]
        private float grabDuration = 1.0f;

        [SerializeField]
        private float grabStopDuration = 0.5f;

        [SerializeField]
        private float grabShift = 50.0f;

        [SerializeField]
        private float returnSpeed = 0.6f;

        [SerializeField]
        private float fallSpeed = 10.0f;

        [SerializeField]
        private AnimationCurve scaleOverZ;

        [SerializeField]
        private ClawCraneButton moveDownButton;

        [SerializeField]
        private Image rewardShadow;

        [SerializeField]
        private AnimationCurve shadowTransparencyOverRatio;

        [SerializeField]
        private float clawRotationDuration = 0.1f;

        [SerializeField]
        private RewardWindow rewardWindow;

        [SerializeField]
        private CanvasGroup moveTutorialGroup;

        [SerializeField]
        private CanvasGroup pressTutorialGroup;

        [SerializeField]
        private AudioSource clawMoveSound;

        [SerializeField]
        private AudioSource rewardFallSound;

        private float position = 0.0f;

        private float positionZ = 0.0f;

        private Coroutine grabCoroutine;

        private bool pressTutorialHasBeenShown = false;

        private bool movingBack = false;

        private void Start()
        {
            moveDownButton.onClick
                .AddListener(() =>
                    {
                        if (grabCoroutine != null)
                            return;

                        grabCoroutine = StartCoroutine(Grab());
                        pressTutorialGroup.alpha = 0.0f;
                    });

            moveTutorialGroup.alpha = 1.0f;
            moveTutorialGroup.blocksRaycasts = false;

            pressTutorialGroup.alpha = 0.0f;
            pressTutorialGroup.blocksRaycasts = false;

            HideShadow();
        }

        private void OnDisable()
        {
            grabCoroutine = null;
            position = 0.0f;
            positionZ = 0.0f;

            ResetOptions();

            StopAllCoroutines();
        }

        private void Update()
        {
            var input = grabCoroutine == null ? lever.Input : Vector2.zero;
            var xShift = input.x * moveSpeed;

            clawMoveSound.volume = movingBack || input.magnitude > 0.0f ? 0.5f : 0.0f;

            position += xShift * Time.deltaTime;
            position = Mathf.Clamp01(position);

            var zShift = input.y * speedOverDepth;

            if (input.magnitude > 0.0f)
            {
                moveTutorialGroup.alpha = 0.0f;
                if (!pressTutorialHasBeenShown)
                {
                    pressTutorialGroup.alpha = 1.0f;
                    pressTutorialHasBeenShown = true;
                }
            }

            positionZ += -zShift * Time.deltaTime;
            positionZ = Mathf.Clamp01(positionZ);

            claw.transform.localScale = Vector3.one * scaleOverZ.Evaluate(positionZ);

            claw.transform.localRotation = Quaternion.Euler(0, 0, xShift * rotationScaler);

            claw.transform.position = Vector3.Lerp(firstPosition.position, secondPosition.position, (position - 0.5f) * scaleOverZ.Evaluate(positionZ) + 0.5f * scaleOverZ.Evaluate(positionZ));
        }

        private IEnumerator RotateClaws(float angle)
        {
            var ratio = 0.0f;
            var initialLeftRotation = leftClaw.localRotation;
            var initialRightRotation = rightClaw.localRotation;

            var targetLeftRotation = Quaternion.AngleAxis(angle, Vector3.forward) * leftClaw.localRotation;
            var targetRightRotation = Quaternion.AngleAxis(-angle, Vector3.forward) * rightClaw.localRotation;

            while (ratio < 1.0f)
            {
                ratio += Time.deltaTime / clawRotationDuration;

                leftClaw.localRotation = Quaternion.Slerp(initialLeftRotation, targetLeftRotation, ratio);
                rightClaw.localRotation = Quaternion.Slerp(initialRightRotation, targetRightRotation, ratio);

                yield return null;
            }
        }

        private void HideShadow()
        {
            var shadowColor = rewardShadow.color;
            shadowColor.a = 0.0f;
            rewardShadow.color = shadowColor;
        }

        private void ResetOptions()
        {
            foreach (var optionToReset in options)
            {
                optionToReset.Visualization.SetParent(optionsRoot);
                optionToReset.Visualization.localPosition = Vector3.zero;
            }
        }

        private IEnumerator Grab()
        {
            HideShadow();
            ResetOptions();

            var ratio = 0.0f;
            var initialPosition = clawExtention.localPosition;
            var targetPosition = clawExtention.localPosition + Vector3.down * grabShift;
            while (ratio < 1.0f)
            {
                ratio += Time.deltaTime / (grabDuration / 2.0f);
                clawExtention.localPosition = Vector3.Lerp(initialPosition, targetPosition, ratio);

                yield return null;
            }

            yield return new WaitForSeconds(grabStopDuration);

            var option = options[Random.Range(0, options.Count)];
            option.Visualization.transform.SetParent(rewardHandPosition);
            option.Visualization.localScale = Vector3.one;
            option.Visualization.localPosition = Vector3.zero;

            yield return StartCoroutine(RotateClaws(clawGrabAngle));

            movingBack = true;

            while (ratio > 0.0)
            {
                ratio -= Time.deltaTime / (grabDuration / 2.0f);
                clawExtention.localPosition = Vector3.Lerp(initialPosition, targetPosition, ratio);

                yield return null;
            }

            while (position > 0.0f || positionZ > 0.0f)
            {
                position = Mathf.MoveTowards(position, 0.0f, Time.deltaTime * returnSpeed);
                positionZ = Mathf.MoveTowards(positionZ, 0.0f, Time.deltaTime * returnSpeed);

                yield return null;
            }

            yield return StartCoroutine(RotateClaws(-clawGrabAngle));

            var initialRewardPosition = option.Visualization.position;

            option.Visualization.SetParent(rewardTargetPosition);

            var rewardRatio = 0.0f;
            while (rewardRatio < 1.0f)
            {
                rewardRatio += Time.deltaTime * fallSpeed;

                option.Visualization.position = Vector3.Lerp(initialRewardPosition, rewardTargetPosition.position, rewardRatio);

                var shadowAlpha = shadowTransparencyOverRatio.Evaluate(rewardRatio);

                var shadowColor = rewardShadow.color;
                shadowColor.a = shadowAlpha;
                rewardShadow.color = shadowColor;

                yield return null;
            }

            rewardFallSound.Play();

            movingBack = false;

            rewardWindow.ShowReward(option.Visualization.gameObject, () =>
                {
                    HideShadow();
                    ResetOptions();
                });

            grabCoroutine = null;
        }
    }
}