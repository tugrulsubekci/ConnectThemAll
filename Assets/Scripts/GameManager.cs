using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;
using Random = UnityEngine.Random;
using TMPro;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private float _width = 5;
    private float _height = 5;
    private float scaleMultiplier = 1.1f;
    private int minSpawnValue = 2;
    private int maxSpawnValue = 64;
    private float shiftSpeed = 0.5f;
    private bool isTutorialCompleted;
    private bool isFirstTutorialCompleted;
    private int scoreMultiplier = 1;
    private float score = 0;
    public float _time;
    private float _passedTime;
    private int currentLevel = 1;

    [SerializeField] private Slider slider;
    [SerializeField] private TextMeshProUGUI currentLevelText;
    [SerializeField] private TextMeshProUGUI nextLevelText;
    [SerializeField] private Node nodePrefab;
    [SerializeField] private Block blockPrefab;
    [SerializeField] private SpriteRenderer boardPrefab;
    [SerializeField] private List<BlockType> types;
    [SerializeField] private Transform nodeParent;
    [SerializeField] private Transform blockParent;
    [SerializeField] private Transform lineParent;
    [SerializeField] private Line linePrefab;
    [SerializeField] private Tap tapPrefab;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI scoreMultiplierText;

    private List<Node> nodes = new List<Node>();
    private List<Block> blocks = new List<Block>();
    private List<BlockType> spawnableBlockTypes = new List<BlockType>();
    private List<Block> matchableBlocks = new List<Block>();
    private List<Block> matchedBlocks = new List<Block>();
    private List<Line> lines = new List<Line>();
    private List<Node> emptyNodes = new List<Node>();
    private List<Node> tutorialNodes = new List<Node>();

    private Camera MainCam => Camera.main;
    private Block closestBlock;
    private Block selectedBlock;
    private Block showBlock;
    private Tap tap;
    private GameState _state;
    private Vector2 firstMousePos;
    private Vector2 mousePos;

    private void Start()
    {
        SpawnShowBlock();
        ChangeState(GameState.Tutorial);
    }

    public void ChangeState(GameState newState)
    {
        _state = newState;

        switch (newState)
        {
            case GameState.MainMenu:
                break;
            case GameState.Tutorial:
                StartFirstTutorial();
                ChangeState(GameState.MainMenu);
                break;
            case GameState.GenerateGame:
                GenerateGame();
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
    private void StartFirstTutorial()
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

        tutorialNodes = nodes.FindAll(n => n.Pos.y == 0);

        tap = Instantiate(tapPrefab, tapPrefab.startPos, Quaternion.identity);

        SpawnFirstTutorialBlocks();

        
    }
    private void ResetFirstTutorial()
    {
        ResetWorld();
        Invoke(nameof(SpawnFirstTutorialBlocks), 1.25f);
    }

    private void ResetWorld()
    {
        foreach (Block block in blocks)
        {
            block.occupiedNode.occupiedBlock = null;
            Destroy(block.gameObject, 0.75f);
        }
        blocks.Clear();
        emptyNodes.Clear();
    }
    private void SpawnFirstTutorialBlocks()
    {
        tap.PlayTapAnimation(TutorialNumber.First);

        foreach (Node tutorialNode in tutorialNodes)
        {
            SpawnSpecificBlock(tutorialNode, GetSpecificBlockTypeWithValue(2));
        }

        ChangeState(GameState.WaitingInput);
    }
    private void StartSecondTutorial()
    {
        ResetWorld();

        tutorialNodes.Clear();

        tutorialNodes = nodes.FindAll(n => n.Pos.y == 0 || n.Pos.y == 1);

        Invoke(nameof(SpawnSecondTutorialBlocks), 1.25f);
    }

    private void SpawnSecondTutorialBlocks()
    {
        SpawnSpecificBlock(tutorialNodes[0], GetSpecificBlockTypeWithValue(4));
        SpawnSpecificBlock(tutorialNodes[3], GetSpecificBlockTypeWithValue(4));
        SpawnSpecificBlock(tutorialNodes[4], GetSpecificBlockTypeWithValue(4));
        SpawnSpecificBlock(tutorialNodes[7], GetSpecificBlockTypeWithValue(4));
        SpawnSpecificBlock(tutorialNodes[8], GetSpecificBlockTypeWithValue(4));
        SpawnSpecificBlock(tutorialNodes[1], GetSpecificBlockTypeWithValue(64));
        SpawnSpecificBlock(tutorialNodes[2], GetSpecificBlockTypeWithValue(32));
        SpawnSpecificBlock(tutorialNodes[5], GetSpecificBlockTypeWithValue(64));
        SpawnSpecificBlock(tutorialNodes[6], GetSpecificBlockTypeWithValue(32));
        SpawnSpecificBlock(tutorialNodes[9], GetSpecificBlockTypeWithValue(64));

        tap.PlayTapAnimation(TutorialNumber.Second);

        ChangeState(GameState.WaitingInput);
    }
    private void ResetSecondTutorial()
    {
        ResetWorld();
        Invoke(nameof(SpawnSecondTutorialBlocks), 1.25f);
    }
    private void GenerateGame()
    {
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
        block._trasform.localScale = Vector2.zero;
        blocks.Add(block);
        node.occupiedBlock = block;
        block.occupiedNode = node;
        block.Init(blockType);
        spawnableBlockTypes.Clear();
        block._trasform.DOScale(Vector3.one * 0.8f, 0.2f);
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
        return spawnableBlockTypes[Random.Range(0, spawnableBlockTypes.Count)];
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

    private void SpawnShowBlock()
    {
        showBlock = Instantiate(blockPrefab, new Vector3(2, 5, 0), Quaternion.identity, blockParent);
        showBlock._trasform.localScale = Vector3.one * 0.65f;
        showBlock.gameObject.SetActive(false);
    }

    private void ActivateShowBlock()
    {
        showBlock.Init(GetSpecificBlockTypeWithValue(CalculateMatchValue(matchedBlocks)));
        showBlock.gameObject.SetActive(true);
    }
    private void DeactivateShowBlock()
    {
        showBlock.gameObject.SetActive(false);
    }
    private void Update()
    {
        if (_state != GameState.WaitingInput) return;

        if (Input.GetMouseButtonDown(0) && blocks.Count > 0)
        {
            firstMousePos = MainCam.ScreenToWorldPoint(Input.mousePosition);

            closestBlock = blocks.OrderBy(n => (n.Pos - firstMousePos).sqrMagnitude).First();

            if ((closestBlock.Pos - firstMousePos).sqrMagnitude < 0.2f)
            {
                matchedBlocks.Add(closestBlock);
                closestBlock.transform.localScale *= scaleMultiplier;
                FindMatchableBlocks(closestBlock);
                ActivateShowBlock();
            }
            else
            {
                closestBlock = null;
            }

            if(!isTutorialCompleted)
            {
                tap.StopTapAnimation();
            }
        }
        else if (Input.GetMouseButton(0) && blocks.Count > 0)
        {
            if (closestBlock == null)
            {
                firstMousePos = MainCam.ScreenToWorldPoint(Input.mousePosition);

                closestBlock = blocks.OrderBy(n => (n.Pos - firstMousePos).sqrMagnitude).First();

                if ((closestBlock.Pos - firstMousePos).sqrMagnitude < 0.2f)
                {
                    matchedBlocks.Add(closestBlock);
                    closestBlock.transform.localScale *= scaleMultiplier;
                    FindMatchableBlocks(closestBlock);
                    ActivateShowBlock();
                }
                else
                {
                    closestBlock = null;
                }
            }

            if (closestBlock == null) return;

            mousePos = MainCam.ScreenToWorldPoint(Input.mousePosition);

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
                        ActivateShowBlock();

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
                ActivateShowBlock();

                Line line = lines[lines.Count - 1];
                lines.Remove(line);
                Destroy(line.gameObject);

                matchableBlocks.Clear();
                FindMatchableBlocks(matchedBlocks[matchedBlocksCount - 2]);
            }
        }
        else if (Input.GetMouseButtonUp(0) && closestBlock != null && blocks.Count > 0)
        {
            if (matchedBlocks.Count >= 2 && isTutorialCompleted)
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
            else if(matchedBlocks.Count >= 2 && !isTutorialCompleted)
            {
                foreach (Block block in matchedBlocks)
                {
                    emptyNodes.Add(block.occupiedNode);
                    block.occupiedNode.occupiedBlock = null;
                    blocks.Remove(block);
                    block._spriteRenderer.sortingOrder = -1;
                    block.transform.DOScale(Vector3.zero, 0.25f);
                    block.transform.DOMove(matchedBlocks[matchedBlocks.Count - 1].Pos, 0.25f);
                    Destroy(block.gameObject, 0.30f);
                }

                foreach (Line line in lines)
                {
                    Destroy(line.gameObject);
                }

                SpawnMatchBlock(matchedBlocks, matchedBlocks[matchedBlocks.Count - 1].occupiedNode);

                if (!isFirstTutorialCompleted)
                {
                    if (matchedBlocks.Count == 5)
                    {
                        isFirstTutorialCompleted = true;
                        StartSecondTutorial();
                    }
                    else if (matchedBlocks.Count < 5)
                    {
                        ResetFirstTutorial();
                    }
                }
                else
                {
                    if (matchedBlocks.Count == 5)
                    {
                        isTutorialCompleted = true;
                        tutorialNodes.Clear();
                        ResetWorld();
                        Invoke(nameof(GenerateGame), 1f);
                    }
                    else if (matchedBlocks.Count < 5)
                    {
                        ResetSecondTutorial();
                    }
                }
                matchedBlocks.Clear();
                lines.Clear();
            }
            else
            {
                closestBlock.transform.localScale /= scaleMultiplier;
            }

            DeactivateShowBlock();
            matchedBlocks.Clear();
            matchableBlocks.Clear();
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
        int matchValue = CalculateMatchValue(matchedBlocks);
        SpawnSpecificBlock(spawnNode, GetSpecificBlockTypeWithValue(matchValue));
        emptyNodes.Remove(spawnNode);
        AddScore(matchValue);
        if(isTutorialCompleted)
        {
            UpdateSlider();
        }
    }

    private void AddScore(int value)
    {
        score += value * scoreMultiplier;
        if(score < 1000)
        {
            scoreText.text = score.ToString();
        }
        else if (score < 1000000)
        {
            scoreText.text = $"{MathF.Round(score / 1000,1)}K";
        }
    }

    private void UpdateSlider()
    {
        _passedTime = Time.time - _time;
        _time = Time.time;

        if (_passedTime > 4)
        {
            slider.value += 10;
        }
        else if(_passedTime <= 4)
        {
            slider.value += Mathf.RoundToInt(_passedTime * 2.5f);
        }

        if (slider.value == slider.maxValue)
        {
            LevelUp();
        }
    }

    private void LevelUp()
    {
        currentLevel++;
        currentLevelText.text = currentLevel.ToString();
        nextLevelText.text = (currentLevel + 1).ToString();
        slider.value = slider.minValue;
    }

    private int CalculateMatchValue(List<Block> matchedBlocks)
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

        return matchValue;
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
        if (CheckMatchableBlocks())
        {
            foreach (Node emptyNode in emptyNodes)
            {
                SpawnRandomBlock(emptyNode);
            }
            emptyNodes.Clear();
        }
        else
        {
            foreach (Node emptyNode in emptyNodes)
            {
                SpawnRandomMatchableBlock(emptyNode);
            }
            emptyNodes.Clear();
        }

        ChangeState(GameState.WaitingInput);
    }

    private bool CheckMatchableBlocks()
    {
        var possibleMatches = new List<Block>();
        foreach (Block block in blocks)
        {
            var aroundBlocks = blocks.FindAll(n => (n.Pos - block.Pos).sqrMagnitude < 3);
            aroundBlocks.Remove(block);

            foreach (Block aroundBlock in aroundBlocks)
            {
                if (block.value == aroundBlock.value)
                {
                    possibleMatches.Add(aroundBlock);
                }
                if (possibleMatches.Count > 0) return true;
            }
        }
        return false;
    }

    private void SpawnRandomMatchableBlock(Node emptyNode)
    {
        var randomMatchableBlock = blocks.FindAll(n => (n.Pos - emptyNode.Pos).sqrMagnitude < 3).OrderBy(n => Random.value).First();

        SpawnSpecificBlock(emptyNode, GetSpecificBlockTypeWithValue(randomMatchableBlock.value));
    }

    public void SortBlocks()
    {
        nodes = nodes.OrderBy(b => b.Pos.y).ThenBy(b => b.Pos.x).ToList();
        blocks = blocks.OrderBy(b => b.value).Reverse().ToList();

        for (int i = 0; i < blocks.Count; i++)
        {
            blocks[i].occupiedNode = null;
            nodes[i].occupiedBlock = null;

            blocks[i].occupiedNode = nodes[i];
            nodes[i].occupiedBlock = blocks[i];
            blocks[i]._trasform.DOMove(nodes[i].Pos, 1);
        }
    }

    public void IncreaseScoreMultiplier()
    {
        scoreMultiplier++;
        scoreMultiplierText.text = $"x{scoreMultiplier}";
    }

    public void DoubleAllBlocksButton()
    {
        foreach (Block block in blocks)
        {
            block._trasform.DORotate(new Vector3(360,0,0), 1, RotateMode.FastBeyond360).OnComplete(() =>
            {
                block.Init(GetSpecificBlockTypeWithValue(block.value * 2));
            });
            //block.Init(GetSpecificBlockTypeWithValue(block.value * 2));
        }
    }
}

[Serializable]
public struct BlockType
{
    public int value;
    public Color color;
    public string valueString;
}

public enum GameState
{
    Tutorial,
    GenerateGame,
    WaitingInput,
    ShiftBlocks,
    FillNodes,
    MainMenu,
}