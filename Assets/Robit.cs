using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Robit : NetworkBehaviour
{
    // private NetworkVariable<Vector3> netPosition;
    // private NetworkVariable<Quaternion> netRotation;
    private List<Card> cardsInHand;

    [SerializeField]
    private List<Card> cardsInDeck;
    private List<Card> function;

    private Queue<CardAction> actionQ;
    [SerializeField]
    private float actionDuration;
    private float actionTimeCounter;
    private CardAction currentAction;
    private Vector3 movementEndpoint;
    private Quaternion rotationEndpoint;

    [SerializeField]
    float moveSpeed;
    [SerializeField]
    float rotationSpeed;

    [SerializeField]
    public Color color;

    void Awake()
    {
        cardsInHand = new List<Card>();
        function = new List<Card>();
        actionQ = new Queue<CardAction>();
    }

    void Start()
    {
        MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer r in renderers)
            r.material.color = color;
        // netPosition.OnValueChanged += (oldVal, newVal) => transform.position = newVal;
        // netRotation.OnValueChanged += (oldVal, newVal) => transform.rotation = newVal;
    }

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            // Move();
            // transform.position = netPosition.Value;
            // transform.rotation = netRotation.Value;
        }
    }

    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(Screen.width - 310f, 10f, 300f, Screen.height - 20f));

        if (GUILayout.Button("Draw Card")) { DrawCard(); }
        if (GUILayout.Button("Play Card 0")) { PlayCard(0); }
        // if (GUILayout.Button("Play Card 1")) { PlayCard(1); }
        if (GUILayout.Button("Add Card 0 to func")) { AddCardToFunction(0); }
        // if (GUILayout.Button("Add Card 1 to func")) { AddCardToFunction(1); }
        if (GUILayout.Button("Discard Card 0")) { DiscardCard(0); }
        if (GUILayout.Button("Run function")) { RunFunction(); }

        GUILayout.Label("Hand:");
        foreach (Card card in cardsInHand)
            GUILayout.Label(card.cardName);

        GUILayout.Label("Function:");
        foreach (Card card in function)
            GUILayout.Label(card.cardName);

        GUILayout.Label("Action Q:");
        foreach (CardAction a in actionQ)
            GUILayout.Label($"{a.movement}, {a.eulerRotation}");

        GUILayout.EndArea();
    }

    public void DrawCard()
    {
        Card card = Card.CreateInstance<Card>();
        int cardIndex = Random.Range(0, cardsInDeck.Count);
        Card deckCard = cardsInDeck[cardIndex];
        card.cardName = deckCard.cardName;
        card.actions = new List<CardAction>();
        foreach (CardAction action in deckCard.actions)
        {
            CardAction a = CardAction.CreateInstance<CardAction>();
            a.eulerRotation = action.eulerRotation;
            a.movement = action.movement;
            card.actions.Add(a);
        }
        cardsInHand.Add(card);
    }

    public void PlayCard(int cardNum)
    {
        if (cardNum >= cardsInHand.Count)
            return;

        Card card = cardsInHand[cardNum];
        cardsInHand.RemoveAt(cardNum);

        ApplyCard(card);
    }

    public void ApplyCard(Card card)
    {
        foreach (CardAction action in card.actions)
        {
            actionQ.Enqueue(action);
            // Debug.Log($"{action.movement}, {action.eulerRotation}");
        }

        // netPosition.Value += transform.forward * card.movement.z + transform.up * card.movement.y + transform.right * card.movement.x;
    }

    public void AddCardToFunction(int cardIndex)
    {
        if (cardIndex >= cardsInHand.Count)
            return;

        Card card = cardsInHand[cardIndex];
        cardsInHand.RemoveAt(cardIndex);

        function.Add(card);
    }

    public void DiscardCard(int cardIndex)
    {
        if (cardIndex >= cardsInHand.Count)
            return;

        cardsInHand.RemoveAt(cardIndex);
    }

    public void RunFunction()
    {
        if (function.Count == 0)
            return;

        foreach (Card card in function)
        {
            ApplyCard(card);
        }

        function.RemoveAt(0);
    }

    // Update is called once per frame
    void Update()
    {
        if (currentAction == null && actionQ.Count > 0)
        {
            //Time to dequeue the next action
            currentAction = actionQ.Dequeue();

            movementEndpoint = transform.position +
                transform.forward * currentAction.movement.z +
                transform.up * currentAction.movement.y +
                transform.right * currentAction.movement.x;

            rotationEndpoint = transform.rotation;
            rotationEndpoint.eulerAngles += currentAction.eulerRotation;
        }

        if (currentAction != null)
        {
            actionTimeCounter += Time.deltaTime;
            if (actionTimeCounter > actionDuration)
            {
                actionTimeCounter = 0;
                currentAction = null;
                transform.position = movementEndpoint;
                transform.rotation = rotationEndpoint;
            }
            else
            {
                transform.position = Vector3.Lerp(transform.position, movementEndpoint, Time.deltaTime * moveSpeed);
                transform.rotation = Quaternion.Slerp(transform.rotation, rotationEndpoint, Time.deltaTime * rotationSpeed);
            }
        }
    }
}
