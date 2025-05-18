using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// (c)2021-2023 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
    [AddComponentMenu("SoulLink/Lamp Control")]
    public class LampControl : MonoBehaviour
    {
        [Tooltip("Select the light which you want to turn on/off by time of day.")]
        [SerializeField]
        private Light _light;

        [SerializeField]
        private float _hourToTurnOn = 17.5f;

        [SerializeField]
        private float _hourToTurnOff = 5.5f;

        [Tooltip("List of objects that you want to emit light when turned on")]
        [SerializeField]
        private List<MeshRenderer> _emissiveObjects = new List<MeshRenderer>();

        [Tooltip("Select the color you want the emissive material to change to when the light is on.")]
        [SerializeField]
        private Color _lightColor;

        [Tooltip("List of objects to enable when the light is turnd on and disable when the light is turned off (e.g. particle effects on a torch)")]
        [SerializeField]
        private List<GameObject> _objectsToEnable = new List<GameObject>();

        [SerializeField]
        private bool _useHalo = false;

        private BaseGameTime _gameTime;
        private Behaviour _halo;

        private YieldInstruction _waitFiveSeconds = new WaitForSeconds(5f);

        void Start()
        {
            if (_light != null)
                StartCoroutine(CheckTimeOfDay());
        } // Start()

        void Awake()
        {
            _gameTime = FindObjectOfType<BaseGameTime>();
            _light = GetComponentInChildren<Light>();
            if (_light != null)
            {
                _halo = (Behaviour)_light.GetComponent("Halo");
            } // if
        } // Awake()

        IEnumerator CheckTimeOfDay()
        {
            bool forever = true;

            while (forever)
            {
                float hour = _gameTime.GetHour();
                float minute = _gameTime.GetMinute();
                float currentTime = hour + (minute / 60f);

                bool turnOn = (currentTime > _hourToTurnOn || currentTime < _hourToTurnOff);
                if (_light.enabled != turnOn)
                {
                    foreach (var obj in _objectsToEnable)
                    {
                        obj.SetActive(turnOn);
                    } // foreach

                    foreach (var obj in _emissiveObjects)
                    {
                        foreach (Material material in obj.materials)
                        {
                            material.SetColor("_EmissionColor", turnOn ? _lightColor : Color.black);
                        }
                    } // foreach

                    _light.enabled = turnOn;
                } // if

                if (_halo != null && _useHalo)
                {
                    _halo.enabled = turnOn;
                }

                // TODO: Based on how long it will take to get to the desired time, yield wait longer 
                // TODO: Possibly register with time system to be notified at the specified time
                yield return _waitFiveSeconds;
            } // while
        } // CheckTimeOfDay()
    } // class LampControl
} // namespace Magique.SoulLink
