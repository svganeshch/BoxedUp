using UnityEngine;

public class Box : MonoBehaviour, IBox
{
    public int rows;
    public int cols;

    public Material boxMaterial;
    private Color color;

    public void SetColor(Color color)
    {
        boxMaterial.color = color;
        this.color = color;
    }

    public Color GetColor()
    {
        return color;
    }
}
