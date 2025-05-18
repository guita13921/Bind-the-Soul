using UnityEngine;
using System.Collections;

// (c)2022-2023 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
	public class Impact : MonoBehaviour
	{
		private ParticleSystem[] _particles;

		void Awake()
		{
			_particles = GetComponentsInChildren<ParticleSystem>();
		} // Awake()

		virtual public void OnSpawned()
		{
            for (int i = 0; i < _particles.Length; ++i)
			{
				_particles[i].time = 0.0f;
				_particles[i].Play();
	    	} // for i
		} // OnSpawned()
    } // class Impact
} // namespace Magique.SoulLink