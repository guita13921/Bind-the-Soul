using UnityEngine;

// (c)2022-2023 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
	public class Scroller : MonoBehaviour
	{
		public Vector2 scrollSpeed;
		
		void Start()
		{
			GetComponent<Rigidbody2D>().velocity = new Vector2(scrollSpeed.x, scrollSpeed.y);
		} // Start()
	} // class Scroller
} // namespace Magique.SoulLink