using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// (c)2022 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
    public interface IWaveItemEventListener
    {
        void OnTriggeredEventHandler(WaveItemData waveItem);

        void OnStartSpawningEventHandler(WaveItemData waveItem);

        void OnSpawningCompleteEventHander(WaveItemData waveItem);

        void OnRepeatCompleteEventHandler(WaveItemData waveItem);

        void OnItemCompleteEventHandler(WaveItemData waveItem);
    } // interface IWaveItemEventListener
} // namespace Magique.SoulLink
