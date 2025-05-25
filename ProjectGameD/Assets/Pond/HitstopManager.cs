using UnityEngine;
using System.Collections;

namespace SG
{

    public class HitstopManager : MonoBehaviour
    {
        public static HitstopManager Instance;

        [Range(0f, 1f)]
        public float timeScaleDuringHitstop = 0f;
        public float defaultTimeScale = 1f;

        private void Awake()
        {
            if (Instance != null && Instance != this)
                Destroy(this);
            else
                Instance = this;

            Time.timeScale = defaultTimeScale;
        }

        public void TriggerHitstop(float duration)
        {
            StartCoroutine(HitstopCoroutine(duration));
        }

        private IEnumerator HitstopCoroutine(float duration)
        {
            Time.timeScale = timeScaleDuringHitstop;
            float pause = duration * defaultTimeScale;
            yield return new WaitForSecondsRealtime(pause);
            Time.timeScale = defaultTimeScale;
            Debug.Log("HitstopCoroutine");
        }
    }

}