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
    private float _height = 6;
    private float scaleMultiplier = 1.1f;
    private float shiftSpeed = 0.5f;
    private int tutorialNumber = 1;
    public float _time;
    private float _passedTime;

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
    [SerializeField] private GameObject gamePlayButtons;
    [SerializeField] private ParticleSystem scoreMultiplierParticle;
    [SerializeField] private GameObject noMore;
    [SerializeField] private GameObject sortPopup;
    [SerializeField] private GameObject multiplierPopup;
    [SerializeField] private GameObject _doublePopup;
    [SerializeField] private GameObject spawnValuePopup;
    [SerializeField] private GameObject levelPopup;
    [SerializeField] private InterstitialAdvertisement InterstitialAd;

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
        UpdateGame();
        GenerateGrid();
        SpawnShowBlock();
        if (!DataManager.Instance.tutorial)
        {
            ChangeState(GameState.Tutorial);
        }
        else
        {
            if (DataManager.Instance.values == null)
            {
                ChangeState(GameState.GenerateGame);
            }
            else
            {
                ChangeState(GameState.LoadGame);
            }
        }
        
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
            case GameState.LoadGame:
                LoadGame();
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
                var node = Instantiate(nodePrefab, new Vector2(i, j - 1), Quaternion.identity, nodeParent);
                nodes.Add(node);
            }
        }
        var center = new Vector2(_width / 2 - 0.5f, _height / 2 - 1.5f);

        var board = Instantiate(boardPrefab, center, Quaternion.identity);
        board.size = new Vector2(_width, _height);
    }
    private void StartFirstTutorial()
    {
        tutorialNodes = nodes.FindAll(n => n.Pos.y == -1);

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

        tutorialNodes = nodes.FindAll(n => n.Pos.y == -1 || n.Pos.y == 0);

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
    private void StartThirdTutorial()
    {
        ResetWorld();

        tutorialNodes.Clear();

        tutorialNodes = nodes.FindAll(n => n.Pos.y == -1 || n.Pos.y == 0);

        Invoke(nameof(SpawnThirdTutorialBlocks), 1.25f);
    }
    private void SpawnThirdTutorialBlocks()
    {
        SpawnSpecificBlock(tutorialNodes[0], GetSpecificBlockTypeWithValue(64));
        SpawnSpecificBlock(tutorialNodes[1], GetSpecificBlockTypeWithValue(2));
        SpawnSpecificBlock(tutorialNodes[2], GetSpecificBlockTypeWithValue(32));
        SpawnSpecificBlock(tutorialNodes[3], GetSpecificBlockTypeWithValue(2));
        SpawnSpecificBlock(tutorialNodes[4], GetSpecificBlockTypeWithValue(32));
        SpawnSpecificBlock(tutorialNodes[5], GetSpecificBlockTypeWithValue(2));
        SpawnSpecificBlock(tutorialNodes[6], GetSpecificBlockTypeWithValue(8));
        SpawnSpecificBlock(tutorialNodes[7], GetSpecificBlockTypeWithValue(2));
        SpawnSpecificBlock(tutorialNodes[8], GetSpecificBlockTypeWithValue(8));
        SpawnSpecificBlock(tutorialNodes[9], GetSpecificBlockTypeWithValue(8));

        tap.PlayTapAnimation(TutorialNumber.Third);

        ChangeState(GameState.WaitingInput);
    }

    private void ResetThirdTutorial()
    {
        ResetWorld();
        Invoke(nameof(SpawnThirdTutorialBlocks), 1.25f);
    }

    private void GenerateGame()
    {
        FillAllNodes(FindAvailableNodes());
        ChangeState(GameState.WaitingInput);
    }

    private void LoadGame()
    {
        for (int i = 0; i < nodes.Count; i++)
        {
            SpawnSpecificBlock(nodes[i], GetSpecificBlockTypeWithValue(DataManager.Instance.values[i]));
        }
    }

    private void ChangeGameStateToGenerateGame()
    {
        ChangeState(GameState.GenerateGame);
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
        block.Init(GetRandomBlockType(DataManager.Instance.minSpawnValue, DataManager.Instance.maxSpawnValue));
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
        Vector3 showBlockPos = new Vector3(2,gamePlayButtons.transform.GetChild(0).position.y);
        showBlock = Instantiate(blockPrefab, showBlockPos, Quaternion.identity, blockParent);
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

                Vibrator.Vibrate(10);
            }
            else
            {
                closestBlock = null;
            }

            if(!DataManager.Instance.tutorial)
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

                    Vibrator.Vibrate(10);
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
                        line.ChangeColor(matchedBlocks[matchedBlocks.Count - 2].color,selectedBlock.color);
                        line.DrawLine(matchedBlocks[matchedBlocks.Count - 2].Pos, matchedBlocks[matchedBlocks.Count - 1].Pos);
                        lines.Add(line);

                        matchableBlocks.Clear();
                        FindMatchableBlocks(selectedBlock);

                        Vibrator.Vibrate(10);
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
            if (matchedBlocks.Count >= 2 && DataManager.Instance.tutorial)
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
            else if(matchedBlocks.Count >= 2 && !DataManager.Instance.tutorial)
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

                if (tutorialNumber == 1)
                {
                    if (matchedBlocks.Count == 5)
                    {
                        tutorialNumber++;
                        StartSecondTutorial();
                    }
                    else if (matchedBlocks.Count < 5)
                    {
                        ResetFirstTutorial();
                    }
                }
                else if(tutorialNumber == 2)
                {
                    if (matchedBlocks.Count == 5)
                    {
                        tutorialNumber++;
                        StartThirdTutorial();
                    }
                    else if (matchedBlocks.Count < 5)
                    {
                        ResetSecondTutorial();
                    }
                }
                else if(tutorialNumber == 3)
                {
                    if (matchedBlocks.Count == 10)
                    {
                        DataManager.Instance.tutorial = true;
                        gamePlayButtons.SetActive(true);
                        tutorialNodes.Clear();
                        ResetWorld();
                        Invoke(nameof(ChangeGameStateToGenerateGame), 1f);
                    }
                    else if (matchedBlocks.Count < 10)
                    {
                        ResetThirdTutorial();
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
            if ((block.value == aroundBlock.value && !IsInTheLine(aroundBlock) )|| (CalculateMatchValue(matchedBlocks) == aroundBlock.value && !IsInTheLine(aroundBlock)))
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
        if(DataManager.Instance.tutorial)
        {
            UpdateSlider();
            AddScore(matchValue);
        }
    }

    private void AddScore(int value)
    {
        DataManager.Instance.highScore += value * DataManager.Instance.scoreMultiplier;
        UpdateScoreText();
    }
    private void UpdateScoreText()
    {
        scoreText.text = IntToString(DataManager.Instance.highScore);
    }
    private string IntToString(int value)
    {
        if (value < 1000)
        {
            return value.ToString();
        }
        else if (value < 1000000)
        {
            return $"{MathF.Round((float)value / 1000, 1)}K";
        }
        else if (value < 1000000000)
        {
            return $"{MathF.Round((float)value / 1000000, 1)}M";
        }
        return "Infinity";
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
        DataManager.Instance.currentLevel++;
        currentLevelText.text = DataManager.Instance.currentLevel.ToString();
        nextLevelText.text = (DataManager.Instance.currentLevel + 1).ToString();
        slider.value = slider.minValue;
        InterstitialAd.ShowAd();
        CheckLevel();
    }

    private void CheckLevel()
    {
        if (DataManager.Instance.currentLevel == 3)
        {
            sortPopup.SetActive(true);
            ActivateButton(0);
        }
        else if (DataManager.Instance.currentLevel == 5)
        {
            spawnValuePopup.SetActive(true);
            ActivateButton(1);
        }
        else if (DataManager.Instance.currentLevel == 7)
        {
            _doublePopup.SetActive(true);
            ActivateButton(2);
        }
        else if (DataManager.Instance.currentLevel == 9)
        {
            multiplierPopup.SetActive(true);
            ActivateButton(3);
        }
        else
        {
            levelPopup.SetActive(true);
        }
    }

    private void UpdateLevelSlider()
    {
        currentLevelText.text = DataManager.Instance.currentLevel.ToString();
        nextLevelText.text = (DataManager.Instance.currentLevel + 1).ToString();
        slider.value = DataManager.Instance.sliderValue;
    }

    private int CalculateMatchValue(List<Block> matchedBlocks)
    {
        int matchValue = 0;
        if (matchedBlocks.Count < 2) return matchedBlocks[0].value;

        foreach (Block block in matchedBlocks)
        {
            matchValue += block.value;
        }

        List<int> valueList = new List<int>();
        foreach (BlockType blockType in types)
        {
            valueList.Add(blockType.value);
            if (blockType.value == matchValue)
            {
                return matchValue;
            }
        }

        valueList.Add((int)matchValue);

        valueList = valueList.OrderBy(n => n).ToList();
        int matchValueIndex = valueList.FindIndex(n => n == matchValue);

        return valueList[matchValueIndex + 1];
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
        Instantiate(scoreMultiplierParticle,gamePlayButtons.transform.GetChild(3).position,Quaternion.identity);
        DataManager.Instance.scoreMultiplier++;
        UpdateScoreMultiplierText();
    }
    private void UpdateScoreMultiplierText()
    {
        scoreMultiplierText.text = $"x{DataManager.Instance.scoreMultiplier}";
    }

    public void DoubleAllBlocks()
    {
        foreach (Block block in blocks)
        {
            block._trasform.GetChild(1).gameObject.SetActive(false);
            block._trasform.DORotate(new Vector3(0,360,0), 1, RotateMode.FastBeyond360).OnComplete(() =>
            {
                block.Init(GetSpecificBlockTypeWithValue(block.value * 2));
                block._trasform.GetChild(1).gameObject.SetActive(true);
            });
        }
    }
    public void IncreaseMinMaxSpawnValue()
    {
        noMore.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = $"No more {GetSpecificBlockTypeWithValue(DataManager.Instance.minSpawnValue).valueString}'s";
        noMore.SetActive(true);

        DataManager.Instance.minSpawnValue *= 2;
        DataManager.Instance.maxSpawnValue *= 2;
    }
    private void UpdateGame()
    {
        DataManager.Instance.Load();
        UpdateScoreText();
        UpdateScoreMultiplierText();
        UpdateLevelSlider();
        UpdateGameplayButtons();
    }

    private void UpdateGameplayButtons()
    {
        if (DataManager.Instance.currentLevel >= 3)
        {
            Destroy(sortPopup);
            ActivateButton(0);
        }
        if (DataManager.Instance.currentLevel >= 5)
        {
            Destroy(spawnValuePopup);
            ActivateButton(1);
        }
        if (DataManager.Instance.currentLevel >= 7)
        {
            Destroy(_doublePopup);
            ActivateButton(2);
        }
        if (DataManager.Instance.currentLevel >= 9)
        {
            Destroy(_doublePopup);
            ActivateButton(3);
        }
    }

    private void ActivateButton(int childIndex)
    {
        GameObject button = gamePlayButtons.transform.GetChild(childIndex).gameObject;
        button.transform.GetChild(0).gameObject.SetActive(false);
        button.GetComponent<Image>().enabled = true;
        button.GetComponent<Button>().interactable = true;
    }

    private void SaveBlockValues()
    {
        if(DataManager.Instance.tutorial)
        {
            var orderedBlocks = blocks.OrderBy(a => a.Pos.x).ThenBy(a => a.Pos.y).ToList();
            for (int i = 0; i < orderedBlocks.Count; i++)
            {
                DataManager.Instance.values[i] = orderedBlocks[i].value;
            }
            DataManager.Instance.sliderValue = (int)slider.value;
            DataManager.Instance.Save();
        }
    }
    private void OnApplicationPause(bool pause)
    {
        if(pause)
        {
            SaveBlockValues();
        }
    }
    private void OnApplicationFocus(bool focus)
    {
        if(!focus)
        {
            SaveBlockValues();
        }

    }
    private void OnApplicationQuit()
    {
        SaveBlockValues();
    }
}

[Serializable]
public struct BlockType
{
    public int value;
    public Color color;
    public string valueString;
    public bool flat;
}

public enum GameState
{
    Tutorial,
    GenerateGame,
    WaitingInput,
    ShiftBlocks,
    FillNodes,
    MainMenu,
    LoadGame
}
public enum MatchType
{
    Normal,
    Complex
}