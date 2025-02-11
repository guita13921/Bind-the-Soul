using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjFadingWall : MonoBehaviour
{
    public float fadeSpeed; // Speed of the fade
    public float fadeAmount; // Target alpha value (transparent or opaque)
    public float fadeDistance; // Distance threshold for fading

    [SerializeField]
    public List<float> originalOpacity = new List<float>(); // Store original opacity for each material

    [SerializeField]
    public List<Material> material = new List<Material>(); // List of materials to fade

    [SerializeField]
    public List<GameObject> objectToFind = new List<GameObject>(); // List of game objects to apply fade to

    public Transform player; // Reference to the player

    int numberOfChild; // Number of child objects

    void Start()
    {
        fadeSpeed = 10f;
        fadeAmount = 0.1f; // Target fade opacity (you can adjust this based on your needs)
        fadeDistance = 5f; // Distance threshold for fading (you can adjust this value)
        numberOfChild = transform.childCount;

        // Initialize the objectToFind and material lists
        for (int i = 0; i < numberOfChild; i++)
        {
            GameObject child = transform.GetChild(i).gameObject;
            objectToFind.Add(child);

            Renderer renderer = child.GetComponent<Renderer>();
            if (renderer != null)
            {
                Material mat = renderer.material;
                material.Add(mat);

                // Set material to transparent mode
                SetMaterialToTransparent(mat);

                // Store the original opacity
                originalOpacity.Add(mat.color.a);
            }
        }
    }

    void Update()
    {
        // Check distance from the player to each object
        float distance = Vector3.Distance(player.position, transform.position);

        if (distance <= fadeDistance)
        {
            FadeNow();
        }
        else
        {
            ResetFade();
        }
    }

    // Sets the material to transparent mode with correct settings for URP
    void SetMaterialToTransparent(Material mat)
    {
        mat.SetFloat("_Surface", 1); // 1 = Transparent
        mat.SetFloat("_Blend", 1);    // Set the blend mode (alpha blending)
        mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        mat.SetInt("_ZWrite", 0);
        mat.DisableKeyword("_ALPHATEST_ON");
        mat.EnableKeyword("_ALPHABLEND_ON");
        mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        mat.renderQueue = 3000; // Transparent render queue
    }

    void FadeNow()
    {
        for (int i = 0; i < material.Count; i++)
        {
            // Get the current color of the material
            Color currentColor = material[i].color;

            // Smoothly transition alpha value using Lerp
            float alpha = Mathf.Lerp(currentColor.a, fadeAmount, fadeSpeed * Time.deltaTime);
            material[i].SetColor("_BaseColor", new Color(currentColor.r, currentColor.g, currentColor.b, alpha));
        }
    }

    void ResetFade()
    {
        for (int i = 0; i < material.Count; i++)
        {
            // Get the current color of the material
            Color currentColor = material[i].color;

            // Smoothly transition back to the original alpha value using Lerp
            float alpha = Mathf.Lerp(currentColor.a, originalOpacity[i], fadeSpeed * Time.deltaTime);
            material[i].SetColor("_BaseColor", new Color(currentColor.r, currentColor.g, currentColor.b, alpha));
        }
    }
}
