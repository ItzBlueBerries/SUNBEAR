﻿using HarmonyLib;
using Il2CppInterop.Runtime;
using MelonLoader;
using SUNBEAR.Assist;
using SUNBEAR.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUNBEAR.Data.Largos
{
    internal class Phosphor
    {
        public static void Load(string sceneName)
        {
            switch (sceneName)
            {
                case "GameCore":
                    {
                        SlimeDefinition largoDefinition = Get<SlimeDefinition>("SunBearPhosphor");
                        SlimeDefinition primaryDef = largoDefinition.BaseSlimes[0];
                        SlimeDefinition secondaryDef = largoDefinition.BaseSlimes[1];
                        string largoName = "Sun Bear Phosphor Largo";

                        LargoHelper.PresetLargo(largoName, largoDefinition);

                        #region SUN_BEAR_PHOSPHOR_LARGO
                        // APPEARANCE
                        SlimeAppearance largoAppearance = UnityEngine.Object.Instantiate(primaryDef.AppearancesDefault[0]);
                        SlimeAppearanceApplicator slimeAppearanceApplicator = largoDefinition.prefab.GetComponent<SlimeAppearanceApplicator>();
                        largoAppearance.name = primaryDef.AppearancesDefault[0].name + secondaryDef.AppearancesDefault[0].name;
                        largoAppearance._dependentAppearances = new SlimeAppearance[] { primaryDef.AppearancesDefault[0], secondaryDef.AppearancesDefault[0] };
                        slimeAppearanceApplicator.Appearance = largoAppearance;
                        slimeAppearanceApplicator.SlimeDefinition = largoDefinition;

                        // REST OF APPEARANCE
                        foreach (SlimeAppearanceStructure secondaryStruct in secondaryDef.AppearancesDefault[0].Structures)
                        {
                            if (secondaryStruct.SupportsFaces || secondaryStruct.Element.Type == SlimeAppearanceElement.ElementType.FACE || secondaryStruct.Element.Name.Contains("Face", StringComparison.OrdinalIgnoreCase))
                                continue;

                            if (secondaryStruct.Element.Type == SlimeAppearanceElement.ElementType.BODY || secondaryStruct.Element.Name.Contains("Body", StringComparison.OrdinalIgnoreCase))
                                continue;

                            if (secondaryStruct.Element.Type == SlimeAppearanceElement.ElementType.EARS || secondaryStruct.Element.Name.Contains("Ears", StringComparison.OrdinalIgnoreCase))
                                continue;

                            largoAppearance.Structures = largoAppearance.Structures.ToArray().AddToArray(new SlimeAppearanceStructure(secondaryStruct));
                        }

                        Material secondaryMat = secondaryDef.AppearancesDefault[0].Structures.TryGetBody().DefaultMaterials[0];

                        Material largoMaterial = UnityEngine.Object.Instantiate(secondaryMat);
                        largoMaterial.hideFlags |= HideFlags.HideAndDontSave;
                        largoMaterial.name = largoAppearance.name + "_Body";
                        largoMaterial.SetTexture("_ColorMask", LocalAssets.maskSunBearMulticolorGreen);
                        largoMaterial.SetColor("_GreenTopColor", secondaryMat.GetColor("_TopColor").AdjustBrightness(0.3f));
                        largoMaterial.SetColor("_GreenMiddleColor", secondaryMat.GetColor("_MiddleColor").AdjustBrightness(0.3f));
                        largoMaterial.SetColor("_GreenBottomColor", secondaryMat.GetColor("_BottomColor").AdjustBrightness(0.3f));

                        List<string> shaderKeywords = largoMaterial.GetShaderKeywords().ToList();
                        shaderKeywords.Add("_BODYCOLORING_MULTI");
                        shaderKeywords.Remove("_BODYCOLORING_DEFAULT");
                        largoMaterial.SetShaderKeywords(shaderKeywords.ToArray());

                        Material earsMaterial = UnityEngine.Object.Instantiate(secondaryMat);
                        earsMaterial.hideFlags |= HideFlags.HideAndDontSave;
                        earsMaterial.name = largoAppearance.name + "_Ears";
                        List<string> earsMatShaderKeywords = earsMaterial.GetShaderKeywords().ToList();
                        earsMatShaderKeywords.Remove("_ENABLEPHOSCORE_ON");
                        earsMaterial.SetShaderKeywords(earsMatShaderKeywords.ToArray());

                        largoAppearance.Structures[0].DefaultMaterials[0] = largoMaterial;
                        largoAppearance.Structures[2].DefaultMaterials[0] = earsMaterial;
                        largoAppearance.Structures.FirstOrDefault(x =>
                            x.Element.Type == SlimeAppearanceElement.ElementType.FACE_ATTACH ||
                            x.Element.Name.Contains("Antennae", StringComparison.OrdinalIgnoreCase)).DefaultMaterials[0] = earsMaterial;
                        largoAppearance.Structures.TryGetWings().DefaultMaterials[0] = earsMaterial;

                        largoAppearance._splatColor = secondaryDef.AppearancesDefault[0].SplatColor;
                        largoAppearance._colorPalette = secondaryDef.AppearancesDefault[0].ColorPalette;

                        largoDefinition.AppearancesDefault = new SlimeAppearance[] { largoAppearance };
                        largoDefinition.prefab.hideFlags |= HideFlags.HideAndDontSave;
                        #endregion
                        break;
                    }
            }
        }
    }
}