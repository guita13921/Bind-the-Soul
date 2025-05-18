// (c)2021 Magique Productions, Ltd. All rights reserved worldwide.

using UnityEngine;

namespace Magique.SoulLink
{
    [System.Serializable]
    public class TimeOfDayData
    {
        public string timeOfDayName;
        [Range(0f,23.99f)]
        public float startTime;
        [Range(0f, 23.99f)]
        public float endTime;

        public string GetRangeString()
        {
            return string.Format("{0} - {1}", Utility.BuildTimeString(startTime), Utility.BuildTimeString(endTime));
        } // GetRangeString()
    } // class TimeOfDayData
} // namespace Magique.SoulLink

