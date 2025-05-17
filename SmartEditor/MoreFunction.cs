using System;
using System.Data;
using System.Text.RegularExpressions;
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
    public static string RangeToRandom(string input) {
        const string pattern = @"-?\d+(\.\d+)?\s*~\s*-?\d+(\.\d+)?";
        return Regex.Replace(input, pattern, match => {
            string range = match.Value;
            string[] parts = range.Split('~');
            string minStr = parts[0].Trim();
            string maxStr = parts[1].Trim();
            float min = float.Parse(minStr);
            float max = float.Parse(maxStr);
            if(min > max) (min, max) = (max, min);
            JARandom rnd = JARandom.Instance;
            int decimalPlaces = Math.Max(GetDecimalPlaces(minStr), GetDecimalPlaces(maxStr));
            if(decimalPlaces == 0) {
                int result = rnd.Next((int) min, (int) max + 1);
                return result.ToString();
            } else {
                int factor = (int) MathF.Pow(10, decimalPlaces);
                int minInt = (int) MathF.Round(min * factor);
                int maxInt = (int) MathF.Round(max * factor);
                int result = rnd.Next(minInt, maxInt + 1);
                float finalResult = result / (float) factor;
                return finalResult.ToString($"F{decimalPlaces}");
            }
        });
    }

    public static int GetDecimalPlaces(string s) {
        int dotIndex = s.IndexOf('.');
        if(dotIndex == -1) return 0;
        return s.Length - dotIndex - 1;
    }

    public static string Capitalize(string input) {
        return Regex.Replace(input, "[a-zA-Z]+", match => {
            string word = match.Value;
            return char.ToUpper(word[0]) + word[1..].ToLower();
        });
    }

    public static void CheckDegree(string name, FunctionArgs args) {
        double deg = Convert.ToDouble(args.Parameters[0].Evaluate());
        double rad = deg * Math.PI / 180;

        args.Result = name.ToLower() switch {
            "dsin" => Math.Sin(rad),
            "dcos" => Math.Cos(rad),
            "dtan" => Math.Tan(rad),
            _ => args.Result
        };
    }

    public static object EvalExpr(string expression) {
        Expression expr = new(expression);
        expr.EvaluateFunction += CheckDegree;

        return expr.Evaluate();
    }

    [JAPatch(typeof(PropertyControl_Text), "Validate", PatchType.Replace, false)]
    public static string TextPatchEntry(PropertyControl_Text __instance) {
        if(__instance.propertyInfo == null) return __instance.inputField.text;
        if(__instance.propertyInfo.type == PropertyType.Float) {
            float num;
            if(float.TryParse(__instance.inputField.text, out float result)) num = __instance.propertyInfo.Validate(result);
            else {
                try {
                    num = __instance.propertyInfo.Validate(RDEditorUtils.DecodeFloat(Calculate(__instance.inputField.text)));
                } catch (Exception e) {
                    Main.Instance.LogException(e);
                    num = (float) __instance.propertyInfo.value_default;
                }
            }
            return num.ToString();
        }
        if(__instance.propertyInfo.type != PropertyType.Int && __instance.propertyInfo.type != PropertyType.Tile) return __instance.inputField.text;
        int num1;
        if(float.TryParse(__instance.inputField.text, out float result1)) num1 = __instance.propertyInfo.Validate(Mathf.RoundToInt(result1));
        else {
            try {
                num1 = RDEditorUtils.DecodeInt(Calculate(__instance.inputField.text));
            } catch (Exception e) {
                Main.Instance.LogException(e);
                num1 = (int) __instance.propertyInfo.value_default;
            }
        }
        return num1.ToString();
    }

    [JAPatch(typeof(PropertyControl_Vector2), "Validate", PatchType.Replace, false)]
    public static (string, string) Vector2PatchEntry(PropertyControl_Vector2 __instance, Vector2 ___lastValue, TMP_InputField x, TMP_InputField y) {
        Vector2 vector2 = new(___lastValue.x, ___lastValue.y);
        string naN1 = ConvertEmptyToNaN(x.text);
        string naN2 = ConvertEmptyToNaN(y.text);
        if(float.TryParse(naN1, out float result1) && float.TryParse(naN2, out float result2)) {
            vector2 = new Vector2(result1, result2);
            vector2 = __instance.propertyInfo.Validate(vector2, __instance.propertiesPanel.inspectorPanel.selectedEvent.isFake);
        } else {
            try {
                vector2.x = RDEditorUtils.DecodeFloat(Calculate(naN1));
            } catch (Exception e) {
                Main.Instance.LogException(e);
            }
            try {
                vector2.y = RDEditorUtils.DecodeFloat(Calculate(naN2));
            } catch (Exception e) {
                Main.Instance.LogException(e);
            }
        }

        if(__instance.propertiesPanel.inspectorPanel.selectedEvent.eventType == LevelEventType.AddDecoration && __instance.propertyInfo.name == "tile") { 
            vector2.x = Mathf.RoundToInt(vector2.x);
            vector2.y = Mathf.RoundToInt(vector2.y);
        }

        return (ConvertNaNToEmpty(vector2.x.ToString("0.######")), ConvertNaNToEmpty(vector2.y.ToString("0.######")));
    }

    private static object Calculate(string value) => EvalExpr(Capitalize(RangeToRandom(value)));
    
    private static string ConvertNaNToEmpty(string s) => s == "NaN" ? "" : s;

    private static string ConvertEmptyToNaN(string s) => s == "" ? "NaN" : s;
}