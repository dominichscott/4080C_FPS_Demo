using System;
using System.Collections.Generic;
using IT4080C;
using TreeEditor;
using Unity.Entities;
using Unity.NetCode;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class ScoreBoardUIManager : MonoBehaviour
{
    
    public UIDocument uiDocument;
    private MultiColumnListView scoreBoard;
    private List<PlayerScore> playerScores = new List<PlayerScore>();

    private EntityManager entityManager;
    private EntityQuery ghostQuery;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // get Entity Manager
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        // UI Stuff
        var root = uiDocument.rootVisualElement;
        scoreBoard = root.Q<MultiColumnListView>("ScoreboardMultiColListView");
        scoreBoard.columns.Add(new Column()
        {
            title = "PlayerName",
            makeCell = MakeCellLabel,
            bindCell = BindNameToCell,
            stretchable = true,
        });
        scoreBoard.columns.Add(new Column()
        {
            title = "Kills",
            makeCell = MakeCellLabel,
            bindCell = BindKillsToCell,
            stretchable = true,
        });
        scoreBoard.columns.Add(new Column()
        {
            title = "Deaths",
            makeCell = MakeCellLabel,
            bindCell = BindDeathsToCell,
            stretchable = true,
        });
        BuildMockUpDisplay();
        scoreBoard.Rebuild();
        
        ghostQuery =entityManager.CreateEntityQuery(typeof(PlayerScore));
    }
    private void BuildMockUpDisplay()
    {
        var persons = new List<PlayerScore>()
            {
                new PlayerScore("John", 20,2),
                new PlayerScore("Jane", 23,2),
            };
        ApplyPersons(persons);
    }
    public void ApplyPersons(List<PlayerScore> scores)
    {
        scoreBoard.itemsSource = scores;
    }
    // Update is called once per frame
    void Update()
    {
        // Check if we have any Ghost entities
        if (ghostQuery == null)
        {
            ghostQuery = entityManager.CreateEntityQuery(typeof(PlayerScore));
        }
        var entities = ghostQuery.ToEntityArray(Unity.Collections.Allocator.Temp);

        if (entities.Length > 0)
        {
            foreach (Entity ent in entities)
            {
                var healthComponent = entityManager.GetComponentData<HealthComponent>(ent);
                var scores = new List<PlayerScore>()
                {
                    new PlayerScore(""+healthComponent.ownerNetworkID, (int)healthComponent.deaths,(int)healthComponent.kills),
                };
                ApplyPersons(scores);
            }
        }
        else
        {
            // Debug.LogWarning("No Entities from query");
        }
        entities.Dispose();
    }

    private Label MakeCellLabel() => new();

    private void BindNameToCell(VisualElement element, int index)
    {
        var label = (Label)element;
        var person = (PlayerScore)scoreBoard.viewController.GetItemForIndex(index);
        label.text = person.playerName;
    }

    private void BindDeathsToCell(VisualElement element, int index)
    {
        var label = (Label)element;
        var playerScore = (PlayerScore)scoreBoard.viewController.GetItemForIndex(index);
        label.text = ""+playerScore.deaths;
    }
    
    private void BindKillsToCell(VisualElement element, int index)
    {
        var label = (Label)element;
        var playerScore = (PlayerScore)scoreBoard.viewController.GetItemForIndex(index);
        label.text = ""+playerScore.kills;
    }
}
public class PlayerScore
{
    public string playerName;
    public int kills;
    public int deaths;

    public PlayerScore(string pName, int kill, int death)
    {
        playerName = pName;
        kills = kill;
        deaths = death;
    }
}
