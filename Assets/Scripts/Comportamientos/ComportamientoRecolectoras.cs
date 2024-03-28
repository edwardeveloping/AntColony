using System;
using System.Collections;
using System.Collections.Generic;
using BehaviourAPI.BehaviourTrees;
using BehaviourAPI.Core;
using BehaviourAPI.Core.Actions;
using BehaviourAPI.Core.Perceptions;
using BehaviourAPI.UnityToolkit;
using UnityEngine;
using BehaviourAPI.StateMachines; ///////
using Action = System.Action;


public class ComportamientoRecolectoras : MonoBehaviour
{




    #region BT

    public StateTransition FoodDetected;
    public StateTransition PredatorDetected;
    public StateTransition IsCarryingFood;

    public PushPerception FoodDetectedPush;
    public PushPerception PredatorDetectedPush;
    public PushPerception IsCarryingFoodPush;

    public Action ExploreAction;
    public Action CollectFoodAction;
    public Action FleeAction;
    public Action ReturnToNestAction;

    public Func<bool> ConditionFoodDetectedCheck;
    public Func<bool> ConditionPredatorDetectedCheck;
    public Func<bool> ConditionIsCarryingFoodCheck;

    #endregion BT

    public BehaviourTree CollectorAntBehaviour = new BehaviourTree();

