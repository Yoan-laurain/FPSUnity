using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class KillFeedItem : MonoBehaviour
{
    [SerializeField]
    TMP_Text text;

    public void Setup(string player, string source)
    {
        text.text = source + " Killed " + player;
    }
}
