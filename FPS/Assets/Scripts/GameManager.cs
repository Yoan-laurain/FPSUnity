using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GameManager : MonoBehaviour
{
    // On va stocker nos joueurs
    private static Dictionary<string, Player> players = new Dictionary<string, Player>();

    //Prefix du nom de nos joueur
    private const string playerIdPrefix = "Player";

    public MatchSettings matchSettings;

    public static GameManager instance;

    [SerializeField]
    private GameObject sceneCamera;

    public delegate void OnPlayerKilledCallBack(string player, string source);
    public OnPlayerKilledCallBack onPlayerKilledCallBack;


    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            return;
        }
    }

    public void SetSceneCameraActive(bool isActive)
    {
        if (sceneCamera == null) return;

        sceneCamera.SetActive(isActive);
    }

    public static void RegisterPlayer(string netID, Player player)
    {
        //On construit son nom
        string playerId = playerIdPrefix + netID;

        //On ajoute le joueur
        players.Add(playerId, player);

        // On renomme le joueur
        player.transform.name = playerId;
    }

    public static void UnRegistrerPlayer(string playerID)
    {
        //Retire le joueur de la liste des joueurs
        players.Remove(playerID);
    }

    public static Player GetPlayer(string playerId)
    {
        return players[playerId];
    }

    public static Player[] GetAllPlayers()
    {
        return players.Values.ToArray();
    }

}
