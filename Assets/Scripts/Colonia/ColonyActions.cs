using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColonyActions : MonoBehaviour
{
    public Colony colony;

    public void ExecuteAction(Colony.Action action)
    {
        switch (action)
        {
            case Colony.Action.AsignarRecolectoras:
                colony.AssignMoreGatherers();
                break;
            case Colony.Action.AsignarSoldados:
                colony.AssignMoreSoldiers();
                break;
            case Colony.Action.AsignarOtroRolARecolectorasOciosas:
                colony.AssignNewRoleToIdleGatherers();
                break;
            case Colony.Action.AsignarObreras:
                colony.AssignMoreWorkers();
                break;
        }
    }
}
