using UnityEngine;

public class KillFeed : MonoBehaviour
{
    [SerializeField]
    GameObject killFeedItemPreFab;

    private void Start()
    {
        GameManager.instance.onPlayerKilledCallBack += OnKill;
    }
    public void OnKill(string player, string source)
    {
        GameObject item = Instantiate(killFeedItemPreFab, transform);
        item.GetComponent<KillFeedItem>().Setup(player, source);

        Destroy(item,3f);
    }
}
