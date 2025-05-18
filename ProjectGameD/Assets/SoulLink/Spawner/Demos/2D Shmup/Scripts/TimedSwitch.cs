using UnityEngine;
using UnityEngine.Events;

// (c)2023 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
    public class TimedSwitch : MonoBehaviour
    {
        [Tooltip("The behavior to switch from.")]
        [SerializeField]
        private MonoBehaviour _switchFrom;

        [Tooltip("The behavior to switch to.")]
        [SerializeField]
        private MonoBehaviour _switchTo;

        [Tooltip("How long to wait before performing the switch.")]
        [SerializeField]
        private float _switchTime = 10f;

        [SerializeField]
        private UnityEvent _onSwitchEventHandler;

        void Start()
        {
            CancelInvoke();
            Invoke("DoSwitch", _switchTime);
        } // Start()

        void DoSwitch()
        {
            _switchFrom.enabled = false;
            _switchTo.enabled = true;

            if (_onSwitchEventHandler != null)
            {
                _onSwitchEventHandler.Invoke();
            } // if
        } // DoSwitch()
    } // class TimedSwitch
} // namespace Magique.SoulLink
