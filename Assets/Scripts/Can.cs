using UnityEngine;

public class Can : MonoBehaviour, IPackageItem
{
    private Color color;
    private MeshRenderer meshRenderer;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    public Color GetColor()
    {
        return color;
    }

    public void SetColor(Color color)
    {
        meshRenderer.material.color = color;

        this.color = color;
    }

    public int GetSize()
    {
        throw new System.NotImplementedException();
    }
}
