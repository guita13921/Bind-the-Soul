using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// (c)2022-2023 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
    public class WaveItemCompleteListener : MonoBehaviour
    {
        [SerializeField]
        private List<string> _waveItemsToComplete = new List<string>();

        [SerializeField]
        private string _waveItemToTrigger;

        [SerializeField]
        private bool _ignoreWaveItemTrigger = false;

        private Dictionary<string, bool> _waveItemsStatus = new Dictionary<string, bool>();
        private bool _allComplete = false;

        private WaveSpawner _waveSpawner;

        private void Awake()
        {
            _waveSpawner = GetComponent<WaveSpawner>();
            _waveSpawner.OnWaveCompletedEventHandler.AddListener(OnWaveComplete);

            foreach (var item in _waveItemsToComplete)
            {
                _waveItemsStatus[item] = false;
            } // foreach
        } // Awake()

        void OnItemCompleteEventHandler(WaveItemData waveItem)
        {
            if (_allComplete) return;

            if (_waveItemsStatus.ContainsKey(waveItem.itemName))
            {
                _waveItemsStatus[waveItem.itemName] = true;
            } // if

            _allComplete = !_waveItemsStatus.Any(e => e.Value == false);
            if (_allComplete)
            {
                if (_ignoreWaveItemTrigger)
                {
                    GetComponent<WaveSpawner>().PerformWaveTrigger(_waveItemToTrigger);
                }
                else
                {
                    GetComponent<WaveSpawner>().AllowWaveToStart(_waveItemToTrigger);
                }
            } // if
        } // OnItemCompleteEventHandler()

        private void OnWaveComplete()
        {
            ResetToDefault();
        } // OnWaveComplete()

        public void ResetToDefault()
        {
            foreach (var item in _waveItemsToComplete)
            {
                _waveItemsStatus[item] = false;
            } // foreach

            _allComplete = false;
        } // ResetToDefault()
    } // class WaveItemCompleteListener
} // namespace Magique.SoulLink
