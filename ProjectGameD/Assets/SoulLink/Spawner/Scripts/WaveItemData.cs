using System.Collections.Generic;
using UnityEngine;
using System;
using static Magique.SoulLink.SoulLinkSpawnerTypes;
using static Magique.SoulLink.SpawnArea;

// (c)2022-2023 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
    [System.Serializable]
    public class WaveItemData: ISoulLinkGuid
    {
        public enum StartKind
        {
            Auto,                       // can start as soon as its delay is over
            Manual,                     // can only start by manual call to a start method
            WaveItemComplete            // can only start when a specific wave item has completed
        }

        public enum CompleteKind
        {
            AutoComplete,               // considered complete always; usually use for unlimited repeat because it will never complete
            SpawningComplete,           // spawns have all been generated
            EliminationKill,            // must be killed
            EliminationKillOrDespawn    // kill or despawn
        }

        public enum RepeatChangeKind
        {
            NoChange,
            Increase,
            Decrease
        }

        [System.Serializable]
        public class SpawnTracker
        {
            public int quota = 0;           // how many spawns will eventually be generated for this wave item
            public int totalSpawns = 0;     // the total amount spawned for this item on the current repeat
            public int spawnsKilled = 0;    // how many spawns were killed
            public int spawnsMissed = 0;    // how many spawns despawned but weren't killed
        } // class SpawnTracker

        public string itemName;
        public string guid; // unique identifier for this wave item
        public StartKind startType = StartKind.Auto;
        public float delayStartWave = 0f;
        public string waveItemDependency; // the name of the wave item that starts this wave item
        public SpawnInfoData spawnInfoData;
        public SpawningTriggerMethod spawningTriggerMethod = SpawningTriggerMethod.GlobalRadius;
        public SpawningMethod spawningMethod = SpawningMethod.SpawnRadius;
        public BaseSpline spline;
        public SplinePositioning splinePositioning = SplinePositioning.Start;
        public float clusterRange = 4f;
        public float overrideTriggerRadius = 5f;
        public Transform triggerOrigin; // the origin point for this item's trigger
        public float spawnRadius = 3f;
        public float minimumSpawnRange = 0f;
        public List<SpawnPoint> spawnPoints = new List<SpawnPoint>();
        public List<SpawnAreaBounds> spawnAreaBounds = new List<SpawnAreaBounds>();

        public Vector2 minMaxSpawns = new Vector2(1, 1);
        public bool randomizeSpawnInterval = false;
        public bool randomizeForEach = false;
        public float spawnInterval = 0f; // defaults to all at once
        public float spawnIntervalMax = 0f; // second value in a random range
        public Transform splinePath; // for referencing a third party spline path

        public bool repeatItem = false;
        public ResetRepeatKind repeatType = ResetRepeatKind.UserDefined;
        public int numberOfRepeats = 0;
        public float repeatDelay = 0f;
        public RepeatChangeKind repeatChangeType = RepeatChangeKind.NoChange;
        public Vector2 repeatMinMaxChange = Vector2.zero;

        public CompleteKind completeType = CompleteKind.SpawningComplete;

        public bool disable = false;

        public SpawnTracker spawnTracker;
        public bool started = false;
        public bool canStart = false;
        public bool completed = false;
        public bool paused = false;
        public int spawnPointIndex = 0;
        public float splineLength = 0f;
        public float splinePointSeparation = 0f;

        public Vector2 originalMinMaxSpawns = Vector2.zero;
        public bool hideItem = false;
        public bool spawnPointsFoldoutState = false;

        /// <summary>
        /// Generate a unique id for this object
        /// </summary>
        public void GenerateGuid()
        {
            if (string.IsNullOrEmpty(guid))
            {
                guid = Guid.NewGuid().ToString();
            } // if
        } // GenerateGuid()

        /// <summary>
        /// Get the object's Guid
        /// </summary>
        /// <returns></returns>
        public string GetGuid()
        {
            return guid;
        } //GetGuid()

        /// <summary>
        /// Set the object's Guid directly
        /// </summary>
        /// <param name="value"></param>
        public void SetGuid(string value)
        {
            guid = value;
        } // SetGuid()
    } // WaveItemData
} // namespace Magique.SoulLink
