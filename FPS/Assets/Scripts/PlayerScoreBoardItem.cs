using UnityEngine;
using TMPro;

public class PlayerScoreBoardItem : MonoBehaviour
{
    [SerializeField]
    TMP_Text usernameText;

    [SerializeField]
    TMP_Text killsText;

    [SerializeField]
    TMP_Text deathsText;

    public void Setup(Player player)
    {
        usernameText.text = player.name;
        killsText.text = "Kills : " + player.kills;
        deathsText.text = "Deaths : " + player.death;

    }
}
