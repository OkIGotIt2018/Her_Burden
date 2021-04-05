using BepInEx;
using BepInEx.Configuration;
using BetterAPI;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using System;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace Her_Burden
{
    class HerGambleEquipment : Her_Burden
    {
        private static String HERGAMBLE_PICKUP;
        private static String HERGAMBLE_DESC;
        private static String HERGAMBLE_LORE;
        public static void Init()
        {
            var Hgcolor = ColorCatalog.ColorIndex.LunarItem;
            if (!Hbdbt.Value)
                Hgcolor = ColorCatalog.ColorIndex.Equipment;
            AddTokens();
            HerGamble = ScriptableObject.CreateInstance<EquipmentDef>();
            HerGamble.name = "HERGAMBLE";
            HerGamble.nameToken = "Her Gamble";
            HerGamble.pickupToken = HERGAMBLE_PICKUP;
            HerGamble.descriptionToken = HERGAMBLE_DESC;
            HerGamble.loreToken = HERGAMBLE_LORE;
            HerGamble.isLunar = Hbdbt.Value;
            HerGamble.colorIndex = Hgcolor;
            HerGamble.pickupIconSprite = Her_Burden.bundle.LoadAsset<Sprite>(Hbiiv.Value + "ItemIcon");
            HerGamble.pickupModelPrefab = Her_Burden.bundle.LoadAsset<GameObject>("her_gamble");
            HerGamble.canDrop = true;
            HerGamble.cooldown = 60f;
            ExperimentalBuff = ScriptableObject.CreateInstance<BuffDef>();
            ExperimentalBuff.name = "ExperimentalBuff";
            ExperimentalBuff.iconSprite = Her_Burden.bundle.LoadAsset<Sprite>(Hbiiv.Value + "ItemIcon");
            Buffs.Add(ExperimentalBuff);
            ExperimentalDeBuff = ScriptableObject.CreateInstance<BuffDef>();
            ExperimentalDeBuff.name = "ExperimentalDeBuff";
            ExperimentalDeBuff.iconSprite = Her_Burden.bundle.LoadAsset<Sprite>(Hbiiv.Value + "ItemIcon");
            Buffs.Add(ExperimentalDeBuff);
            AddLocation();
        }
        private static void AddTokens()
        {
            if (Hbdbt.Value)
            {
                HERGAMBLE_PICKUP = "An equipment that gambles your stats";
                HERGAMBLE_DESC = "An equipment that gambles your stats that come from Her Burden Variants";
            }
            if (!Hbdbt.Value)
            {
                HERGAMBLE_PICKUP = "Has a chance to double your stats";
                HERGAMBLE_DESC = "Has a chance to double your stats that come from Her Burden Variants";
            }
            HERGAMBLE_LORE = "None";

        }
        public static void AddLocation()
        {
        }
    }
}
