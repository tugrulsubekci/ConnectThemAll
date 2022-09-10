using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;
using Unity.VisualScripting;

public class GameManager : MonoBehaviour
{
    private float _width = 5;
    private float _height = 5;
    private float scaleMultiplier = 1.1f;
    private int minSpawnValue = 2;
    private int maxSpawnValue = 64;
    private float shiftSpeed = 0.5f;

    [SerializeField] private Node nodePrefab;
    [SerializeField] private Block blockPrefab;
    [SerializeField] private SpriteRenderer boardPrefab;
    [SerializeField] private List<BlockType> types;
    [SerializeField] private Transform nodeParent;
    [SerializeField] private Transform blockParent;
    [SerializeField] private Transform lineParent;
    [SerializeField] private Line linePrefab;

    private List<Node> nodes = new List<Node>();
    private List<Block> blocks = new List<Block>();
    private List<BlockType> spawnableBlockTypes = new List<BlockType>();
    private List<Block> matchableBlocks = new List<Block>();
    private List<Block> matchedBlocks = new List<Block>();
    private List<Line> lines = new List<Line>();
    private List<Node> emptyNodes = new List<Node>();

    private GameState _state;
    private Camera MainCam => Camera.main;
    private Block closestBlock;
    private Block selectedBlock;

    private void Start()
    {
        ChangeState(GameState.GenerateGame);
    }

    private void ChangeState(GameState newState)
    {
        _state = newState;

        switch (newState)
        {
            case GameState.GenerateGame:
                GenerateGrid();
                break;
            case GameState.WaitingInput:
                break;
            case GameState.ShiftBlocks:
                Shift();
                break;
            case GameState.FillNodes:
                FillEmptyNodes();
                break;
            default:
                break;
        }
    }
    private void GenerateGrid()
    {
        for (int i = 0; i < _width; i++)
        {
            for (int j = 0; j < _height; j++)
            {
                var node = Instantiate(nodePrefab, new Vector2(i, j), Quaternion.identity, nodeParent);
                nodes.Add(node);
            }
        }

        var center = new Vector2(_width / 2 - 0.5f, _height / 2 - 0.5f);

        var board = Instantiate(boardPrefab, center, Quaternion.identity);
        board.size = new Vector2(_width, _height);

        FillAllNodes(FindAvailableNodes());

        ChangeState(GameState.WaitingInput);
    }

