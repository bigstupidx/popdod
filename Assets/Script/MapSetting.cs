using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapSetting : MonoBehaviour{

	// Game Over 
	public int MAX_HEIGHT;

	// Block number in one line
	public int MAX_WIDTH;
	public int MIN_WIDTH;

	// New Line Appearance frequency 
	public int MAX_PERIOD;
	public int MIN_PERIOD;

	[Space]
	// New Color Appearance Timing
	public int[] NEW_COLOR_APPEAR;
	[Range(0, 100)]
	public int GLASS_FREQUENCY;
	[Range(0, 100)]
	public int UNTOUCHABLE_FREQUENCY; 

	[Space]
	// Additional Rule
	public int COLOR_BURST;
	public int BLOCK_REPETITION_LIMIT;
	
	// "tap" is tap of Map
	public string GetLine(int tap)
	{
		string str = "";
		string ret = "";
		int length = (int)Random.Range(MIN_WIDTH, MAX_WIDTH + 1);
		int tmp, glassCount = 0;

		int maxColor = 4;
		if (NEW_COLOR_APPEAR[4] < tap)
			maxColor++;
		if (NEW_COLOR_APPEAR[5] < tap)
			maxColor++;

		for (int i = 0; i < maxColor; i++)
			for (int j = 0; j < BLOCK_REPETITION_LIMIT; j++)
				str += (char)('A' + i);

		for (int i = 0; i < length; i++)
		{
			if (Random.Range(0, 100) < GLASS_FREQUENCY && glassCount < BLOCK_REPETITION_LIMIT)
			{
				ret += 'G';
				glassCount++;
			}
			else
			{
				tmp = (int)Random.Range(0, str.Length);
				ret += str[tmp];
				str = str.Remove(tmp, 1);
			}
		}

		return ret;
	} 

	// "t" is left time of other side
	public int GetLeft(int t)
	{
		int i = 0;

		while (true)
		{
			i = (int)Random.Range(MIN_PERIOD, MAX_PERIOD);
			if (i != t)
				break;
		}

		return i;
	}
}