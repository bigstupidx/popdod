using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map {

	public struct Pair<T, U>
	{
		public Pair(T first, U second)
		{
			this.First = first;
			this.Second = second;
		}

		public T First { get; set; }
		public U Second { get; set; }
	};
	
	public List<List<Block>> container;

	public int tap;
	public int removedLine;
	public int removedBlock;
	
	public Map()
	{
		container = new List<List<Block>>();

		tap = 0;
		removedLine = 0;
		removedBlock = 0;
	}

	public Map(UserStatus us)
	{
		container = new List<List<Block>>();

		string[] str = us.MapInfo.Split('\n');

		for(int i = 0; i < str.Length; i++)
		{
			AddLineLeft(str[i]);
		}

		tap = us.tap;
		removedLine = us.removedBlocks;
		removedBlock = us.removedLines;
	}

	#region Add Blocks

	/**
	 *
	 *	Add Blocks Functions
	 *
	 */

	public List<Block> ParseString(string str)
	{
		/*
			A B C D E F --> normal blocks
			a b c d e f --> untouchable blocks
			G g			--> glass blocks (not classified between G and g) 
		*/

		List<Block> tmp = new List<Block>();

		for (int i = 0; i < str.Length; i++)
		{
			if (str[i] >= 'a' && str[i] <= 'f')
			{
				tmp.Add(new Block(BlockType.untouchable, (BlockColor)(str[i] - 'a')));
			}
			else if(str[i] >= 'A' && str[i] <= 'F')
			{
				tmp.Add(new Block(BlockType.normal, (BlockColor)(str[i] - 'A')));
			}
			else if(str[i] == 'g' || str[i] == 'G')
			{
				tmp.Add(new Block(BlockType.glass, BlockColor.G));
			}
			else
			{
				Debug.Log("Invailed Input! " + str);
				return null;
			}
		}

		return tmp;
	}

	public void AddLineRight(string str)
	{
		List<Block> tmp = ParseString(str);
		container.Insert(0, tmp);
	}

	public void AddLineLeft(string str)
	{
		List<Block> tmp = ParseString(str);
		container.Add(tmp);
	}

	#endregion

	#region Search Blocks

	/**
	 *
	 *	Search Blocks Functions
	 *
	 */

	public bool Contains(Block b)
	{
		for (int i = 0; i < container.Count; i++)
			if (container[i].Contains(b))
				return true;

		return false;
	}

	public Pair<int, int> FindIndex(Block b) {
		for (int i = 0; i < container.Count; i++)
			for (int j = 0; j < container[i].Count; j++)
				if (container[i][j] == b)
					return new Pair<int, int>(i, j);

		return new Pair<int, int>(-1, -1);
	}

	public Block FindBlock(int line, int idx)
	{
		return container[line][idx];
	}

	public List<Block> FindBlockListConnected(Block b)
	{
		List<Block> tmp = new List<Block>();
		List<Block> ret = new List<Block>();

		Queue<Block> myQueue = new Queue<Block>();
		myQueue.Enqueue(b);

		while(myQueue.Count > 0)
		{
			Block myBlock = myQueue.Dequeue();
			tmp = FindBlockListDistance(myBlock, 3);
			for (int i = 0; i < tmp.Count; i++)
				if (!ret.Contains(tmp[i]) && (tmp[i].blockColor == b.blockColor || tmp[i].blockColor == BlockColor.G))
				{
					ret.Add(tmp[i]);
					myQueue.Enqueue(tmp[i]);
				}
		}

		return ret;
	}

	public List<Block> FindBlockListConnected(int line, int idx)
	{
		List<Block> tmp = new List<Block>();
		List<Block> ret = new List<Block>();

		Queue<Block> myQueue = new Queue<Block>();
		Block b = FindBlock(line, idx);
		myQueue.Enqueue(FindBlock(line, idx));

		while (myQueue.Count > 0)
		{
			Block myBlock = myQueue.Dequeue();
			tmp = FindBlockListDistance(myBlock, 3);
			for (int i = 0; i < tmp.Count; i++)
				if (!ret.Contains(tmp[i]) && (tmp[i].blockColor == b.blockColor || tmp[i].blockColor == BlockColor.G))
					ret.Add(tmp[i]);
		}

		return ret;
	}

	public List<Block> FindBlockListDistance(Block b, int dist)
	{
		List<Block> tmp = new List<Block>();

		for (int i = 0; i < container.Count; i++)
			for (int j = 0; j < container[i].Count; j++)
				if (GetDistance(container[i][j], b) <= dist)
					tmp.Add(container[i][j]);

		return tmp;
	}

	public List<Block> FindBlockListDistance(int line, int idx, int dist)
	{
		List<Block> tmp = new List<Block>();
		Block b = FindBlock(line, idx);

		for (int i = 0; i < container.Count; i++)
			for (int j = 0; j < container[i].Count; j++)
				if (GetDistance(container[i][j], b) <= dist)
					tmp.Add(container[i][j]);

		return tmp;
	}

	public List<Block> FindBlockListColor(BlockColor c)
	{
		List<Block> tmp = new List<Block>();

		for (int i = 0; i < container.Count; i++)
			for (int j = 0; j < container[i].Count; j++)
				if (container[i][j].blockColor == c)
					tmp.Add(container[i][j]);

		return tmp;
	}

	public List<Block> FindBlockListLine(Block b)
	{
		List<Block> tmp = new List<Block>();

		int line = FindIndex(b).First;
		for (int i = 0; i < container[line].Count; i++)
			tmp.Add(container[line][i]);

		return tmp;
	}

	public List<Block> FindBlockListLine(int line)
	{
		List<Block> tmp = new List<Block>();

		for (int i = 0; i < container[line].Count; i++)
			tmp.Add(container[line][i]);

		return tmp;
	}

	public int GetDistance(Block A, Block B)
	{
		Pair<int, int> aidx = FindIndex(A);
		Pair<int, int> bidx = FindIndex(B);

		int xDistance = Mathf.Abs((2 * aidx.Second - (container[aidx.First].Count - 1)) - (2 * bidx.Second - (container[bidx.First].Count - 1)));
		int yDistance = Mathf.Abs(aidx.First - bidx.First) * 2;

		return xDistance + yDistance;
	}

	public int GetCountLine()
	{
		return container.Count;
	}

	public int GetCountBlock()
	{
		int sum = 0;
		for (int i = 0; i < container.Count; i++)
			sum += container[i].Count;

		return sum;
	}

	#endregion

	#region Delete Blocks

	/**
	 *
	 *	Del Blocks Functions
	 *
	 */

	public int RemoveEmptyLine()
	{
		int ret = 0;

		for (int i = container.Count - 1; i >= 0; i--)
		{
			if (container[i].Count <= 0)
			{
				container.RemoveAt(i);
				ret++;
			}
		}

		return ret;
	}

	public int RemoveBlockNormal(Block b)
	{
		List<Block> tmp = FindBlockListConnected(b);
		int ret = 0;

		for (int i = 0; i < container.Count; i++)
		{
			for (int j = container[i].Count - 1; j >= 0; j--)
			{
				if (tmp.Contains(container[i][j]))
				{
					container[i].RemoveAt(j);
					ret++;
				}
			}
		}

		return ret;
	}

	public int RemoveBlockColor(BlockColor c)
	{
		List<Block> tmp = FindBlockListColor(c);
		int ret = 0;

		for (int i = 0; i < container.Count; i++)
		{
			for (int j = container[i].Count - 1; j >= 0; j--)
			{
				if (tmp.Contains(container[i][j]))
				{
					container[i].RemoveAt(j);
					ret++;
				}
			}
		}

		return ret;
	}

	public int RemoveBlockDistance(Block b, int dist)
	{
		List<Block> tmp = FindBlockListDistance(b, dist);
		int ret = tmp.Count;

		for (int i = 0; i < container.Count; i++)
			for (int j = container[i].Count - 1; j >= 0; j--)
				if (tmp.Contains(container[i][j]))
					container[i].RemoveAt(j);

		return ret;
	}

	public int RemoveBlockLine(Block b)
	{
		int line = FindIndex(b).First;
		int ret = container[line].Count;
		container[line].Clear();

		return ret;
	}

	public int RemoveBlockSingle(Block b)
	{
		Pair<int, int> idx = FindIndex(b);
		container[idx.First].RemoveAt(idx.Second);

		return 1;
	}

	#endregion

	/**
	 *
	 *	Else
	 *
	 */


	/**
	 *
	 *	Debug
	 *
	 */

	public void PrintMapInfo()
	{
		string str = "";

		for(int i=  0; i < container.Count; i++)
		{
			for(int j = 0; j < container[i].Count; j++)
			{
				if (container[i][j].blockType != BlockType.glass)
					str += (char)((container[i][j].blockType != BlockType.untouchable) ? container[i][j].blockColor + 'A' : container[i][j].blockColor + 'a');
				else
					str += 'G';
			}

			str += '\n';
		}

		Debug.Log(str);
	}

	public string GetMapInfo()
	{
		string str = "";

		for (int i = 0; i < container.Count; i++)
		{
			for (int j = 0; j < container[i].Count; j++)
			{
				if (container[i][j].blockType != BlockType.glass)
					str += (char)((container[i][j].blockType != BlockType.untouchable) ? container[i][j].blockColor + 'A' : container[i][j].blockColor + 'a');
				else
					str += 'G';
			}

			str += '\n';
		}

		return str;
	}
}
