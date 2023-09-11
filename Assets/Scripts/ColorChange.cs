using UnityEngine;

public class ColorChange : MonoBehaviour
{
    public float colorChangeSpeed = 1.0f;  // Vitesse de changement de couleur

    private Material material;
    private float hueValue = 0.0f;

    private void Start()
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            material = renderer.material;
        }
        else
        {
            Debug.LogError("Aucun composant de rendu (Renderer) attaché à cet objet.");
            enabled = false;
        }
    }

    private void Update()
    {
        hueValue += Time.deltaTime * colorChangeSpeed;
        hueValue = Mathf.Repeat(hueValue, 1.0f);
        Color newColor = Color.HSVToRGB(hueValue, 1.0f, 1.0f);
        material.color = newColor;
    }
}
