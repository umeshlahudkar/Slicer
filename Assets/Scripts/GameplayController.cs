using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Controls the main gameplay logic, including managing the game grid and handling block clearing and movement.
/// </summary>
public class GameplayController : MonoBehaviour
{
    private int rows;                    // Number of rows in the grid
    private int columns;                 // Number of columns in the grid
    public Block[,] grid;               // 2D array representing the game grid containing blocks

    [SerializeField] private GridGenerator gridGenerator;    // Reference to the grid generator component
    [SerializeField] private float clearedBlockMoveSpeed;     // Speed at which cleared blocks move down

    private List<Block> clearedBlock = new List<Block>();     // List of cleared blocks awaiting removal

    public static GameplayController Instance;                 // Singleton instance of the gameplay controller

    private void Awake()
    {
        Instance = this;  
    }

    private void Start()
    {
        gridGenerator.GenerateGrid(this);   
    }

    private void Update()
    {
        MoveClearedBlock();  
    }

    /// <summary>
    /// Initializes the game grid with the specified number of rows and columns.
    /// </summary>
    public void InitGrid(int row, int col)
    {
        rows = row;
        columns = col;
        grid = new Block[rows, columns]; 
    }

    /// <summary>
    /// Clears a block and all blocks below it in the same column.
    /// </summary>
    public void ClearBlock(Block block)
    {
        int col = block.Col_ID;
        int row = block.Row_ID;

        for (int i = row; i < rows; i++)
        {
            if (grid[i, col] != null)
            {
                grid[i, col].ResetBlock();
                clearedBlock.Add(grid[i, col]);
                grid[i, col] = null;
            }
        }
    }

    /// <summary>
    /// Moves cleared blocks downwards until they are removed from the game.
    /// </summary>
    private void MoveClearedBlock()
    {
        if (clearedBlock.Count > 0)
        {
            for (int i = 0; i < clearedBlock.Count; i++)
            {
                clearedBlock[i].ThisTransform.Translate(clearedBlockMoveSpeed * Time.deltaTime * Vector3.back);

                // Check if the cleared block has moved out of view and needs to be destroyed
                if (clearedBlock[i].ThisTransform.position.z < -100f)
                {
                    Destroy(clearedBlock[i].gameObject);
                    clearedBlock.RemoveAt(i);
                    i--;
                }
            }
        }
    }
}
