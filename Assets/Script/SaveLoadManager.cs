using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public static class SaveLoadManager
{

	public static void SavePlayer(UserStatus us)
	{
		BinaryFormatter bf = new BinaryFormatter();
		FileStream stream = new FileStream(Application.persistentDataPath + "/save.popdod", FileMode.Create);

		bf.Serialize(stream, us);
		stream.Close();
	}

	public static UserStatus LoadPlayer()
	{
		if (File.Exists(Application.persistentDataPath + "/save.popdod"))
		{
			BinaryFormatter bf = new BinaryFormatter();

			FileStream stream = new FileStream(Application.persistentDataPath + "/save.popdod", FileMode.Open);

			UserStatus dat = bf.Deserialize(stream) as UserStatus;

			stream.Close();
			return dat;
		}
		else
		{
			UserStatus dat = new UserStatus();
			dat.rightLeft = -100;
			return dat;
		}
	}

	public static void DeletePlayer()
	{
		if (File.Exists(Application.persistentDataPath + "/save.popdod"))
		{
			BinaryFormatter bf = new BinaryFormatter();
			FileStream stream = new FileStream(Application.persistentDataPath + "/save.popdod", FileMode.Create);

			UserStatus dat = new UserStatus();
			dat.rightLeft = -100;
			bf.Serialize(stream, dat);
			stream.Close();
		}
	}
}

[Serializable]
public class UserStatus
{
	public bool Mute;
	public bool Darkness;

	public int Highscore;
	public int Playcount;

	public string MapInfo;
	public string right;
	public string left;
	public int rightLeft;
	public int leftLeft;

	public int tap;
	public int removedBlocks;
	public int removedLines;
	public int score;

	public UserStatus() { }

	public void SetProperty(bool m, bool d)
	{
		Mute = m;
		Darkness = d;
	}

	public void SetUserStatus(int i, int c)
	{
		Highscore = i;
		Playcount = c;
	}

	public void SetMapInformation(Map m, string r, string l, int rr, int ll, int s)
	{
		MapInfo = m.GetMapInfo();
		tap = m.tap;
		removedBlocks = m.removedBlock;
		removedLines = m.removedLine;

		right = r;
		left = l;
		rightLeft = rr;
		leftLeft = ll;
		score = s;
	}
}

