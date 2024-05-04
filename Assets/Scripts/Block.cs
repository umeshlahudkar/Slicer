using UnityEngine;

/// <summary>
/// Controls the behavior of a block in the game.
/// </summary>
public class Block : MonoBehaviour
{
    private BlockType blockType;          // Type of the block (determines its color)
    private int row_ID;                   // Row index of the block in the game grid
    private int col_ID;                   // Column index of the block in the game grid

    [SerializeField] private Collider thisCollider;   // Reference to the block's collider component
    [SerializeField] private Renderer thisRenderer;   // Reference to the block's renderer component
    [SerializeField] private Transform thisTransform; // Reference to the block's transform component

    /// <summary>
    /// Sets the block's type, row index, and column index.
    /// </summary>
    public void SetBlock(BlockType blockType, int row, int col)
    {
        this.blockType = blockType;
        row_ID = row;
        col_ID = col;
        AssignColor();
    }

    /// <summary>
    /// Resets the block's state to default (inactive).
    /// </summary>
    public void ResetBlock()
    {
        thisCollider.enabled = false; 
        blockType = BlockType.None;    
        row_ID = -1;                   
        col_ID = -1;                  
    }

    /// <summary>
    /// Assigns the block's color based on its type.
    /// </summary>
    private void AssignColor()
    {
        thisRenderer.material.color = GetBlockColor(); 
    }

    public BlockType BlockType { get { return blockType; } }

    public int Row_ID { get { return row_ID; } }

    public int Col_ID { get { return col_ID; } }

    public Transform ThisTransform { get { return thisTransform; } }

    /// <summary>
    ///  Returns the color associated with the block's type.
    /// </summary>
    public Color GetBlockColor()
    {
        switch (blockType)
        {
            case BlockType.Green:
                return Color.green;

            case BlockType.Blue:
                return Color.blue;

            case BlockType.Cyan:
                return Color.cyan;
        }

        return Color.white;
    }
}