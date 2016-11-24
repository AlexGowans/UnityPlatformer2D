using UnityEngine;
using System.Collections;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.SceneManagement;

[System.Serializable]
public class Game : MonoBehaviour {

    public static Game control;

    [HideInInspector]
    public GameObject savedCheckpoint = null;
    [HideInInspector]
    public float savedPosX;
    [HideInInspector]
    public float savedPosY;
    [HideInInspector]
    public float savedPosZ;
    [HideInInspector]
    public int savedGravDir;
    [HideInInspector]
    public bool[] savedItemGet = new bool[9]; //For the towers that give items

    //Items
    public bool gotGravityPack;
    public bool gotBlaster;
    public bool gotGravityGun;
    public bool gotIceBoots;
    public bool gotSmashBoots;
    public bool gotChargeShot;


    public bool loadingGame = false;


    //Makes it stay in the game if scene change or destroy if one exists already
    void Awake () {

        if (control == null) {
            DontDestroyOnLoad(this.gameObject);
            control = this;
        }
        else if (control != this) {
            Destroy(gameObject);
        }
        for (int i = 0; i < savedItemGet.Length; i++) {           
            savedItemGet[i] = false;
        }
    }

    public void Save() {

        GetCheckpointPos();
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/mySaviour.corey");

        Debug.Log(Application.persistentDataPath);

        PlayerData data = new PlayerData();
        data.savedPosX = savedPosX;
        data.savedPosY = savedPosY;
        data.savedPosZ = savedPosZ;
        data.savedGravDir = savedGravDir;
        for (int i = 0; i < savedItemGet.Length; i++) {
            data.savedItemGet[i] = savedItemGet[i];
        }
        data.gotGravityPack = gotGravityPack;
        data.gotBlaster = gotBlaster;
        data.gotGravityGun = gotGravityGun;
        data.gotIceBoots = gotIceBoots;
        data.gotSmashBoots = gotSmashBoots;
        data.gotChargeShot = gotChargeShot;

        bf.Serialize(file, data);
        file.Close();

        Debug.Log("SAVED");
    }

    public void Load() {

        if (File.Exists(Application.persistentDataPath + "/mySaviour.corey")) {
            loadingGame = true;
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/mySaviour.corey", FileMode.Open);
            PlayerData data = (PlayerData)bf.Deserialize(file);
            file.Close();

            savedPosX = data.savedPosX;
            savedPosY = data.savedPosY;
            savedPosZ = data.savedPosZ;
            savedGravDir = data.savedGravDir;
            for (int i = 0; i < data.savedItemGet.Length; i++) {
                savedItemGet[i] = data.savedItemGet[i];
            }
            gotGravityPack = data.gotGravityPack;
            gotBlaster = data.gotBlaster;
            gotGravityGun = data.gotGravityGun;
            gotIceBoots = data.gotIceBoots;
            gotSmashBoots = data.gotSmashBoots;
            gotChargeShot = data.gotChargeShot;

            loadingGame = true;

            Debug.Log("LOADING");
        }
    }


    void GetCheckpointPos() { //So that we save a var not a gameobject
        if (savedCheckpoint != null) {
            savedPosX = savedCheckpoint.transform.position.x;
            savedPosY = savedCheckpoint.transform.position.y;
            savedPosZ = savedCheckpoint.transform.position.z;
        }
    }

    public void GotItem(int id) {
        
        if (id < savedItemGet.Length) {
            savedItemGet[id] = true;
        }
        
    }
    
}

[Serializable]
class PlayerData {
    public float savedPosX;
    public float savedPosY;
    public float savedPosZ;

    public int savedGravDir;

    public bool[] savedItemGet = new bool[9];

    //Items
    public bool gotGravityPack;
    public bool gotBlaster;
    public bool gotGravityGun;
    public bool gotIceBoots;
    public bool gotSmashBoots;
    public bool gotChargeShot;
}
