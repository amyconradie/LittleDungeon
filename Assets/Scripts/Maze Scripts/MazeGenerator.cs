using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

public class MazeGenerator
{

	public int enemyCount;
	public int mushroomCount;
	public int potionCount;
	public int mazeDimensions = 9;
	private IntMaze intMaze;

	private int minMazeSize = 3; // 3-9 for empty room, 11+ for maze
	private int maxMazeSize = 23;

	System.Random _random = new System.Random();


	public int[,] CreateMaze(int enemyCount, int mushroomCount, int potionCount, int mazeDimensions)
    {

		if (enemyCount > 0 && enemyCount < 10)
        {
			this.enemyCount = enemyCount;
        }

		if (mushroomCount > 0 && mushroomCount < 20)
		{
			this.mushroomCount = mushroomCount;
		}

		if (potionCount > 0 && potionCount < 10)
		{
			this.potionCount = potionCount;
		}

		if (mazeDimensions % 2 == 0)
		{
			Debug.Log("Maze Size must be an odd number!!");
		}

		if (mazeDimensions > maxMazeSize)
		{
			mazeDimensions = maxMazeSize;
		}

		if (mazeDimensions >= minMazeSize && mazeDimensions <= maxMazeSize)
		{
			this.mazeDimensions = mazeDimensions;
		}

		intMaze = new IntMaze(this.mazeDimensions, this.enemyCount, this.mushroomCount, this.potionCount);

		//PrintMaze();

		return intMaze.Get2DMaze();
	}

	public void PrintMaze()
    {
		string mazeString = "";
		int[,] maze = intMaze.Get2DMaze();
		for (int i = 0; i < mazeDimensions; i++)
        {
			for (int j = 0; j < mazeDimensions; j++)
			{
				mazeString += maze[i, j];
			}
			mazeString += "\n";
		}
		Debug.Log(mazeString);

	}


	public class IntMaze
	{
		readonly int rows, columns;
		readonly int empty = 0, wall = 1;
		readonly int[] maze;
		readonly int[] dirs = new int[] { 0, 1, 2, 3 };
		readonly List<int> deadends = new List<int>();
		readonly List<int> openSpaces = new List<int>();
		readonly int enemyCount, mushroomCount, potionCount;
		readonly (int value, int count)[] objects;

		private UnityEngine.Vector2[] playerGoalSpawnPoints;

		public IntMaze(int dimens, int enemyCount, int mushroomCount, int potionCount)
		{

			//make sure maze scale is an odd number
			if (dimens % 2 == 0)
				dimens += 1;

			rows = dimens;
			columns = dimens;

			this.enemyCount = enemyCount;
			this.mushroomCount = mushroomCount; 
			this.potionCount = potionCount;
			// make everything walls
			maze = new int[rows * columns];
			for (int i = 0; i < rows * columns; ++i)
            {
				maze[i] = wall;
			}


			if (dimens < 11)
            {
				// maze generator breaks if smaller than 11, so anything smaller will be an empty room
				CreatePassagelessRoom();
            }
            else
            {
				//get walls
				CreatePassages(1, 1);
				int center = (rows) / 2;
				CreateCenterRoom(center);
				Deadends();
				Braid();
			}

			SpawnEnemies(enemyCount);
			SpawnMushrooms(mushroomCount);
			SpawnPotions(potionCount);
			SpawnGoal();

		}

		


		int GetIndex(int x, int y) { return y * rows + x; }

		(int, int) GetIndex2DIndex(int index) { return (index % columns, index / rows); }

		bool ValidPosition(int x, int y)
		{
			if (x < 0 || x >= rows || y < 0 || y >= columns)
				return false;
			return true;
		}

		void CreatePassagelessRoom()
        {
            for (int i = 0; i < rows; i++)
            {
				for (int j = 0; j < columns; j++)
				{
					if (i == 0 || j == 0 || i == rows - 1 || j == columns - 1)
                    { maze[GetIndex(i, j)] = wall; }
					else
					{ maze[GetIndex(i, j)] = empty; }
				}
			}
        }

		void CreatePassages(int x, int y) // recursive function to generate maze
		{
			maze[GetIndex(x, y)] = empty;

			System.Random _random = new System.Random();

			for (int i = 0; i < 4; ++i)
			{
				int random = _random.Next(0, 3);
				int temp = dirs[random];
				dirs[random] = dirs[i];
				dirs[i] = temp;
			}

			for (int i = 0; i < 4; ++i)
			{
				int xDirection = 0, yDirection = 0;

				switch (dirs[i])
				{
					case 0:
						yDirection = -1;
						break;
					case 1:
						yDirection = 1;
						break;
					case 2:
						xDirection = 1;
						break;
					case 3:
						xDirection = -1;
						break;
				}

				int xPos = x + (xDirection << 1); //(x << 1) is equivelant to (x * 2^1)
				int yPos = y + (yDirection << 1); //(y << 1) is equivelant to (y * 2^1)

				if (ValidPosition(xPos, yPos))
					if (maze[GetIndex(xPos, yPos)] == wall)
					{
						maze[GetIndex(xPos - xDirection, yPos - yDirection)] = empty;
						CreatePassages(xPos, yPos);
					}
			}
		}

