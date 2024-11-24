using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class BoxSlotsManager : SlotsManager
{
    private float popDuration = 0.2f;
    private float popScaleFactor = 0.5f;

    public override void OnSlotsFull(Box box)
    {
        base.OnSlotsFull(box);

        foreach (var slot in LevelManager.Instance.slotsPlatformManager.slots)
        {
            if (slot.Obj == box.gameObject)
            {
                slot.Obj = null;

                box.animator.Play("boxclose");
                StartCoroutine(WaitForAnimation(box.animator, "boxclose", () =>
                {
                    OnBoxFull(box);
                }));
            }
        }
    }

    private void OnBoxFull(Box box)
    {
        AnimateBoxOut(box);
    }

    public void AnimateBoxOut(Box box)
    {
        GameObject boxObj = box.gameObject;
        Vector3 originalScale = transform.localScale;

        LeanTween.scale(boxObj, originalScale * popScaleFactor, popDuration)
            .setEaseOutQuad()
            .setOnComplete(() =>
            {
                LeanTween.scale(boxObj, originalScale, popDuration)
                    .setEaseInQuad();

                LeanTween.move(boxObj, LevelManager.Instance.boxOutLocation.position, moveDuration)
                    .setEaseInOutQuad()
                    .setOnComplete(() =>
                    {
                        Debug.Log("Animation complete!");

                        LevelManager.Instance.boxGridManager.boxList.Remove(box);

                        Destroy(box.gameObject);

                        if (LevelManager.Instance.boxGridManager.boxList.Count == 0)
                        {
                            LevelManager.Instance.NextLevel();
                        }
                    });
            });
    }

    IEnumerator WaitForAnimation(Animator anim, string stateName, System.Action onComplete)
    {
        if (anim == null)
        {
            Debug.LogError("Animator is null.");
            yield break;
        }

        while (!anim.GetCurrentAnimatorStateInfo(0).IsName(stateName))
        {
            yield return null;
        }

        while (anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
        {
            yield return null;
        }

        onComplete?.Invoke();
    }
}
