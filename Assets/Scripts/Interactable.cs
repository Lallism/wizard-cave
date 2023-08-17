using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    public Sprite interactSprite;

    public abstract void Activate();
}
