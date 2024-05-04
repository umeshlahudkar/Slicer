using UnityEngine;

/// <summary>
/// This class generates a grid of blocks based on specified parameters and populates it with Block instances.
/// </summary>
public class GridGenerator : MonoBehaviour
{
    [SerializeField] private Block blockPrefab;    // Prefab used to instantiate blocks in the grid
    [SerializeField] private int numRows;           // Number of rows in the grid
    [SerializeField] private int numColumns;        // Number of columns in the grid
    [SerializeField] private float blockSize = 2f; // Size of each block
    [SerializeField] private float spacing = 0.1f;  // Spacing between blocks

    /// <summary>
    /// Generates a grid of blocks based on the specified parameters and initializes the grid in the GameplayController.
    /// </summary>
    public void GenerateGrid(GameplayController gameplayController)
    {
        gameplayController.InitGrid(numRows, numColumns);

        Vector3 prefabPosition = blockPrefab.transform.position;
        float startX = prefabPosition.x;
        float startZ = prefabPosition.z;
        int blueRows = numRows / 2;  // Determine number of rows that will be blue-colored

        for (int row = 0; row < numRows; row++)
        {
            // Determine the block type for the current row (either Blue or None based on the row number)
            BlockType rowBlockType = (row < blueRows) ? BlockType.Blue : BlockType.None;

            // Determine the number of green and cyan columns for the current row
            int numGreenColumns = numColumns / 2;
            int numCyanColumns = numColumns - numGreenColumns;

            for (int col = 0; col < numColumns; col++)
            {
                // Determine the block type for the current column (either Green or Cyan based on the column number)
                BlockType colBlockType = (col < numGreenColumns) ? BlockType.Green : BlockType.Cyan;

                // Calculate the position for the new block based on row and column indices, blockSize, and spacing
                float x = startX + col * (blockSize + spacing);
                float z = startZ - row * (blockSize + spacing);
                Vector3 blockPos = new Vector3(x, prefabPosition.y, z);

                Block newBlock = Instantiate(blockPrefab, blockPos, Quaternion.identity, transform);
                newBlock.ThisTransform.localScale = new Vector3(blockSize, blockSize, blockSize);
                // Determine the final block type for the current block (priority: Blue > Green > Cyan)
                BlockType type = (rowBlockType == BlockType.Blue) ? BlockType.Blue : colBlockType;
                newBlock.SetBlock(type, row, col);
                newBlock.gameObject.SetActive(true);

                gameplayController.grid[row, col] = newBlock;
            }
        }
    }
}