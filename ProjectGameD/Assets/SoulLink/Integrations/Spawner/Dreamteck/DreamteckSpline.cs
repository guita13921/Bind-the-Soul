#if DREAMTECK_SPLINES
using Dreamteck.Splines;
using UnityEngine;

// (c)2022 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
    public class DreamteckSpline : BaseSpline
    {
        private SplineComputer _spline;

        private void Awake()
        {
            _spline = GetComponent<SplineComputer>();
        } // Awake()

        /// <summary>
        /// Get the calculated length of the spline
        /// </summary>
        /// <returns></returns>
        override public float GetLength()
        {
            if (_spline == null) return 0f;
            return _spline.CalculateLength();
        } // GetLength()

        /// <summary>
        /// Get the world position at the specified distance
        /// </summary>
        /// <param name="distance"></param>
        /// <returns></returns>
        override public Vector3 GetWorldPosition(float distance)
        {
            if (_spline == null) return Vector3.zero;
            return _spline.EvaluatePosition(Mathf.Clamp(distance, 0f, 1f));
        } // GetWorldPosition()
    } // DreamteckSpline
} // namespace Magique.SoulLink
#endif