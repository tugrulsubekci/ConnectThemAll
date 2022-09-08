using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private float _width = 5;
    private float _height = 5;
    private int minSpawnValue = 2;
    private int maxSpawnValue = 64;
    [SerializeField] private Node nodePrefab;
    [SerializeField] private Block blockPrefab;
    [SerializeField] private SpriteRenderer boardPrefab;
    [SerializeField] private List<TypeOfBlock> types;

    private List<Node> nodes = new List<Node>();
    private List<Block> blocks = new List<Block>();
    private List<TypeOfBlock> spawnableBlockTypes = new List<TypeOfBlock>();
    private void Start()
    {
        GenerateGrid();
        SpawnBlocks(FindAvailableNodes());
    }

    private void GenerateGrid()
    {
        for (int i = 0; i < _width; i++)
        {
            for (int j = 0; j < _height; j++)
            {
                var node = Instantiate(nodePrefab, new Vector2(i, j), Quaternion.identity);
                nodes.Add(node);
            }
        }

        var center = new Vector2(_width / 2 - 0.5f, _height / 2 - 0.5f);

        var board = Instantiate(boardPrefab, center, Quaternion.identity);
        board.size = new Vector2(_width, _height);
    }

    private void SpawnBlocks(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            var block = Instantiate(blockPrefab, nodes[i].Pos, Quaternion.identity);
            blocks.Add(block);
            nodes[i].occupiedBlock = block;
            block.Init(GetRandomBlockType(minSpawnValue,maxSpawnValue));
        }
    }

    private TypeOfBlock GetRandomBlockType(int min, int max)
    {
        foreach (TypeOfBlock blockType in types)
        {
            if (blockType.value >= min && blockType.value <= max)
            {
                spawnableBlockTypes.Add(blockType);
            }
        }
        return spawnableBlockTypes[UnityEngine.Random.Range(0, spawnableBlockTypes.Count)];
    }

    private int FindAvailableNodes()
    {
        var availableNodes = nodes.Where(n => n.occupiedBlock == null); // OrderBy(x => Random.value)
        return availableNodes.Count();
    }
}

[Serializable]
public struct TypeOfBlock
{
    public int value;
    public Color color;
}
