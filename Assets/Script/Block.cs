using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block{

	public BlockType blockType;
	public BlockColor blockColor;
	public int checkNumber;

	public Block()
	{
		blockType = BlockType.normal;
		blockColor = BlockColor.A;
		checkNumber = 0;
	}

	public Block(BlockType t, BlockColor c)
	{
		blockType = t;
		blockColor = c;
		checkNumber = 0;
	}

	public void SetCheckNumber(int i)
	{
		checkNumber = i;
	}
}

public enum BlockType {
	normal,	
	untouchable,
	glass
}

public enum BlockColor {
	A = 0,
	B = 1,
	C = 2,
	D = 3,
	E = 4,
	F = 5,
	G = 6
}