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
    class HerTorporItem : Her_Burden
    {
        public static void Init()
        {
            HerTorpor = new ItemDef
            {
                name = "HERTORPOR",
                nameToken = "HERTORPOR_NAME",
                pickupToken = "HERTORPOR_PICKUP",
                descriptionToken = "HERTORPOR_DESC",
                loreToken = "HERTORPOR_LORE",
                tier = ItemTier.Lunar,
                pickupIconPath = "@Her_Burden:Assets/Import/herburdenicon/HushroyalblueItemIcon.png",
                pickupModelPath = "@Her_Burden:Assets/Import/herburden/" + Hbiiv.Value + "royalblueher_burden.prefab",
                canRemove = true,
                hidden = false
            };
            AddTokens();
            AddLocation();
        }
        private static void AddTokens()
        {
            //AssetPlus is deprecated, so I switched it to use the current LanguageAPI
            LanguageAPI.Add("HERTORPOR_NAME", "Her Torpor");
            LanguageAPI.Add("HERTORPOR_PICKUP", "Increase regen and decrease attack speed.\nAll item drops are now variants of: <color=#307FFF>Her Burden</color>");
            LanguageAPI.Add("HERTORPOR_DESC", "Increase regen by 5% and decrease attack speed by 2.5%.\nAll item drops are now variants of: <color=#307FFF>Her Burden</color>");
            LanguageAPI.Add("HERTORPOR_LORE", "None");

        }
        public static void AddLocation()
        {
            GameObject followerPrefab = Resources.Load<GameObject>("@Her_Burden:Assets/Import/herburden/" + Hbiiv.Value + "royalblueher_burden.prefab");
            followerPrefab.AddComponent<PrefabSizeScript>();
            var rules = new ItemDisplayRuleDict(null);
            HerTorpor.itemIndex = ItemAPI.Add(new CustomItem(HerTorpor, rules));
        }
    }
}
