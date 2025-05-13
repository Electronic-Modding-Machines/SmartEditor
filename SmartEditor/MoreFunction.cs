using System.Data;
using System.Reflection;
using ADOFAI;
using ADOFAI.LevelEditor.Controls;
using JALib.Core;
using JALib.Core.Patch;
using JALib.Tools;
using NCalc;
using TMPro;
using UnityEngine;

namespace SmartEditor;

public class MoreFunction() : Feature(Main.Instance, nameof(MoreFunction), patchClass: typeof(MoreFunction)) {
    protected override void OnEnable() {
        base.OnEnable();
    }
    
    protected override void OnDisable() {
        base.OnDisable();
    }

    [JAPatch(typeof(PropertyControl_Text), "Validate", PatchType.Replace, false)]
    public static string TextPatchEntry(PropertyControl_Text __instance)
    {
        if (__instance.propertyInfo == null)
            return __instance.inputField.text;
        if (__instance.propertyInfo.type == PropertyType.Float)
        {
            float result = 1f;
            float num;
            if (float.TryParse(__instance.inputField.text, out result))
            {
                num = __instance.propertyInfo.Validate(result);
            }
            else
            {
                try
                {
                    num = __instance.propertyInfo.Validate(RDEditorUtils.DecodeFloat(new Expression(__instance.inputField.text).Evaluate()));
                }
                catch
                {
                    num = (float) __instance.propertyInfo.value_default;
                }
            }
            return num.ToString();
        }
        if (__instance.propertyInfo.type != PropertyType.Int && __instance.propertyInfo.type != PropertyType.Tile)
            return __instance.inputField.text;
        float result1;
        int num1;
        if (float.TryParse(__instance.inputField.text, out result1))
        {
            num1 = __instance.propertyInfo.Validate(Mathf.RoundToInt(result1));
        }
        else
        {
            try
            {
                num1 = RDEditorUtils.DecodeInt(new Expression(__instance.inputField.text).Evaluate());
            }
            catch
            {
                num1 = (int) __instance.propertyInfo.value_default;
            }
        }
        return num1.ToString();
    }
    [JAPatch(typeof(PropertyControl_Vector2), "Validate", PatchType.Replace, false)]
    public static (string, string) Vector2PatchEntry(PropertyControl_Vector2 __instance, Vector2 lastValue, TMP_InputField ___x, TMP_InputField ___y) {
        Vector2 vector2 = new Vector2(lastValue.x, lastValue.y);
        MethodInfo convertEmptyToNaN = typeof(PropertyControl_Vector2).Method("ConvertEmptyToNaN");
        string naN1 = (string)convertEmptyToNaN.Invoke(null, new object[] { ___x.text });
        string naN2 = (string)convertEmptyToNaN.Invoke(null, new object[] { ___y.text });
        float result1;
        float result2;
        if (float.TryParse(naN1, out result1) && float.TryParse(naN2, out result2))
        {
            vector2 = new Vector2(result1, result2);
            vector2 = __instance.propertyInfo.Validate(vector2, __instance.propertiesPanel.inspectorPanel.selectedEvent.isFake);
        }
        else
        {
            DataTable dataTable = new DataTable();
            try
            {
                object dictValue = new Expression(naN1).Evaluate();
                vector2.x = RDEditorUtils.DecodeFloat(dictValue);
            }
            catch
            {
            }
            try
            {
                object dictValue = new Expression(naN2).Evaluate();
                vector2.y = RDEditorUtils.DecodeFloat(dictValue);
            }
            catch
            {
            }
        }
        if (__instance.propertiesPanel.inspectorPanel.selectedEvent.eventType == LevelEventType.AddDecoration && __instance.propertyInfo.name == "tile")
        {
            vector2.x = (float) Mathf.RoundToInt(vector2.x);
            vector2.y = (float) Mathf.RoundToInt(vector2.y);
        }
        MethodInfo convertNaNToEmpty = typeof(PropertyControl_Vector2).Method("ConvertNaNToEmpty");
        return (
                   (string)convertNaNToEmpty.Invoke(__instance, new object[] { vector2.x.ToString("0.######") }),
                   (string)convertNaNToEmpty.Invoke(__instance, new object[] { vector2.y.ToString("0.######") })
               );
    }
}