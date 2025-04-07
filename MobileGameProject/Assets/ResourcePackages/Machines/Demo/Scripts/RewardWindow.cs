using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

#pragma warning disable CS0649

namespace MinigamesDemo
{
    public class RewardWindow : MonoBehaviour
    {
        [SerializeField]
        private UnityEvent onShown;

        [SerializeField]
        private UnityEvent onRewardAnimation;

        [SerializeField]
        private Transform rewardTargetPosition;

        [SerializeField]
        private float moveDuration = 0.5f;

        [SerializeField]
        private float animationDuration = 0.25f;

        [SerializeField]
        private CanvasGroup group;

        [SerializeField]
        private CanvasGroup shineGroup;

        [SerializeField]
        private float shineGroupAppearDuration = 0.5f;

        [SerializeField]
        private GameObject flares;

        private GameObject rewardObject;

        private Coroutine rewardCoroutine;

        private Vector3 initialRewardPosition;
        private Quaternion initialRewardRotation;
        private Vector3 initialRewardScale;
        private Transform initialRewardObjectParent;
        private Action onComplete;

        public void ShowReward(GameObject rewardObject, Action onComplete)
        {
            if (rewardCoroutine != null)
                return;

            onShown.Invoke();
            this.rewardObject = rewardObject;
            group.alpha = 1.0f;
            group.blocksRaycasts = true;

            this.onComplete = onComplete;
            rewardCoroutine = StartCoroutine(OnShow());
        }

        private IEnumerator ChangeVisibility(CanvasGroup group, float duration, bool state)
        {
            var ratio = 0.0f;
            group.blocksRaycasts = state;
            while (ratio < 1.0f && duration > 0.0f)
            {
                ratio += Time.deltaTime / duration;
                group.alpha = Mathf.Lerp(state ? 0 : 1, state ? 1 : 0, ratio);

                yield return null;
            }

            group.alpha = state ? 1 : 0;
        }

        public void Hide()
        {
            if (rewardCoroutine != null)
            {
                StopCoroutine(rewardCoroutine);
                rewardCoroutine = null;
            }

            group.alpha = 0.0f;
            group.blocksRaycasts = false;

            StartCoroutine(ChangeVisibility(shineGroup, 0.0f, false));

            rewardObject.transform.SetParent(initialRewardObjectParent, true);
            rewardObject.transform.position = initialRewardPosition;
            rewardObject.transform.rotation = initialRewardRotation;
            rewardObject.transform.localScale = initialRewardScale;

            var animator = rewardObject.GetComponent<Animator>();
            if (animator != null)
                animator.Play("Idle");

            if (onComplete != null)
                onComplete.Invoke();
        }

        private IEnumerator OnShow()
        {
            flares.SetActive(false);

            initialRewardObjectParent = rewardObject.transform.parent;
            initialRewardScale = rewardObject.transform.localScale;
            rewardObject.transform.SetParent(transform);
            rewardObject.transform.SetSiblingIndex(rewardTargetPosition.GetSiblingIndex() + 1);

            initialRewardPosition = rewardObject.transform.position;
            initialRewardRotation = rewardObject.transform.rotation;

            var ratio = 0.0f;
            while (ratio < 1.0f)
            {
                ratio += Time.deltaTime / moveDuration;

                rewardObject.transform.position = Vector3.Lerp(initialRewardPosition, rewardTargetPosition.position, ratio);
                rewardObject.transform.rotation = Quaternion.Slerp(initialRewardRotation, rewardTargetPosition.rotation, ratio);
                rewardObject.transform.localScale = Vector3.Lerp(initialRewardScale, rewardTargetPosition.localScale, ratio);

                yield return null;
            }

            onRewardAnimation.Invoke();
            var animator = rewardObject.GetComponent<Animator>();
            if (animator != null)
                animator.Play("Get reward");

            yield return new WaitForSeconds(animationDuration);

            flares.SetActive(true);

            yield return StartCoroutine(ChangeVisibility(shineGroup, shineGroupAppearDuration, true));

            rewardCoroutine = null;
        }
    }
}