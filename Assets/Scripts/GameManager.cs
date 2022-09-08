using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private float _width = 5;
    private float _height = 5;
    [SerializeField] private Node nodePrefab;
    [SerializeField] private Block blockPrefab;
    [SerializeField] private SpriteRenderer boardPrefab;
    [SerializeField] private List<BlockType> types;

    private List<Node> nodes = new List<Node>();
    private List<Block> blocks = new List<Block>();
    private void Start()
    {
        GenerateGrid();
        SpawnBlocks(FindAvailableNodes());
    }

    private void GenerateGrid()
    {
        for (int i = 0; i < _width; i++)
        {
            for(int j = 0; j < _height; j++)
            {
                var node = Instantiate(nodePrefab,new Vector2(i,j),Quaternion.identity);
                nodes.Add(node);
            }
        }

        var center = new Vector2(_width /2 -0.5f, _height /2 - 0.5f);

        var board = Instantiate(boardPrefab, center, Quaternion.identity);
        board.size = new Vector2(_width, _height);
    }

    private void SpawnBlocks(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            var block = Instantiate(blockPrefab, nodes[i].Pos,Quaternion.identity);
            blocks.Add(block);
            nodes[i].occupiedBlock = block;
        }
    }

    private int FindAvailableNodes()
    {
        var availableNodes = nodes.Where(n => n.occupiedBlock == null); // OrderBy(x => Random.value)
        return availableNodes.Count();
    }

    [Serializable] 
    public struct BlockType
    {
        public int value;
        public Color color;
    }
}
