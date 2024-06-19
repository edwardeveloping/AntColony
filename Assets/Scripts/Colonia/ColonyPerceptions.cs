using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColonyPerceptions : MonoBehaviour
{
    public Colony colony;

    public bool CheckPerception(Colony.Perception perception)
    {
        switch (perception)
        {
            case Colony.Perception.AtaqueALaColonia:
                return colony.IsUnderAttack();
            case Colony.Perception.FaltaDeComida:
                return colony.IsFoodInsufficient();
            case Colony.Perception.IncapacidadParaRecolectarComida:
                return colony.AreGatherersUnableToCollectFood();
            case Colony.Perception.ExcesoDeHuevosAlmacenados:
                return colony.AreThereTooManyStoredEggs();
            case Colony.Perception.FaltaDeHuevos:
                return colony.IsThereALackOfEggs();
            case Colony.Perception.DisminucionDePoblacionDeSoldados:
                return colony.IsSoldierPopulationDecreasing();
            case Colony.Perception.ClimaAdverso:
                return colony.IsAdverseWeather();
            case Colony.Perception.RecolectorasOciosas:
                return colony.AreGatherersIdle();
            case Colony.Perception.ExcesoDeComidaYLarvasHambrientas:
                return colony.IsThereExcessFoodAndHungryLarvas();
            default:
                return false;
        }
    }
}
