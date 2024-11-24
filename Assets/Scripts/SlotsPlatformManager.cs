using UnityEngine;

public class SlotsPlatformManager : SlotsManager
{
    public override void OnAnimationComplete(GameObject obj)
    {
        base.OnAnimationComplete(obj);

        obj.TryGetComponent<Box>(out Box box);

        box.animator.Play("boxopen");
    }
}
