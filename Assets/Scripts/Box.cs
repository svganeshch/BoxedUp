using System.Collections.Generic;
using UnityEngine;

public class Box : MonoBehaviour, IPackageItem
{
    [SerializeField] private int size;

    public Material targetMaterial;
    public MeshRenderer[] meshRenderers;
    public List<Material> boxMaterials = new List<Material>();

    private Color color;

    private void Awake()
    {
        meshRenderers = GetComponentsInChildren<MeshRenderer>();

        foreach (var renderer in meshRenderers)
        {
            if (renderer.sharedMaterial == targetMaterial)
                boxMaterials.Add(renderer.material);
        }
    }

    public void SetColor(Color color)
    {
        foreach (var material in boxMaterials)
        {
            material.color = color;
        }

        this.color = color;
    }

    public Color GetColor()
    {
        return color;
    }

    public int GetSize()
    {
        return size;
    }
}