    protected BehaviourGraph CreateGraph()
    {
        // Definir acciones
        ExploreAction = () => { /* Implementar lógica de exploración */ };
        CollectFoodAction = () => { /* Implementar lógica de recolección de comida */ };
        FleeAction = () => { /* Implementar lógica de huida */ };
        ReturnToNestAction = () => { /* Implementar lógica de retorno al hormiguero */ };

        // Definir condiciones
        ConditionFoodDetectedCheck = () => { /* Implementar lógica de detección de comida */ };
        ConditionPredatorDetectedCheck = () => { /* Implementar lógica de detección de depredadores */ };
        ConditionIsCarryingFoodCheck = () => { /* Implementar lógica de llevar comida */ };

        // Crear percepciones
        FoodDetected = CollectorAntBehaviour.CreateTransition(ExploreAction, CollectFoodAction, ConditionFoodDetectedCheck);
        PredatorDetected = CollectorAntBehaviour.CreateTransition(ExploreAction, FleeAction, ConditionPredatorDetectedCheck);
        IsCarryingFood = CollectorAntBehaviour.CreateTransition(CollectFoodAction, ReturnToNestAction, ConditionIsCarryingFoodCheck);

        FoodDetectedPush = new PushPerception(FoodDetected);
        PredatorDetectedPush = new PushPerception(PredatorDetected);
        IsCarryingFoodPush = new PushPerception(IsCarryingFood);

        return CollectorAntBehaviour;
    }
}


    /*public Action Explorar;
    public Action RecoleactarComida;
    public Action Huir;
    public Action RegresarNido;

    public PushPerception DetectarComida;
    public PushPerception DetectarDepredador;
    public PushPerception LlevaComida;

    public BehaviourTree ComportamientoRecolectora = new BehaviourTree();

    protected BehaviourGraph CreateGraph()
    {
        // Definir las acciones
        Explorar = new ActionNode("Explore");
        RecoleactarComida = new ActionNode("Collect Food");
        Huir = new ActionNode("Flee");
        RegresarNido = new ActionNode("Return To Nest");

        // Definir las percepciones como PushPerception
        DetectarComida = new PushPerception();
        DetectarDepredador = new PushPerception();
        LlevaComida = new PushPerception();

        // Crear el Ã¡rbol de comportamiento
        ComportamientoRecolectora.AddChild(
            new SelectorNode(
                new SequenceNode(
                    new LeafNode(DetectarComida),
                    RecoleactarComida
                ),
                new SequenceNode(
                    new LeafNode(DetectarDepredador),
                    Huir
                ),
                RegresarNido
            )
        );

        return ComportamientoRecolectora;
    }

  
}

/*using BehaviourAPI.BehaviourTrees;
using BehaviourAPI.Core;
using BehaviourAPI.Core.Actions;
using BehaviourAPI.Core.Perceptions;
using BehaviourAPI.StateMachines;
using BehaviourAPI.UnityToolkit;
using BehaviourAPI.UnityToolkit.GUIDesigner.Runtime;
using System;
using Action = System.Action;

namespace GGG.Components.Enemies
{
    public class NormalEnemyAI : BehaviourRunner
    {
        public StateTransition DetectPlayer;
        public StateTransition LostPatience;
        public StateTransition Rested;

        public PushPerception playerDetected;
        public PushPerception playerLost;
        public PushPerception RestedPush;

        public System.Action StartPatrol;
        public System.Func<Status> UpdatePatrol;

        public System.Action StartChase;
        public System.Func<Status> UpdateChase;
        public System.Action StartSleep;

        public float sleepTime = 2.0f;

        public FSM NormalEnemyBehaviour = new FSM();

        #region BT

        public StateTransition Notified;
        public StateTransition EnemyFoundWhileBT;
        public StateTransition EnemyNotFoundWhileBT;

        public PushPerception EnemyFoundWhileBTPush;
        public PushPerception EnemyNotFoundWhileBTPush;
        public PushPerception NotifiedPush;

        public Action StartCheckOnDestination;
        public Func<Status> UpdateCheckOnDestination;

        public Action StartChaseExit;
        public Func<Status> UpdateChaseExit;

        public Action StartMoveClose;
        public Func<Status> UpdateMoveClose;

        public Action StartPatrolExit;
        public Func<Status> UpdatePatrolExit;

        public Action StartWalkToDestination;
        public Func<Status> UpdateWalkToDestination;

        public Func<bool> ConditionSeePlayerCheck;
        public Func<bool> ConditionKeepSearchingCheck;

        #endregion BT

        protected override BehaviourGraph CreateGraph()
        {
            FunctionalAction Patrol_action = new FunctionalAction();
            Patrol_action.onStarted = StartPatrol;
            Patrol_action.onUpdated = UpdatePatrol;
            State Patrol = NormalEnemyBehaviour.CreateState("patrol", Patrol_action);

            FunctionalAction Chase_action = new FunctionalAction();
            Chase_action.onStarted = StartChase;
            Chase_action.onUpdated = UpdateChase;
            State Chase = NormalEnemyBehaviour.CreateState("chase", Chase_action);

            FunctionalAction Sleep_action = new FunctionalAction();

            Sleep_action.onStarted = StartSleep;
            State Sleep = NormalEnemyBehaviour.CreateState("sleep", Sleep_action);

            DetectPlayer = NormalEnemyBehaviour.CreateTransition(Patrol, Chase, statusFlags: StatusFlags.None);

            LostPatience = NormalEnemyBehaviour.CreateTransition(Chase, Sleep, statusFlags: StatusFlags.None);

            Rested = NormalEnemyBehaviour.CreateTransition(Sleep, Patrol, statusFlags: StatusFlags.None);

            playerDetected = new PushPerception(DetectPlayer);
            playerLost = new PushPerception(LostPatience);
            RestedPush = new PushPerception(Rested);

            #region BT

            BehaviourTree subBehaviourTree = new BehaviourTree();

            FunctionalAction CheckOnDestinationAction = new FunctionalAction();
            CheckOnDestinationAction.onStarted = StartCheckOnDestination;
            CheckOnDestinationAction.onUpdated = UpdateCheckOnDestination;
            LeafNode CheckOnDestination = subBehaviourTree.CreateLeafNode(CheckOnDestinationAction);

            FunctionalAction ChaseExitAction = new FunctionalAction();
            ChaseExitAction.onStarted = StartChaseExit;
            ChaseExitAction.onUpdated = UpdateChaseExit;
            LeafNode ChaseExit = subBehaviourTree.CreateLeafNode(ChaseExitAction);

            ConditionNode ConditionSeeNode = subBehaviourTree.CreateDecorator<ConditionNode>(ChaseExit);
            ConditionSeeNode.Perception = new ConditionPerception(ConditionSeePlayerCheck);

            FunctionalAction MoveCloseAction = new FunctionalAction();
            MoveCloseAction.onStarted = StartMoveClose;
            MoveCloseAction.onUpdated = UpdateMoveClose;
            LeafNode MoveClose = subBehaviourTree.CreateLeafNode(MoveCloseAction);

            ConditionNode ConditionKeepSearching = subBehaviourTree.CreateDecorator<ConditionNode>(MoveClose);
            ConditionKeepSearching.Perception = new ConditionPerception(ConditionKeepSearchingCheck);

            FunctionalAction PatrolExitAction = new FunctionalAction();
            PatrolExitAction.onStarted = StartPatrolExit;
            PatrolExitAction.onUpdated = UpdatePatrolExit;
            LeafNode PatrolExit = subBehaviourTree.CreateLeafNode(PatrolExitAction);

            SelectorNode Selector2 = subBehaviourTree.CreateComposite<SelectorNode>(false, ConditionSeeNode, ConditionKeepSearching, PatrolExit);
            Selector2.IsRandomized = false;

            SequencerNode Sequencer1 = subBehaviourTree.CreateComposite<SequencerNode>(false, CheckOnDestination, Selector2);
            Sequencer1.IsRandomized = false;

            FunctionalAction WalkToDestinationAction = new FunctionalAction();
            WalkToDestinationAction.onStarted = StartWalkToDestination;
            WalkToDestinationAction.onUpdated = UpdateWalkToDestination;
            LeafNode WalkToDestination = subBehaviourTree.CreateLeafNode(WalkToDestinationAction);

            SelectorNode Selector1 = subBehaviourTree.CreateComposite<SelectorNode>(false, Sequencer1, WalkToDestination);
            Selector1.IsRandomized = false;

            LoopNode BasicLoop = subBehaviourTree.CreateDecorator<LoopNode>(Selector1);
            BasicLoop.Iterations = -1;

            subBehaviourTree.SetRootNode(BasicLoop);

            #endregion BT

            SubsystemAction notifiedAction = new SubsystemAction(subBehaviourTree);
            State NotifiedState = NormalEnemyBehaviour.CreateState("BT", notifiedAction);

            Notified = NormalEnemyBehaviour.CreateTransition(Patrol, NotifiedState, statusFlags: StatusFlags.Success);
            NotifiedPush = new PushPerception(Notified);

            EnemyFoundWhileBT = NormalEnemyBehaviour.CreateTransition(NotifiedState, Chase, statusFlags: StatusFlags.None);
            EnemyNotFoundWhileBT = NormalEnemyBehaviour.CreateTransition(NotifiedState, Patrol, statusFlags: StatusFlags.None);

            EnemyFoundWhileBTPush = new PushPerception(EnemyFoundWhileBT);
            EnemyNotFoundWhileBTPush = new PushPerception(EnemyNotFoundWhileBT);

            return NormalEnemyBehaviour;
        }
    }
}*/