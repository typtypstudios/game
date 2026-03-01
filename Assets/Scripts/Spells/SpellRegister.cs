using UnityEngine;

[NoAutoCreate]
[CreateAssetMenu(fileName = "SpellRegister", menuName = "TypTyp/Spells/SpellRegister", order = 1)]
public class SpellRegister : ScriptableRegister<SpellDefinition, SpellRegister>
{

}
/*
https://docs.unity3d.com/6000.3/Documentation/ScriptReference/ScriptableObject.html
Awake	    Called when an instance of ScriptableObject is created.
OnDestroy	This function is called when the scriptable object will be destroyed.
OnDisable	This function is called when the scriptable object goes out of scope.
OnEnable	This function is called when the object is loaded.
OnValidate	Editor-only function that Unity calls when the script is loaded or a value changes in the Inspector.
Reset	    Reset to default values.
*/