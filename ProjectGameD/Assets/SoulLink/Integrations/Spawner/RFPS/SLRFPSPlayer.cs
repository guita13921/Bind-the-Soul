#if SOULLINK_USE_RFPS
using UnityEngine;

namespace Magique.SoulLink
{
    public class SLRFPSPlayer : BaseSoulLinkPlayer
    {
        private FPSPlayer _healthController;

        override public void Awake()
        {
            base.Awake();

            _healthController = GetComponent<FPSPlayer>();
        } // Awake()

#region Health Interface

        override public int GetCurrentHealth()
        {
            return (int)_healthController.hitPoints;
        } // GetCurrentHealth()

        override public int GetMaximumHealth()
        {
            return (int)_healthController.maximumHitPoints;
        } // GetMaximumHealth()

        override public void OnDamaged(int damageAmount, Transform target, Vector3 hitLocation, string damageType = "Slashing")
        {
            _healthController.ApplyDamage(damageAmount, target);
        } // OnDamaged()

        override public bool IsAlive()
        {
            return _healthController.hitPoints > 0;
        } // IsAlive()

#endregion

    } // class BaseSoulLinkPlayer
} // namespace Magique.SoulLink
#endif