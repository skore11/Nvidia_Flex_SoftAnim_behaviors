using UnityEngine;
using UnityEngine.UI;
 
using UnityEditor;
 
// ---------------
//  String => Int
// ---------------
[UnityEditor.CustomPropertyDrawer(typeof(StringIntDictionary))]
public class StringIntDictionaryDrawer : SerializableDictionaryDrawer<string, int> {
    protected override SerializableKeyValueTemplate<string, int> GetTemplate() {
        return GetGenericTemplate<SerializableStringIntTemplate>();
    }
}
internal class SerializableStringIntTemplate : SerializableKeyValueTemplate<string, int> {}
 
// ---------------
//  GameObject => Float
// ---------------
[UnityEditor.CustomPropertyDrawer(typeof(GameObjectFloatDictionary))]
public class GameObjectFloatDictionaryDrawer : SerializableDictionaryDrawer<GameObject, float> {
    protected override SerializableKeyValueTemplate<GameObject, float> GetTemplate() {
        return GetGenericTemplate<SerializableGameObjectFloatTemplate>();
    }
}
internal class SerializableGameObjectFloatTemplate : SerializableKeyValueTemplate<GameObject, float> {}

// ---------------
//  String => String
// ---------------
[UnityEditor.CustomPropertyDrawer(typeof(StringStringDictionary))]
public class StringStringDictionaryDrawer : SerializableDictionaryDrawer<string, string> {
    protected override SerializableKeyValueTemplate<string, string> GetTemplate() {
        return GetGenericTemplate<SerializableStringStringTemplate>();
    }
}
internal class SerializableStringStringTemplate : SerializableKeyValueTemplate<string, string> { }

// ---------------
//  String => GameObject
// ---------------
[UnityEditor.CustomPropertyDrawer(typeof(StringGameObjectDictionary))]
public class StringGameObjectDictionaryDrawer : SerializableDictionaryDrawer<string, GameObject> {
    protected override SerializableKeyValueTemplate<string, GameObject> GetTemplate() {
        return GetGenericTemplate<SerializableStringGameObjectTemplate>();
    }
}
internal class SerializableStringGameObjectTemplate : SerializableKeyValueTemplate<string, GameObject> { }

// ---------------
//  Int => Vector3
// ---------------
[UnityEditor.CustomPropertyDrawer(typeof(IntVector3Dictionary))]
public class IntVector3DictionaryDrawer : SerializableDictionaryDrawer<int, Vector3>
{
    protected override SerializableKeyValueTemplate<int, Vector3> GetTemplate()
    {
        return GetGenericTemplate<SerializableIntVector3Template>();
    }
}
internal class SerializableIntVector3Template : SerializableKeyValueTemplate<int, Vector3> { }

// ---------------
//  String => (Int => Vector3)
// ---------------
[UnityEditor.CustomPropertyDrawer(typeof(StringIntVector3Dictionary))]
public class StringIntVector3DictionaryDrawer : SerializableDictionaryDrawer<string, IntVector3Dictionary>
{
    protected override SerializableKeyValueTemplate<string, IntVector3Dictionary> GetTemplate()
    {
        return GetGenericTemplate<SerializableStringIntVector3Template>();
    }
}
internal class SerializableStringIntVector3Template : SerializableKeyValueTemplate<string, IntVector3Dictionary> { }
