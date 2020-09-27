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
    class HerBurdenItem3 : Her_Burden
    {
        public static void Init()
        {
            HerBurden3 = new ItemDef
            {
                name = "HERBURDEN3",
                nameToken = "HERBURDEN_NAME3",
                pickupToken = "HERBURDEN_PICKUP",
                descriptionToken = "HERBURDEN_DESC",
                loreToken = "HERBURDEN_LORE",
                tier = ItemTier.Lunar,
                pickupIconPath = "@Her_Burden:Assets/Import/herburdenicon/itemIcon.png",
                pickupModelPath = "@Her_Burden:Assets/Import/herburden/her_burden.prefab",
                canRemove = true,
                hidden = false
            };
            AddTokens();
            AddLocation();
        }
        private static void AddTokens()
        {
            //AssetPlus is deprecated, so I switched it to use the current LanguageAPI
            LanguageAPI.Add("HERBURDEN_NAME", "Her Burden");
            LanguageAPI.Add("HERBURDEN_PICKUP", "Increase attack speed and decrease HP.\nAll item drops are now:<color=#307FFF>Her Burden</color>");
            LanguageAPI.Add("HERBURDEN_DESC", "Increase attack speed by 5% and decrease HP by 2.5%.\nAll item drops are now:<color=#307FFF>Her Burden</color>");
            LanguageAPI.Add("HERBURDEN_LORE", "None");

        }
        public static void AddLocation()
        {
            var rules = new ItemDisplayRuleDict(null);
            HerBurden3.itemIndex = ItemAPI.Add(new CustomItem(HerBurden3, rules));
        }
    }
}
