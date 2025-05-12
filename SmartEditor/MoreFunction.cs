using System.Data;
using ADOFAI;
using ADOFAI.LevelEditor.Controls;
using JALib.Core;
using JALib.Core.Patch;
using NCalc;
using UnityEngine;

namespace SmartEditor;

public class MoreFunction() : Feature(Main.Instance, nameof(MoreFunction)) {
    protected override void OnEnable() {
        base.OnEnable();
    }
    
    protected override void OnDisable() {
        base.OnDisable();
    }

    [JAPatch(typeof(PropertyControl_Text), "Validate", PatchType.Replace, false)]
    public static string PatchEntry(PropertyControl_Text __instance)
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
}