using UnityEngine;

public class Door : Interactable
{
    public Key requiredKey;

    private void Awake()
    {
        interactSprite = requiredKey.sprite;
    }

    public override void Activate()
    {
        Player player = GameManager.instance.player;

        Key key = player.keys.Find(x => x.keyName == requiredKey.keyName);
        if (key != null)
        {
            player.keys.Remove(key);
            GameManager.instance.RemoveFromKeyList(key);
            Destroy(gameObject);
        }
    }
}
