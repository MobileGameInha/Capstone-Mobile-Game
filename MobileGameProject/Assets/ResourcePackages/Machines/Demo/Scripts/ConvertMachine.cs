using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable CS0649

namespace MinigamesDemo
{
    public class ConvertMachine : MonoBehaviour
    {
        [SerializeField]
        private Animator rewardAnimator;

        [SerializeField]
        private CanvasGroup background;

        [SerializeField]
        private float backgroundFadeDuration = 0.5f;

        [SerializeField]
        private float prizeDisplayDuration = 1.5f;

        private void Start()
        {
            rewardAnimator.Play("Hidden");
        }

        private void OnEnable()
        {
            background.blocksRaycasts = false;
            background.alpha = 0.0f;
        }

        public void GiveReward()
        {
            StopAllCoroutines();
            StartCoroutine(ShowPrize());
        }

        public void Hide()
        {
            rewardAnimator.Play("Hidden");
            StopAllCoroutines();
            StartCoroutine(ShowPrize());
        }

        private IEnumerator ShowPrize()
        {
            rewardAnimator.Play("Appear");
            yield return StartCoroutine(SetBackState(true));
        
            yield return new WaitForSeconds(prizeDisplayDuration);

            yield return StartCoroutine(SetBackState(false));
        }

        private IEnumerator SetBackState(bool state)
        {
            background.blocksRaycasts = state;
            var ratio = 0.0f;
            background.blocksRaycasts = state;
            while (ratio < 1.0f)
            {
                ratio += Time.deltaTime / backgroundFadeDuration;
                background.alpha = Mathf.Lerp(state ? 0.0f : 1.0f, state ? 1.0f : 0.0f, ratio);

                yield return null;
            }
        }
    }
}