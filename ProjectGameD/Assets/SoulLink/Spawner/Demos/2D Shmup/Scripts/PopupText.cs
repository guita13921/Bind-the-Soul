using UnityEngine;
using TMPro;

// (c)2022-2023 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
	public class PopupText : MonoBehaviour, ISpawnEvents
	{
		enum State
		{
			normal,
			fade
		};

		public TextMeshPro textMesh;
		public float fadeRate = 1.0f;
		public float growRate = 1.0f;
		public float startScale = 1.0f;

		private State _state = State.normal;
		private float _alpha = 1.0f;
		private Color _color;
		private Transform _transform;
		private float _scale = 1.0f;

		void Awake()
		{
			_transform = transform;
		} // Aswake()

		// Update is called once per frame
		void Update()
		{
			switch (_state)
			{
				case State.normal:
					return;
				case State.fade:
					_alpha -= fadeRate * Time.deltaTime;
					textMesh.color = new Color(_color.r, _color.g, _color.b, _alpha);
					_scale += growRate * Time.deltaTime;
					_transform.localScale = new Vector3(_scale, _scale, _scale);
					break;
			} // switch
		} // Update()

		public void SetText(string someText, Color someColor)
		{
			textMesh.text = someText;
			textMesh.color = someColor;
			textMesh.color = new Color(someColor.r, someColor.g, someColor.b, 1.0f);
			_color = someColor;
		} // SetText()

		public void SetPosition(float x, float y, float z)
		{
			_transform.position = new Vector3(x, 10.0f, z);
		} // SetPosition()

		public void Fade()
		{
			_state = State.fade;
		} // Activate()

		public void OnSpawned()
		{
			GetComponent<Spawnable>().SetSpawnCategory("Messages");
			_state = State.normal;
			_alpha = 1.0f;
			_scale = startScale;

			Fade();
		} // OnSpawned()

        public void OnDespawned()
        {
            // nothing
        } // OnDespawned()
    } // class PopupText
} // namespace Magique.SoulLink