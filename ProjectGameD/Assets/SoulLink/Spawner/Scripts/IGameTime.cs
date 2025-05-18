using UnityEngine.SceneManagement;
using static Magique.SoulLink.SoulLinkSpawnerTypes;

// (c)2020-2023 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
    public interface IGameTime
    {
        bool GetSoulLinkControlsTime();

        void GetComponents();

        void SetHour(float hour);

        float GetHour();

        void SetMinute(float minute);

        float GetMinute();

        int GetDay();

        void SetDay(int value);

        int GetMonth();

        void SetMonth(int value);

        int GetYear();

        void SetYear(int value);

        Season GetSeason();

        void SetSeason(Season value);

        float GetTime();

        float GetGameDeltaTime();

        void SetDayLength(float value);

        float GetDayLength();

        void Refresh();

        void Pause();

        void UnPause();

        float GetSecondsAdded();

        void UpdateSecondsAdded();

        void OnSceneLoaded(Scene scene, LoadSceneMode mode);
    } // interface IGameTime
} // namespace Magique.SoulLink