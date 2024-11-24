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

                LevelManager.Instance.boxGridManager.boxList.Remove(box);
                Destroy(box.gameObject);

                if (LevelManager.Instance.boxGridManager.boxList.Count == 0)
                {
                    LevelManager.Instance.NextLevel();
                }
            }
        }
    }
}
