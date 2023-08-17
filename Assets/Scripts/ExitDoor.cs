using UnityEngine;

public class ExitDoor : Interactable
{
    public override void Activate()
    {
        GameManager.instance.LevelComplete();
    }
}
