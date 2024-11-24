using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : MonoBehaviour, IPackageItem
{
    public bool isSloted = false;
    public Material[] targetMaterials;
    public BoxSlotsManager slotsManager;
    [SerializeField] private int size;
    [SerializeField] private float rayDistance = 5f;
    [SerializeField] private bool isBoxBlocked = false;

    private float blockColorPercentage = 0.35f;

    public Animator animator;
    private BoxCollider boxCollider;
    private MeshRenderer[] meshRenderers;
    private List<Material> boxMaterials = new List<Material>();

    private Color color;
    private Color blockedColor;

    private Coroutine boxBlockCheckCoroutine;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider>();
        slotsManager = GetComponentInChildren<BoxSlotsManager>();
        meshRenderers = GetComponentsInChildren<MeshRenderer>();

        foreach (var renderer in meshRenderers)
        {
            foreach (var targetMaterial in targetMaterials)
            {
                if (renderer.sharedMaterial == targetMaterial)
                    boxMaterials.Add(renderer.material);
            }
        }
    }

    private void Start()
    {
        boxBlockCheckCoroutine = StartCoroutine(BoxBlockCheck());
    }

    public bool PlaceBox()
    {
        if (!isBoxBlocked)
        {
            LevelManager.Instance.slotsPlatformManager.SetSlot(gameObject);
            isSloted = true;
            StopCoroutine(boxBlockCheckCoroutine);

            return true;
        }

        return false;
    }

    private IEnumerator BoxBlockCheck()
    {
        while (true)
        {
            Vector3 center = boxCollider.center;
            Vector3 size = boxCollider.size;

            Transform transform = boxCollider.transform;

            Vector3[] localTopCorners = new Vector3[5];
            localTopCorners[0] = center + new Vector3(-size.x, size.y, -size.z) * 0.45f; // Top-Left-Back
            localTopCorners[1] = center + new Vector3(size.x, size.y, -size.z) * 0.45f;  // Top-Right-Back
            localTopCorners[2] = center + new Vector3(-size.x, size.y, size.z) * 0.45f;  // Top-Left-Front
            localTopCorners[3] = center + new Vector3(size.x, size.y, size.z) * 0.45f;   // Top-Right-Front
            localTopCorners[4] = center; // Middle

            bool isCurrentlyBlocked = false;

            foreach (Vector3 localCorner in localTopCorners)
            {
                Vector3 worldCorner = transform.TransformPoint(localCorner);
                Ray ray = new Ray(worldCorner, Vector3.up);
                if (Physics.Raycast(ray, out RaycastHit hit, rayDistance))
                {
                    isCurrentlyBlocked = true;
                    Debug.DrawRay(worldCorner, Vector3.up * rayDistance, Color.red);
                    break;
                }

                Debug.DrawRay(worldCorner, Vector3.up * rayDistance, Color.green);
            }

            if (isBoxBlocked != isCurrentlyBlocked)
            {
                isBoxBlocked = isCurrentlyBlocked;
                ApplyColor(isBoxBlocked ? blockedColor : color);
            }

            yield return new WaitForSeconds(0.2f);
        }
    }

    public void SetColor(Color newColor)
    {
        color = newColor;

        ApplyColor(newColor);
        InitialiseBlockedColor();
    }

    private void ApplyColor(Color newColor)
    {
        foreach (var material in boxMaterials)
        {
            material.color = newColor;
        }
    }

    public void InitialiseBlockedColor()
    {
        blockColorPercentage = Mathf.Clamp01(blockColorPercentage);

        blockedColor = new Color(
            color.r * (1 - blockColorPercentage),
            color.g * (1 - blockColorPercentage),
            color.b * (1 - blockColorPercentage),
            color.a
        );
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
