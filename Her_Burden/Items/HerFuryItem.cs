using BepInEx;
using BepInEx.Configuration;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using R2API;
using R2API.Utils;
using RoR2;
using System;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace Her_Burden
{
    class HerFuryItem : Her_Burden
    {
        public static void Init()
        {
            HerFury = new ItemDef
            {
                name = "HERFURY",
                nameToken = "HERFURY_NAME",
                pickupToken = "HERFURY_PICKUP",
                descriptionToken = "HERFURY_DESC",
                loreToken = "HERFURY_LORE",
                tier = ItemTier.Lunar,
                pickupIconPath = "@Her_Burden:Assets/Import/herburdenicon/HushreallyredItemIcon.png",
                pickupModelPath = "@Her_Burden:Assets/Import/herburden/" + Hbiiv.Value + "reallyredher_burden.prefab",
                canRemove = true,
                hidden = false
            };
            AddTokens();
            AddLocation();
        }
        private static void AddTokens()
        {
            //AssetPlus is deprecated, so I switched it to use the current LanguageAPI
            LanguageAPI.Add("HERFURY_NAME", "Her Fury");
            LanguageAPI.Add("HERFURY_PICKUP", "Increase attack speed and decrease HP.\nAll item drops are now variants of: <color=#307FFF>Her Burden</color>");
            LanguageAPI.Add("HERFURY_DESC", "Increase attack speed by 5% and decrease HP by 2.5%.\nAll item drops are now variants of: <color=#307FFF>Her Burden</color>");
            LanguageAPI.Add("HERFURY_LORE", "None");

        }
        public static void AddLocation()
        {
            GameObject followerPrefab = Resources.Load<GameObject>("@Her_Burden:Assets/Import/herburden/" + Hbiiv.Value + "reallyredher_burden.prefab");
            followerPrefab.AddComponent<PrefabSizeScript>();
            var rules = new ItemDisplayRuleDict(null);
            HerFury.itemIndex = ItemAPI.Add(new CustomItem(HerFury, rules));
        }
    }
}
