using System;
 
using UnityEngine;
 
// ---------------
//  String => Int
// ---------------
[Serializable]
public class StringIntDictionary : SerializableDictionary<string, int> {}
 
// ---------------
//  GameObject => Float
// ---------------
[Serializable]
public class GameObjectFloatDictionary : SerializableDictionary<GameObject, float> {}

// ---------------
//  String => String
// ---------------
[Serializable]
public class StringStringDictionary : SerializableDictionary<string, string> { }

// ---------------
//  String => GameObject
// ---------------
[Serializable]
public class StringGameObjectDictionary : SerializableDictionary<string, GameObject> { }

// ---------------
//  Int => Vector3
// ---------------
[Serializable]
public class IntVector3Dictionary : SerializableDictionary<int, Vector3> { }


// ---------------
//  String => (Int => Vector3)
// ---------------
[Serializable]
public class StringIntVector3Dictionary : SerializableDictionary<string, IntVector3Dictionary> { }