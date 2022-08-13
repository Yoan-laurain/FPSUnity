using UnityEngine;

public class ScoreBoard : MonoBehaviour
{
    [SerializeField]
    GameObject playerScoreBoardItem;

    [SerializeField]
    Transform playerScoreBoardList;

    private void OnEnable()
    {
        // Récupérer une array de tous les joeuurs du serveurs
        Player[] players = GameManager.GetAllPlayers();

        //Loop sur l'array et mise en place d'une ligne de UI pour chaque joueur avec leurs données
        foreach (Player player in players)
        {
            //On créer la ligne
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
