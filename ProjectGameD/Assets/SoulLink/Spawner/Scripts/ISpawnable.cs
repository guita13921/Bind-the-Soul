using System.Collections.Generic;
using UnityEngine;
using static Magique.SoulLink.SoulLinkSpawnerTypes;

// (c)2021-2023 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
    public interface ISpawnable
    {
        GameObject GetGameObject();

        void SetSpawnAction(BaseSpawnAction spawnAction);

        string GetTimeOfDaySpawned();

        void SetTimeOfDaySpawned(string timeOfDayName);

        List<WeatherRuleData> GetWeatherRules();

        void SetWeatherRules(List<WeatherRuleData> weatherRules);

        List<SeasonRuleData> GetSeasonRules();

        void SetSeasonRules(List<SeasonRuleData> seasonRules);

        List<QuestRuleData> GetQuestRules();

        void SetQuestRules(List<QuestRuleData> questRules);

        Transform GetSpawnPrefab();

        void SetSpawnPrefab(Transform prefab);

        bool GetRequiresNavMesh();

        void SetRequiresNavMesh(bool value);

        bool GetIsPersistent();

        void SetIsPersistent(bool value);

        bool GetIgnoreTotalSpawns();

        void SetIgnoreTotalSpawns(bool value);

        float GetDespawnDelay();

        void SetDespawnDelay(float value);

        float GetDespawnCheckInterval();

        void SetDespawnCheckInterval(float value);

        float GetDespawnRangeBuffer();

        void SetDespawnRangeBuffer(float value);

        void SetOriginalScale(Vector3 value);

        Vector3 GetOriginalScale();

        void SetSpawnArea(string spawnAreaGuid);

        string GetSpawnAreaGuid();

        SpawnArea GetSpawnArea();

        void SetWaveSpawner(string waveSpawnerGuid, string waveItemGuid);

        string GetWaveSpawnerGuid();

        string GetWaveItemGuid();

        WaveSpawner GetWaveSpawner();

        void SetSpawnedBy(SpawnedBy spawnedBy);

        SpawnedBy GetSpawnedBy();

        bool AllowDespawn();

        void SetSpawnOutsidePlayerFOV(bool value);

        bool GetSpawnOutsidePlayerFOV();

        void SetDespawnOutsidePlayerFOV(bool value);

        bool GetDespawnOutsidePlayerFOV();

        void ForceDespawn();

        void ImportSettings(SpawnableSettingsObject spawnableSettings);

        void ExportSettings();

        void EnableNavMeshAgent(bool value = true);

        void SetExtraAIData(ExtraAIData extraAIData);

        void OnSave();

        void OnLoad();

        void SetRandomChance(uint value);

        uint GetRandomChance();

        void SetHerdID(uint value);

        uint GetHerdID();

        void SetSpawnCategory(string categoryName);

        string GetSpawnCategory();
    } // interface ISpawnable
} // namespace Magique.SoulLink