		void CreateCenterRoom(int center) // create an empty room in the middle of the maze
		{

			int[] middleRoom = new int[9] {
				GetIndex(center - 1, center - 1),
				GetIndex(center - 1, center    ),
				GetIndex(center - 1, center + 1),
				GetIndex(center    , center - 1),
				GetIndex(center    , center	   ),
				GetIndex(center    , center + 1),
				GetIndex(center + 1, center - 1),
				GetIndex(center + 1, center    ),
				GetIndex(center + 1, center + 1)
			};

            foreach (var tile in middleRoom)
            {
				maze[tile] = 0;
			}
		}

		void Deadends() // count deadends
		{
			for (int i = 0; i < rows; i++)
				for (int j = 0; j < columns; j++)
					if (i != 0 && j != 0 && i != rows - 1 && j != columns - 1)
					{

						int[] neighbours = new int[8] {
							GetIndex(i - 1, j - 1),
							GetIndex(i - 1, j),
							GetIndex(i - 1, j + 1),
							GetIndex(i ,    j - 1),
							GetIndex(i ,    j + 1),
							GetIndex(i + 1, j - 1),
							GetIndex(i + 1, j),
							GetIndex(i + 1, j + 1)
						};

						int nCount = 0;
						for (int n = 0; n < neighbours.Length; n++)
							if (maze[neighbours[n]] == 1)
								nCount++;

						if (nCount > 6)
							deadends.Add(GetIndex(i, j));
					}
		}

		void Braid() // delete deadends
		{
			foreach (int i in Enumerable.Range(0, deadends.Count))
			{
				List<int> neighbours = new List<int>();

				int[] nVals = new int[] { -rows, -1, +1, +rows };

				for (int j = 0; j < 4; j++)
				{
					int index = deadends[i] + nVals[j];
					var (x, y) = GetIndex2DIndex(index);
					if (maze[index] == 1 && x != 0 && x != rows - 1 && y != 0 && y != columns - 1)
						neighbours.Add(index);
				}


				foreach (int neighbour in Enumerable.Range(0, neighbours.Count))
					if (maze[neighbours[neighbour]] == 1 && neighbours[neighbour] > rows && !(neighbours[neighbour] % columns == 0))
					{
						maze[neighbours[neighbour]] = 0;
						break;
					}
			}
		}

		// Spawning Goal, Consumables and Enemies
		//----------------------------------------

		// empty = 0
		// wall = 1
		// enemy = 2
		// mushroom = 3
		// potions = 4
		// stairs = 5

		void SpawnEnemies(int enemyCount)
		{
			for (int i = 0; i < enemyCount; i++)
			{
				maze[RandomIndex()] = 2;
			}
		}

		void SpawnMushrooms(int foodCount)
		{
			for (int i = 0; i < foodCount; i++)
			{
				maze[RandomIndex()] = 3;
			}
		}

		void SpawnPotions(int potionCount)
		{
			for (int i = 0; i < potionCount; i++)
			{
				maze[RandomIndex()] = 4;
			}
		}

		void SpawnGoal()
		{
			maze[RandomIndex()] = 5;
		}

		// other functions
		//-----------------

		int[] GetNeighbours(int center)
        {
			int[] middleRoom = new int[9] {
				GetIndex(center - 1, center - 1),
				GetIndex(center - 1, center    ),
				GetIndex(center - 1, center + 1),
				GetIndex(center    , center - 1),
				GetIndex(center    , center    ),
				GetIndex(center    , center + 1),
				GetIndex(center + 1, center - 1),
				GetIndex(center + 1, center    ),
				GetIndex(center + 1, center + 1)
			};

			return middleRoom;
		}

		int RandomIndex()
        {

			int center = (rows) / 2;
			int[] middleRoom = GetNeighbours(center);

			int randomRow, randomColumn, randomIndex;
			bool valid = false;
			do
			{
				randomRow = new System.Random().Next(1, rows - 1);
				randomColumn = new System.Random().Next(1, columns - 1);
				randomIndex = GetIndex(randomRow, randomColumn);

				if (randomIndex != middleRoom[0] && randomIndex != middleRoom[1] &&
					randomIndex != middleRoom[2] && randomIndex != middleRoom[3] &&
					randomIndex != middleRoom[4] && randomIndex != middleRoom[5] &&
					randomIndex != middleRoom[6] && randomIndex != middleRoom[7] &&
					randomIndex != middleRoom[8]) // may not be index in center room
				{
					if (maze[randomIndex] == 0)
					{
						valid = true;
					}
				}

			} while (!valid);

			return randomIndex;
		}

		public int[,] Get2DMaze()
		{
			int[,] outputGrid = new int[rows, columns];

			for (int i = 0; i < rows; i++)
				for (int j = 0; j < columns; j++)
					outputGrid[i, j] = maze[GetIndex(i, j)];

			return outputGrid;
		}

	}
}
