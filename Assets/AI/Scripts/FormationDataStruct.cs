using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct FormationDataStruct
{
    [SerializeField] [Range(1, 3)] public float unitSpacingFactor;
    [SerializeField] public GameObject unitPrefab;
    [SerializeField] public FormationTypes formation;
    [SerializeField] public float formationRotationSpeed;

    public enum FormationTypes
    { Square, Triangle , None}
}
