using BehaviourAPI.BehaviourTrees;
using BehaviourAPI.Core;
using BehaviourAPI.Core.Actions;
using BehaviourAPI.Core.Perceptions;
using BehaviourAPI.StateMachines;
using BehaviourAPI.UnityToolkit;
using BehaviourAPI.UnityToolkit.GUIDesigner.Runtime;
using System;
using System.Runtime.InteropServices;
using System.Xml;
using UnityEngine;
using Action = System.Action;

public class ComportamientoReina : MonoBehaviour
{
    public StateTransition FedStateTrans;
    public StateTransition UnfedStateTrans;
    public StateTransition LayEggStateTrans;
    public StateTransition LowLifeStateTrans;
    public StateTransition DieStateTrans;

    public PushPerception FedPush;
    public PushPerception UnfedPush;
    public PushPerception LayEggPush;
    public PushPerception LowLifePush;

    public System.Action StartWaitForFood;
    public Func<Status> UpdateWaitForFood;

    public System.Action StartLayEgg;
    public Func<Status> UpdateLayEgg;

    public System.Action StartGenerateRoyalJelly;
    public Func<Status> UpdateGenerateRoyalJelly;

    public System.Action StartDie;
    public Func<Status> UpdateDie;

    public float lifeTime;

    public FSM QueenBehaviour = new FSM();

    protected BehaviourGraph CreateGraph()
    {

        //CREAMOS LOS ESTADOS
        FunctionalAction WaitForFoodAction = new FunctionalAction();
        WaitForFoodAction.onStarted = StartWaitForFood;
        WaitForFoodAction.onUpdated = UpdateWaitForFood;
        State WaitForFoodState = QueenBehaviour.CreateState("WaitForFood", WaitForFoodAction);

        FunctionalAction LayEggAction = new FunctionalAction();
        LayEggAction.onStarted = StartLayEgg;
        LayEggAction.onUpdated = UpdateLayEgg;
        State LayEggState = QueenBehaviour.CreateState("lay_egg", LayEggAction);

        FunctionalAction GenerateRoyalJellyAction = new FunctionalAction();
        GenerateRoyalJellyAction.onStarted = StartGenerateRoyalJelly;
        GenerateRoyalJellyAction.onUpdated = UpdateGenerateRoyalJelly;
        State GenerateRoyalJellyState = QueenBehaviour.CreateState("generate_royal_jelly", GenerateRoyalJellyAction);

        FunctionalAction DieAction = new FunctionalAction();
        DieAction.onStarted = StartDie; // Implementa este método para destruir la entidad
        State DieState = QueenBehaviour.CreateState("Die", DieAction);

        //CREAMOS LAS TRANSICIONES
        Transition fedToLayEgg = QueenBehaviour.CreateTransition(WaitForFoodState, LayEggState, statusFlags: StatusFlags.Success);
        Transition layEggToWaitForFood = QueenBehaviour.CreateTransition(LayEggState, WaitForFoodState, statusFlags: StatusFlags.Success);
        Transition fedToGenerateJelly = QueenBehaviour.CreateTransition(WaitForFoodState, GenerateRoyalJellyState, statusFlags: StatusFlags.Success);
        Transition deathTransition = QueenBehaviour.CreateTransition(GenerateRoyalJellyState, DieState, statusFlags: StatusFlags.Success);
        Transition waitForFoodToWaitForFood = QueenBehaviour.CreateTransition(WaitForFoodState, WaitForFoodState, statusFlags: StatusFlags.Success);


        //CREAMOS PERCEPCIONES
        FedPush = new PushPerception(fedToLayEgg);
        UnfedPush = new PushPerception(waitForFoodToWaitForFood);
        LayEggPush = new PushPerception(layEggToWaitForFood);
        LowLifePush = new PushPerception(fedToGenerateJelly);

        // Implementar lógica para los métodos de inicio y actualización aquí

        return QueenBehaviour;
    }
}


/*
 using BehaviourAPI.BehaviourTrees;
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
}
 
 */



/*public StateTransition SurfaceToNest;
    public StateTransition NestToSurface;
    public StateTransition FoodDetected;
    public StateTransition PredatorDetected;
    public StateTransition PredatorLost;

    public PullPerception IsInNest;
    public PullPerception IsInSurface;
    public PullPerception IsFoodDetected;
    public PullPerception IsPredatorDetected;
    public PullPerception IsCarryingFood;

    public Action StartExploring;
    public Func<Status> UpdateExploring;

    public Action StartCollectingFood;
    public Func<Status> UpdateCollectingFood;

    public Action StartFleeing;
    public Func<Status> UpdateFleeing;

    public Action StartReturningToNest;
    public Func<Status> UpdateReturningToNest;

    public Action StartWaiting;
    public Func<Status> UpdateWaiting;

    public FSM CollectorAntBehaviour = new FSM();

    protected override BehaviourGraph CreateGraph()
    {
        FunctionalAction ExploreAction = new FunctionalAction();
        ExploreAction.onStarted = StartExploring;
        ExploreAction.onUpdated = UpdateExploring;
        State ExploreState = CollectorAntBehaviour.CreateState("Explore", ExploreAction);

        FunctionalAction CollectFoodAction = new FunctionalAction();
        CollectFoodAction.onStarted = StartCollectingFood;
        CollectFoodAction.onUpdated = UpdateCollectingFood;
        State CollectFoodState = CollectorAntBehaviour.CreateState("Collect Food", CollectFoodAction);

        FunctionalAction FleeAction = new FunctionalAction();
        FleeAction.onStarted = StartFleeing;
        FleeAction.onUpdated = UpdateFleeing;
        State FleeState = CollectorAntBehaviour.CreateState("Flee", FleeAction);

        FunctionalAction ReturnToNestAction = new FunctionalAction();
        ReturnToNestAction.onStarted = StartReturningToNest;
        ReturnToNestAction.onUpdated = UpdateReturningToNest;
        State ReturnToNestState = CollectorAntBehaviour.CreateState("Return To Nest", ReturnToNestAction);

        FunctionalAction WaitAction = new FunctionalAction();
        WaitAction.onStarted = StartWaiting;
        WaitAction.onUpdated = UpdateWaiting;
        State WaitState = CollectorAntBehaviour.CreateState("Wait", WaitAction);

        SurfaceToNest = CollectorAntBehaviour.CreateTransition(ExploreState, ReturnToNestState, statusFlags: StatusFlags.Success);
        NestToSurface = CollectorAntBehaviour.CreateTransition(ReturnToNestState, ExploreState, statusFlags: StatusFlags.None);
        FoodDetected = CollectorAntBehaviour.CreateTransition(ExploreState, CollectFoodState, statusFlags: StatusFlags.Success);
        PredatorDetected = CollectorAntBehaviour.CreateTransition(ExploreState, FleeState, statusFlags: StatusFlags.Failure);
        PredatorLost = CollectorAntBehaviour.CreateTransition(FleeState, WaitState, statusFlags: StatusFlags.Success);

        IsInNest = new PullPerception(NestToSurface);
        IsInSurface = new PullPerception(SurfaceToNest);
        IsFoodDetected = new PullPerception(FoodDetected);
        IsPredatorDetected = new PullPerception(PredatorDetected);
        IsCarryingFood = new PullPerception(ReturnToNestState);

        return CollectorAntBehaviour;*/




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