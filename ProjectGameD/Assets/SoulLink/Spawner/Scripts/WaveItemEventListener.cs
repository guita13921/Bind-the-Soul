using UnityEngine;

// WaveItemEventListener
// An example of how to implement the IWaveItemEventListener interface

// (c)2022 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
    public class WaveItemEventListener : MonoBehaviour, IWaveItemEventListener
    {
        public void OnTriggeredEventHandler(WaveItemData waveItem)
        {
            Debug.Log("=====OnTriggeredEventHandler for " + waveItem.itemName);
        }

        public void OnStartSpawningEventHandler(WaveItemData waveItem)
        {
            Debug.Log("=====OnStartSpawningEventHandler for " + waveItem.itemName);
        }

        public void OnSpawningCompleteEventHander(WaveItemData waveItem)
        {
            Debug.Log("=====OnSpawningCompleteEventHandler for " + waveItem.itemName);
        }

        public void OnRepeatCompleteEventHandler(WaveItemData waveItem)
        {
            Debug.Log("=====OnRepeatCompleteEventHandler for " + waveItem.itemName);
        }

        public void OnItemCompleteEventHandler(WaveItemData waveItem)
        {
            Debug.Log("=====OnItemCompleteEventHandler for " + waveItem.itemName);
        }
    } // class WaveItemEventListener
} // namespace Magique.SoulLink