    private void FillAllNodes(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            SpawnRandomBlock(nodes[i]);
        }
    }

    private void SpawnRandomBlock(Node node)
    {
        var block = Instantiate(blockPrefab, node.Pos, Quaternion.identity, blockParent);
        block._trasform.localScale = Vector2.zero;
        blocks.Add(block);
        node.occupiedBlock = block;
        block.occupiedNode = node;
        block.Init(GetRandomBlockType(minSpawnValue, maxSpawnValue));
        spawnableBlockTypes.Clear();
        block._trasform.DOScale(Vector3.one * 0.8f, 0.2f);
    }

    private void SpawnSpecificBlock(Node node, BlockType blockType)
    {
        var block = Instantiate(blockPrefab, node.Pos, Quaternion.identity, blockParent);
        blocks.Add(block);
        node.occupiedBlock = block;
        block.occupiedNode = node;
        // nodes.Add(node);
        emptyNodes.Remove(node);
        block.Init(blockType);
        spawnableBlockTypes.Clear();
    }

    private BlockType GetRandomBlockType(int min, int max)
    {
        foreach (BlockType blockType in types)
        {
            if (blockType.value >= min && blockType.value <= max)
            {
                spawnableBlockTypes.Add(blockType);
            }
        }
        return spawnableBlockTypes[UnityEngine.Random.Range(0, spawnableBlockTypes.Count)];
    }

    private BlockType GetSpecificBlockTypeWithValue(int value)
    {
        foreach (BlockType blockType in types)
        {
            if (blockType.value == value)
            {
                return blockType;
            }
        }
        return types[0];
    }

    private int FindAvailableNodes()
    {
        var availableNodes = nodes.Where(n => n.occupiedBlock == null);
        return availableNodes.Count();
    }
    private void Update()
    {
        if (_state != GameState.WaitingInput) return;

        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = MainCam.ScreenToWorldPoint(Input.mousePosition);

            closestBlock = blocks.OrderBy(n => (n.Pos - mousePos).sqrMagnitude).First();

            if ((closestBlock.Pos - mousePos).sqrMagnitude < 0.2f)
            {
                matchedBlocks.Add(closestBlock);
                closestBlock.transform.localScale *= scaleMultiplier;
                FindMatchableBlocks(closestBlock);
            }
            else
            {
                closestBlock = null;
            }
        }
        else if (Input.GetMouseButton(0) && closestBlock != null)
        {
            Vector2 mousePos = MainCam.ScreenToWorldPoint(Input.mousePosition);

            selectedBlock = blocks.OrderBy(n => (n.Pos - mousePos).sqrMagnitude).First();

            if ((selectedBlock.Pos - mousePos).sqrMagnitude > 0.2f)
            {
                return;
            }

            if (!IsInTheLine(selectedBlock))
            {
                foreach (Block block in matchableBlocks)
                {
                    if (selectedBlock == block)
                    {
                        selectedBlock.transform.localScale *= scaleMultiplier;
                        matchedBlocks.Add(selectedBlock);

                        var line = Instantiate(linePrefab, Vector2.zero, Quaternion.identity, lineParent);
                        line.ChangeColor(closestBlock.color);
                        line.DrawLine(matchedBlocks[matchedBlocks.Count - 2].Pos, matchedBlocks[matchedBlocks.Count - 1].Pos);
                        lines.Add(line);

                        matchableBlocks.Clear();
                        FindMatchableBlocks(selectedBlock);
                        break;
                    }
                }
            }

            int matchedBlocksCount = matchedBlocks.Count;

            if (matchedBlocksCount >= 2 && selectedBlock == matchedBlocks[matchedBlocksCount - 2])
            {
                matchedBlocks[matchedBlocksCount - 1].transform.localScale /= scaleMultiplier;
                matchedBlocks.RemoveAt(matchedBlocksCount - 1);

                Line line = lines[lines.Count - 1];
                lines.Remove(line);
                Destroy(line.gameObject);

                matchableBlocks.Clear();
                FindMatchableBlocks(matchedBlocks[matchedBlocksCount - 2]);
            }
        }
        else if (Input.GetMouseButtonUp(0) && closestBlock != null)
        {
            if (matchedBlocks.Count >= 2)
            {
                foreach (Block block in matchedBlocks)
                {
                    emptyNodes.Add(block.occupiedNode);
                    // nodes.Remove(block.occupiedNode);
                    block.occupiedNode.occupiedBlock = null;
                    blocks.Remove(block);
                    block._spriteRenderer.sortingOrder = -1;
                    block.transform.DOScale(Vector3.zero, 0.25f);
                    block.transform.DOMove(matchedBlocks[matchedBlocks.Count - 1].Pos, 0.25f);
                    Destroy(block.gameObject,0.30f);
                }
                
                foreach (Line line in lines)
                {
                    Destroy(line.gameObject);
                }
                
                SpawnMatchBlock(matchedBlocks, matchedBlocks[matchedBlocks.Count - 1].occupiedNode);

                matchedBlocks.Clear();
                lines.Clear();

                ChangeState(GameState.ShiftBlocks);
            }
            else
            {
                closestBlock.transform.localScale /= scaleMultiplier;
            }

            matchedBlocks.Clear();
            closestBlock = null;
            selectedBlock = null;
        }
    }

    private List<Block> FindMatchableBlocks(Block block)
    {
        var aroundBlocks = blocks.FindAll(n => (n.Pos - block.Pos).sqrMagnitude < 3);
        aroundBlocks.Remove(block);

        foreach (Block aroundBlock in aroundBlocks)
        {
            if (closestBlock.value == aroundBlock.value && !IsInTheLine(aroundBlock))
            {
                matchableBlocks.Add(aroundBlock);
            }
        }
        return matchableBlocks;
    }

    private bool IsInTheLine(Block selectedBlock)
    {
        foreach (Block block in matchedBlocks)
        {
            if (selectedBlock == block)
            {
                return true;
            }
        }
        return false;
    }

    private void SpawnMatchBlock(List<Block> matchedBlocks, Node spawnNode)
    {
        int matchValue = 0;
        if (matchedBlocks.Count <= 3)
        {
            matchValue = matchedBlocks[0].value * 2;
        }
        else if (matchedBlocks.Count <= 7 && matchedBlocks.Count > 3)
        {
            matchValue = matchedBlocks[0].value * 4;
        }
        else if (matchedBlocks.Count <= 15 && matchedBlocks.Count > 7)
        {
            matchValue = matchedBlocks[0].value * 8;
        }

        SpawnSpecificBlock(spawnNode, GetSpecificBlockTypeWithValue(matchValue));
    }
    private void Shift()
    {
        nodes = nodes.OrderBy(b => b.Pos.x).ThenBy(b => b.Pos.y).ToList();
        foreach (Node emptyNode in emptyNodes)
        {
            foreach (Node node in nodes)
            {
                if (emptyNode.Pos + Vector2.up == node.Pos && node.occupiedBlock != null)
                {
                    node.occupiedBlock._trasform.DOLocalMoveY(node.Pos.y + Vector3.down.y, shiftSpeed);
                    node.MoveTo(Vector3.down);
                    emptyNode.MoveTo(Vector3.up);
                }
                else if (emptyNode.Pos + Vector2.up * 2 == node.Pos && node.occupiedBlock != null)
                {
                    node.occupiedBlock._trasform.DOLocalMoveY(node.Pos.y + Vector3.down.y * 2, shiftSpeed);
                    node.MoveTo(Vector3.down * 2);
                    emptyNode.MoveTo(Vector3.up * 2);
                }
                else if (emptyNode.Pos + Vector2.up * 3 == node.Pos && node.occupiedBlock != null)
                {
                    node.occupiedBlock._trasform.DOLocalMoveY(node.Pos.y + Vector3.down.y * 3, shiftSpeed);
                    node.MoveTo(Vector3.down * 3);
                    emptyNode.MoveTo(Vector3.up * 3);
                }
            }
        }

        Invoke(nameof(ChangeStateToFillNodes), shiftSpeed);
    }
    private void ChangeStateToFillNodes()
    {
        ChangeState(GameState.FillNodes);
    }

    private void FillEmptyNodes()
    {
        foreach (Node emptyNode in emptyNodes)
        {
            SpawnRandomBlock(emptyNode);
        }
        emptyNodes.Clear();
        ChangeState(GameState.WaitingInput);
    }
}

[Serializable]
public struct BlockType
{
    public int value;
    public Color color;
}

public enum GameState
{
    GenerateGame,
    WaitingInput,
    ShiftBlocks,
    FillNodes,
}