namespace Secrets.Editor
{
    using UnityEngine;
    using UnityEditor;
    using System.Text;
    public class ColorPaletteTool : EditorWindow
    {
        private Color mainColor = Color.white;
        private Color secondaryColor = Color.white;
        private bool useSecondaryColor = false;
        private int paletteSize = 6;
        private Color[] generatedPalette;
        private bool showHexValues = true;
        private Vector2 scrollPosition;
        private Vector2 scrollPositionHelpBox;
        private bool autoGenerate = true;
        private string colorArrayString = "";

        [MenuItem("Tools/Color Palette Generator")]
        private static void ShowWindow()
        {
            var window = GetWindow<ColorPaletteTool>("Color Palette");
            window.minSize = new Vector2(300, 400);
        }

        private void OnEnable()
        {
            GeneratePalette();
        }

        private void OnGUI()
        {
            EditorGUI.BeginChangeCheck();

            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                EditorGUILayout.LabelField("Palette Settings", EditorStyles.boldLabel);
                mainColor = EditorGUILayout.ColorField("Main Color", mainColor);

                useSecondaryColor = EditorGUILayout.Toggle("Use Secondary Color", useSecondaryColor);
                if (useSecondaryColor)
                {
                    secondaryColor = EditorGUILayout.ColorField("Secondary Color", secondaryColor);
                }

                paletteSize = EditorGUILayout.IntSlider("Palette Size", paletteSize, 2, 12);
                autoGenerate = EditorGUILayout.Toggle("Auto Generate", autoGenerate);
                showHexValues = EditorGUILayout.Toggle("Show Hex Values", showHexValues);
            }

            if (EditorGUI.EndChangeCheck() && autoGenerate)
            {
                GeneratePalette();
            }

            EditorGUILayout.Space();

            if (!autoGenerate && GUILayout.Button("Generate Palette", GUILayout.Height(30)))
            {
                GeneratePalette();
            }

            if (GUILayout.Button("Copy Color Array", GUILayout.Height(30)))
            {
                CopyColorArrayToClipboard();
            }

            EditorGUILayout.Space();

            if (generatedPalette != null && generatedPalette.Length > 0)
            {
                DisplayPalette();
            }
        }

        private void DisplayPalette()
        {
            using (var scrollView = new EditorGUILayout.ScrollViewScope(scrollPosition))
            {
                scrollPosition = scrollView.scrollPosition;

                for (int i = 0; i < generatedPalette.Length; i++)
                {
                    using (new EditorGUILayout.HorizontalScope(EditorStyles.helpBox))
                    {
                        var color = generatedPalette[i];

                        EditorGUI.DrawRect(GUILayoutUtility.GetRect(50, 35), color);

                        EditorGUILayout.Space();

                        if (showHexValues)
                        {
                            EditorGUILayout.LabelField($"#{ColorUtility.ToHtmlStringRGB(color)}", GUILayout.Width(80));
                        }

                        EditorGUILayout.LabelField($"R: {color.r:F2} G: {color.g:F2} B: {color.b:F2}", GUILayout.Width(200));
                    }
                }
            }

            EditorGUILayout.Space();

            using (var scrollView = new EditorGUILayout.ScrollViewScope(scrollPositionHelpBox))
            {
                scrollPositionHelpBox = scrollView.scrollPosition;
                EditorGUILayout.TextArea(colorArrayString);
            }
        }

        private void GeneratePalette()
        {
            generatedPalette = new Color[paletteSize];

            Color.RGBToHSV(mainColor, out float h, out float s, out float v);

            if (useSecondaryColor)
            {
                Color.RGBToHSV(secondaryColor, out float h2, out float s2, out float v2);
                GeneratePaletteWithSecondary(h, s, v, h2, s2, v2);
            }
            else
            {
                GeneratePaletteFromMain(h, s, v);
            }

            UpdateColorArrayString();
        }

        private void GeneratePaletteFromMain(float h, float s, float v)
        {
            generatedPalette[0] = mainColor;

            float goldenRatio = 0.618033988749895f;
            float hueStep = 1f / paletteSize;

            for (int i = 1; i < paletteSize; i++)
            {
                float newHue = (h + (hueStep * i * goldenRatio)) % 1f;
                float newSat = Mathf.Clamp01(s + Random.Range(-0.2f, 0.2f));
                float newVal = Mathf.Clamp01(v + Random.Range(-0.1f, 0.1f));

                generatedPalette[i] = Color.HSVToRGB(newHue, newSat, newVal);
            }
        }

        private void GeneratePaletteWithSecondary(float h1, float s1, float v1, float h2, float s2, float v2)
        {
            generatedPalette[0] = mainColor;
            generatedPalette[1] = secondaryColor;

            float hueStep = Mathf.Abs(h2 - h1) / (paletteSize - 1);
            float satStep = (s2 - s1) / (paletteSize - 1);
            float valStep = (v2 - v1) / (paletteSize - 1);

            for (int i = 2; i < paletteSize; i++)
            {
                float t = (float)(i - 1) / (paletteSize - 2);
                float newHue = Mathf.Lerp(h1, h2, t) % 1f;
                float newSat = Mathf.Clamp01(Mathf.Lerp(s1, s2, t));
                float newVal = Mathf.Clamp01(Mathf.Lerp(v1, v2, t));

                generatedPalette[i] = Color.HSVToRGB(newHue, newSat, newVal);
            }
        }

        private void UpdateColorArrayString()
        {
            var sb = new StringBuilder();
            sb.AppendLine("Color[] colors = new Color[] {");

            for (int i = 0; i < generatedPalette.Length; i++)
            {
                var c = generatedPalette[i];
                sb.AppendLine($"    new Color({c.r}f, {c.g}f, {c.b}f, {c.a}f),");
            }

            sb.AppendLine("};");
            colorArrayString = sb.ToString();
        }

        private void CopyColorArrayToClipboard()
        {
            EditorGUIUtility.systemCopyBuffer = colorArrayString;
            Debug.Log("Color array copied to clipboard!");
        }

        private float WrapHue(float hue)
        {
            while (hue < 0f) hue += 1f;
            while (hue > 1f) hue -= 1f;
            return hue;
        }
    }
}