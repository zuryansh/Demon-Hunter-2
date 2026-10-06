using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UniversalConstants : MonoBehaviour
{
    Camera mainCam;
    public Camera MainCam
    {
        get { if (mainCam == null) Debug.LogError("Player was not found when trying to acess", gameObject); return mainCam; }
    }
    public Player _Player
    {
        get { if (player == null) Debug.LogWarning("Player was not found when trying to acess", gameObject); return player; }
    }
    public MapManager _MapManager
    {
        get { if (mapManager == null) Debug.LogError("mapManager was not found when trying to acess"); return mapManager; }
    }
    public static UniversalConstants inst;


    public MapManager MapManager => mapManager;

    Player player;
    MapManager mapManager;


    private void Awake()
    {
        inst = this;

        player = FindAnyObjectByType<Player>();
        mapManager = FindAnyObjectByType<MapManager>(); 
    }

    // Start is called before the first frame update
    void Start()
    {
        mainCam = Camera.main;
    }


}
