using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// (c)2022-2023 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
    public class RandomValuePool
    {
        public uint PoolSize = 100;
        public uint MinValue = 1;
        public uint MaxValue = 100;

        private List<uint> _values = new List<uint>();
        private IOrderedEnumerable<uint> _orderedValues;

        /// <summary>
        /// Clears the old values and generates a new set of random numbers. 
        /// </summary>
        /// <param name="poolSize"></param>
        public void GenerateValues(uint poolSize = 100)
        {
            PoolSize = poolSize;

            lock (_values)
            {
                _values.Clear();

                for (int i = 0; i < PoolSize; ++i)
                {
                    _values.Add((uint)Random.Range(MinValue, MaxValue + 1));
                } // for i

                // Descending order since we want to take the highest values first
                _orderedValues = _values.OrderByDescending(i => i);
            } // lock
        } // GenerateValues()

        /// <summary>
        /// Generates a new random value and adds to the list. This will be rejected if the total items is already at PoolSize
        /// </summary>
        public void GenerateNewValue()
        {
            // Do not exceed the maximum values allowed for this pool
            if (_values.Count == PoolSize) return;

            lock (_values)
            {
                _values.Add((uint)Random.Range(MinValue, MaxValue));

                // Re-sort the list
                _orderedValues = _values.OrderByDescending(i => i);
            } // lock

        } // GenerateNewValue()

        /// <summary>
        /// Attempts to find a value in the list that is <= the input parameter. Returns true if the value is found
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool FindValue(uint value, out uint valueFound)
        {
            bool found = false;
            valueFound = 0;

            if (_orderedValues == null) return false;
//            SoulLinkGlobal.DebugLog(null, "Available = " + _orderedValues.Count() + ", TotalSpawns = " + SoulLinkSpawner.Instance.TotalSpawns + ", MaxSpawns = " + SoulLinkSpawner.Instance.MaxSpawns +
//                ", Trying to find value = " + value);

            // If there are no more values then generate one now and see if it is in the correct range
            if (_orderedValues.Count() == 0)
            {
                var newValue = (uint)Random.Range(MinValue, MaxValue);
                if (value >= newValue)
                {
                    valueFound = newValue;
                    return true;
                } // if
            } // if

            lock (_orderedValues)
            {
                foreach (var listValue in _orderedValues)
                {
                    if (value >= listValue)
                    {
                        valueFound = listValue;
                        found = true;
                        break;
                    } // if
                } // foreach
            } // lock

            if (found)
            {
                UseValue(valueFound);
            } // if

            return found;
        } // FindValue()

        /// <summary>
        /// Return an unused value to the pool
        /// </summary>
        /// <param name="value"></param>
        public void ReturnValue(uint value)
        {
            if (value < MinValue) return;
            
            // Do not exceed the maximum values allowed for this pool
            if (_values.Count == PoolSize || value == 999) return;

            lock (_values)
            {
                _values.Add(value);

                _orderedValues = _values.OrderByDescending(i => i);
            } // lock
        } // ReturnValue()

        /// <summary>
        /// Remove the specified value from the list since it is being used
        /// </summary>
        /// <param name="value"></param>
        public void UseValue(uint value)
        {
            lock (_values)
            {
                if (_values.Contains(value))
                {
                    _values.Remove(value);
                }

                _orderedValues = _values.OrderByDescending(i => i);
            } // lock
        } // UseValue()
    } // class RandomValuePool
} // namespace Magique.SoulLink