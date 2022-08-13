using UnityEngine;

public class ScoreBoard : MonoBehaviour
{
    [SerializeField]
    GameObject playerScoreBoardItem;

    [SerializeField]
    Transform playerScoreBoardList;

    private void OnEnable()
    {
        // R�cup�rer une array de tous les joeuurs du serveurs
        Player[] players = GameManager.GetAllPlayers();

        //Loop sur l'array et mise en place d'une ligne de UI pour chaque joueur avec leurs donn�es
        foreach (Player player in players)
        {
            //On cr�er la ligne
            GameObject itemGO = Instantiate(playerScoreBoardItem,playerScoreBoardList);
            PlayerScoreBoardItem item = itemGO.GetComponent<PlayerScoreBoardItem>();

            if(item != null)
            {
                // On remplit la ligne
                item.Setup(player);
            }
        }

    }

    private void OnDisable()
    {
        // On vide notre liste

        foreach (Transform child in playerScoreBoardList)
        {
            Destroy(child.gameObject);
        }
    }
}
