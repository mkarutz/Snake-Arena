using UnityEngine;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;
using UnityEngine.SceneManagement;

public class PlayerProfile : MonoBehaviour 
{
	private const string FILE_NAME = "player_profile.dat";
	private BinaryFormatter bf = new BinaryFormatter();

	private int coins = 50;
	private string nickname = "";
	private int skin = 0;
	private bool adsDisabled = false;
	private int inputScheme = 0;

	private static PlayerProfile instance = null;

	public static PlayerProfile Instance()
	{
		return instance;
	}


	void Awake ()
	{
		if (MakeSingletonOrDestory()) {
			return;
		}

		LoadFromFile();
	}


	void OnApplicationQuit()
	{
		SaveToFile();
	}


	bool MakeSingletonOrDestory()
	{
		if (PlayerProfile.instance != null) {
			Destroy(gameObject);
			return true;
		}

		PlayerProfile.instance = this;
		DontDestroyOnLoad(gameObject);
		return false;
	}


	void OnDestroy()
	{
		if (PlayerProfile.instance == this) {
			PlayerProfile.instance = null;
		}
	}


	void LoadFromFile()
	{
		if (File.Exists(FilePath())) {
			FileStream file = File.Open(FilePath(), FileMode.Open);
			PlayerProfilePOCO poco = (PlayerProfilePOCO) bf.Deserialize(file);
			ReplicateFromPOCO(poco);
		}
	}


	void ReplicateFromPOCO(PlayerProfilePOCO poco) 
	{
		coins = poco.coins;
		nickname = poco.Nickname;
		skin = poco.skin;
		adsDisabled = poco.adsDisabled;
		inputScheme = poco.inputScheme;
	}


	void SaveToFile()
	{
		FileStream file = File.Open(FilePath(), FileMode.Create);
		bf.Serialize(file, new PlayerProfilePOCO(this));
		file.Close();
	}


	string FilePath()
	{
		return Application.persistentDataPath + "/" + FILE_NAME;
	}


	public int Coins {
		get {
			return this.coins;
		}
		set {
			coins = value;
			SaveToFile();
		}
	}

	public string Nickname {
		get {
			return this.nickname;
		}
		set {
			nickname = value;
			Debug.Log("Player name set: " + nickname);
			SaveToFile();
		}
	}

	public int Skin {
		get {
			return this.skin;
		}
		set {
			skin = value;
			SaveToFile();
		}
	}

	public bool AdsDisabled {
		get {
			return this.adsDisabled;
		}
		set {
			this.adsDisabled = value;
			SaveToFile();
		}
	}

	public int ControlScheme {
		get {
			return this.inputScheme;
		}
		set {
			this.inputScheme = value;
			SaveToFile();
		}
	}
}


[Serializable]
class PlayerProfilePOCO
{
	public int coins = 50;
	public string Nickname = "";
	public int skin = 0;
	public bool adsDisabled = false;
	public int inputScheme = 0;

	public PlayerProfilePOCO(PlayerProfile profile)
	{
		coins = profile.Coins;
		Nickname = profile.Nickname;
		skin = profile.Skin;
		adsDisabled = profile.AdsDisabled;
		inputScheme = profile.ControlScheme;
	}
}
