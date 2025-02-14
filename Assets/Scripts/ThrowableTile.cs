using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "ThrowableTile", menuName = "Tiles/ThrowableTile")]
public class ThrowableTile : Tile
{
    public string tileName;
    public int durability;
    public int mass;
    public int damage;
}
