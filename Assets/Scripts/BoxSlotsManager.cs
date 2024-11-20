using System.Linq;
using UnityEngine;

public class BoxSlotsManager : SlotsManager
{
    public override void OnSlotsFull(Box box)
    {
        base.OnSlotsFull(box);

        foreach (var slot in LevelManager.Instance.slotsPlatformManager.slots)
        {
            if (slot.Obj == box.gameObject)
            {
                slot.Obj = null;
                Destroy(box.gameObject);
            }
        }
    }
}
