using System;
using System.Data;
using System.Reflection;
using ADOFAI;
using ADOFAI.LevelEditor.Controls;
using JALib.Core;
using JALib.Core.Patch;
using JALib.Tools;
using NCalc;
using SmartEditor.FixLoad.CustomSaveState;
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
        Main.Instance.Log("Test1 PropertyControl_Text");
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
                catch (Exception e) {
                    Main.Instance.LogException(e);
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
            catch (Exception e) {
                Main.Instance.LogException(e);
                num1 = (int) __instance.propertyInfo.value_default;
            }
        }
        return num1.ToString();
    }
    [JAPatch(typeof(PropertyControl_Vector2), "Validate", PatchType.Replace, false)]
    public static (string, string) Vector2PatchEntry(PropertyControl_Vector2 __instance, Vector2 ___lastValue, TMP_InputField x, TMP_InputField y) {
        Main.Instance.Log("Test2 PropertyControl_Vector2");
        Vector2 vector2 = new Vector2(___lastValue.x, ___lastValue.y);
        string naN1 = FixPrivateMethod.ConvertEmptyToNaN(x.text);
        string naN2 = FixPrivateMethod.ConvertEmptyToNaN(y.text);
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
                Main.Instance.Log(dictValue);
                vector2.x = RDEditorUtils.DecodeFloat(dictValue);
            }
            catch (Exception e) {
                Main.Instance.LogException(e);
            }
            try
            {
                object dictValue = new Expression(naN2).Evaluate();
                Main.Instance.Log(dictValue);
                vector2.y = RDEditorUtils.DecodeFloat(dictValue);
            }
            catch (Exception e) {
                Main.Instance.LogException(e);
            }
        }
        if (__instance.propertiesPanel.inspectorPanel.selectedEvent.eventType == LevelEventType.AddDecoration && __instance.propertyInfo.name == "tile")
        {
            vector2.x = (float) Mathf.RoundToInt(vector2.x);
            vector2.y = (float) Mathf.RoundToInt(vector2.y);
        }
        return (FixPrivateMethod.ConvertNaNToEmpty(vector2.x.ToString("0.######")), FixPrivateMethod.ConvertNaNToEmpty(vector2.y.ToString("0.######")));
    }
}