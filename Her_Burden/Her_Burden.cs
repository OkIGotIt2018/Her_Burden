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
using Random = System.Random;
using UnityRandom = UnityEngine.Random;
using Object = System.Object;
using UnityObject = UnityEngine.Object;

namespace Her_Burden
{
    [R2APISubmoduleDependency(nameof(ResourcesAPI))]
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("com.OkIgotIt.Her_Burden", "Her_Burden", "1.1.2")]
    [R2APISubmoduleDependency(nameof(ItemAPI), nameof(ItemDropAPI), nameof(LanguageAPI))]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]

    public class Her_Burden : BaseUnityPlugin
    {
        public static ItemDef HerBurden;
        /*public static ItemDef HerBurden2;
        public static ItemDef HerBurden3;
        public static ItemDef HerBurden4;
        public static ItemDef HerBurden5;
        public static ItemDef HerBurden6;*/
        public static ConfigEntry<bool> Hbisos { get; set; }
        public static ConfigEntry<bool> Hbpul { get; set; }
        public static ConfigEntry<int> Hbcpu { get; set; }
        public static ConfigEntry<float> Hbims { get; set; }
        public static ConfigEntry<float> Hbimssm { get; set; }
        public static ConfigEntry<float> Hbbuff { get; set; }
        public static ConfigEntry<float> Hbdebuff { get; set; }
        internal Her_Burden() { }
        internal static BepInEx.Logging.ManualLogSource log;
        //The Awake() method is run at the very start when the game is initialized.
        public void Awake()
        {
            log = Logger;
            Hbisos = Config.Bind<bool>("Her Burden Toggle", "Toggle Item Visibility", true, "Changes if Her Burden shows on the Survivor");
            Hbpul = Config.Bind<bool>("Her Burden Toggle", "Toggle Luck Effect", false, "Changes if luck effects chance to pickup Her Burden once you have one");
            Hbcpu = Config.Bind<int>("Her Burden Size", "Chance to change pickup to Her Burden", 100, "Chance to change other items to Her Burden on pickup once you have one");
            Hbims = Config.Bind<float>("Her Burden Size", "Max size of the item", 2, "Changes the max size of the item on the Survivor");
            Hbimssm = Config.Bind<float>("Her Burden Size", "Size Multiplier for the item", 0.049375f, "Changes the rate that the item size increases by");
            Hbbuff = Config.Bind<float>("Her Burden Stats Multiplier", "Buff", 1.05f, "Changes the increase of buff of the item per item exponentially");
            Hbdebuff = Config.Bind<float>("Her Burden Stats Multiplier", "Debuff", 0.975f, "Changes the decrease of debuff of the item per item exponentially");
            LanguageAPI.Add("HERBURDEN_NAME", "Her Burden");
            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Her_Burden.Resources.herburden"))
            {
                var bundle = AssetBundle.LoadFromStream(stream);
                var provider = new AssetBundleResourcesProvider("@Her_Burden", bundle);
                ResourcesAPI.AddProvider(provider);
            }
            HerBurdenItem.Init();
            /*HerBurdenItem2.Init();
            HerBurdenItem3.Init();
            HerBurdenItem4.Init();
            HerBurdenItem5.Init();
            HerBurdenItem6.Init();*/
            //maxHealth-moveSpeed
            //armor-regen
            //attackSpeed-maxHealth
            //moveSpeed-armor
            //damage-attackSpeed
            //regen-damage

            On.RoR2.GenericPickupController.GrantItem += (orig, self, body, inventory) =>
            {
                /*if (body.inventory.GetItemCount(HerBurden.itemIndex) > 0 && Util.CheckRoll(Hbcpu.Value, body.master) && Hbpul.Value == true)
                {
                    switch (Mathf.FloorToInt(UnityRandom.Range(0, 6)))
                    {
                        case 0:
                            self.pickupIndex = PickupCatalog.FindPickupIndex(HerBurden.itemIndex);
                            break;
                        case 1:
                            self.pickupIndex = PickupCatalog.FindPickupIndex(HerBurden2.itemIndex);
                            break;
                        case 2:
                            self.pickupIndex = PickupCatalog.FindPickupIndex(HerBurden3.itemIndex);
                            break;
                        case 3:
                            self.pickupIndex = PickupCatalog.FindPickupIndex(HerBurden4.itemIndex);
                            break;
                        case 4:
                            self.pickupIndex = PickupCatalog.FindPickupIndex(HerBurden5.itemIndex);
                            break;
                        case 5:
                            self.pickupIndex = PickupCatalog.FindPickupIndex(HerBurden6.itemIndex);
                            break;
                    }
                }
                if (body.inventory.GetItemCount(HerBurden.itemIndex) > 0 && Util.CheckRoll(Hbcpu.Value, body.master) && Hbpul.Value == false)
                {
                    switch (Mathf.FloorToInt(UnityRandom.Range(0, 6)))
                    {
                        case 0:
                            self.pickupIndex = PickupCatalog.FindPickupIndex(HerBurden.itemIndex);
                            break;
                        case 1:
                            self.pickupIndex = PickupCatalog.FindPickupIndex(HerBurden2.itemIndex);
                            break;
                        case 2:
                            self.pickupIndex = PickupCatalog.FindPickupIndex(HerBurden3.itemIndex);
                            break;
                        case 3:
                            self.pickupIndex = PickupCatalog.FindPickupIndex(HerBurden4.itemIndex);
                            break;
                        case 4:
                            self.pickupIndex = PickupCatalog.FindPickupIndex(HerBurden5.itemIndex);
                            break;
                        case 5:
                            self.pickupIndex = PickupCatalog.FindPickupIndex(HerBurden6.itemIndex);
                            break;
                    }
                }*/
                if (body.inventory.GetItemCount(HerBurden.itemIndex) > 0 && Util.CheckRoll(Hbcpu.Value, body.master) && Hbpul.Value == true)
                    self.pickupIndex = PickupCatalog.FindPickupIndex(HerBurden.itemIndex);
                if (body.inventory.GetItemCount(HerBurden.itemIndex) > 0 && Util.CheckRoll(Hbcpu.Value) && Hbpul.Value == false)
                    self.pickupIndex = PickupCatalog.FindPickupIndex(HerBurden.itemIndex);
                orig(self, body, inventory);

                //Handle the size change with scripts
                if (!body.gameObject.GetComponent<BodySizeScript>() && body.inventory.GetItemCount(HerBurden.itemIndex) > 0 && Hbisos.Value == true)
                {
                    body.gameObject.AddComponent<BodySizeScript>();
                    body.gameObject.GetComponent<BodySizeScript>().SetBodyMultiplier(body.baseNameToken);
                }
            };
            WhoKnows();
            On.RoR2.CharacterBody.Update += CharacterBody_Update;
            On.RoR2.CharacterMaster.OnBodyStart += CharacterMaster_OnBodyStart;
        }

        private void CharacterMaster_OnBodyStart(On.RoR2.CharacterMaster.orig_OnBodyStart orig, CharacterMaster self, CharacterBody body)
        {
            orig(self, body);
            if (self.playerCharacterMasterController)
            {
                if (!body.gameObject.GetComponent<BodySizeScript>() && body.inventory.GetItemCount(HerBurden.itemIndex) > 0 && Hbisos.Value == true)
                {
                    body.gameObject.AddComponent<BodySizeScript>();
                    body.gameObject.GetComponent<BodySizeScript>().SetBodyMultiplier(body.baseNameToken);
                }
            }
        }

        //This hook just updates the stack count
        private void CharacterBody_Update(On.RoR2.CharacterBody.orig_Update orig, CharacterBody self)
        {
            orig(self);
            if (self.gameObject.GetComponent<BodySizeScript>() && Hbisos.Value == true)
            {
                self.gameObject.GetComponent<BodySizeScript>().UpdateStacks(self.inventory.GetItemCount(HerBurden.itemIndex));
            }
        }

        public  void WhoKnows()
        {
            IL.RoR2.CharacterBody.RecalculateStats += (il) =>
            {
                ILCursor c = new ILCursor(il);
                ILLabel dioend = il.DefineLabel(); //end label

                try
                {
                    c.Index = 0;

                    // find the section that the code will be injected					
                    c.GotoNext(
                        MoveType.Before,
                        x => x.MatchLdarg(0), // 1388	0D5A	ldarg.0
                        x => x.MatchLdcI4(0x21), // 1389	0D5B	ldc.i4.s	0x21
                        x => x.MatchCallvirt<CharacterBody>("HasBuff") // 1390	0D5D	call	instance bool RoR2.CharacterBody::HasBuff(valuetype RoR2.BuffIndex)
                    );

                    if (c.Index != 0)
                    {
                        c.Index++;

                        // this block is just "If artifact isn't enabled, jump to dioend label". In an item case, this should be the part where you check if you have the items or not.
                        c.Emit(OpCodes.Ldarg_0);
                        c.EmitDelegate<Func<CharacterBody, bool>>((cb) =>
                        {
                            if (cb.master && cb.master.inventory)
                            {
                                int items = cb.master.inventory.GetItemCount(HerBurden.itemIndex);
                                /*int items2 = cb.master.inventory.GetItemCount(HerBurden2.itemIndex);
                                int items3 = cb.master.inventory.GetItemCount(HerBurden3.itemIndex);
                                int items4 = cb.master.inventory.GetItemCount(HerBurden4.itemIndex);
                                int items5 = cb.master.inventory.GetItemCount(HerBurden5.itemIndex);
                                int items6 = cb.master.inventory.GetItemCount(HerBurden6.itemIndex);*/
                if (items > 0/* || items2 > 0 || items3 > 0 || items4 > 0 || items5 > 0 || items6 > 0*/) return true;
                                return false;
                            }
                            return false;
                        });
                        c.Emit(OpCodes.Brfalse, dioend);

                        // this.maxHealth
                        c.Emit(OpCodes.Ldarg_0);
                        c.Emit(OpCodes.Ldarg_0);
                        c.Emit(OpCodes.Callvirt, typeof(CharacterBody).GetMethod("get_maxHealth")); // 1414 0DA3	call	instance float32 RoR2.CharacterBody::get_maxHealth()

                        // get the inventory count for the item, calculate multiplier, return a float value
                        // This is essentially `this.maxHealth *= multiplier;`
                        c.Emit(OpCodes.Ldarg_0);
                        c.EmitDelegate<Func<CharacterBody, float>>((cb) =>
                        {
                            float healthmultiplier = 1;
                            if (cb.master && cb.master.inventory)
                            {
                                int itemCount = cb.master.inventory.GetItemCount(HerBurden.itemIndex);
                                if (itemCount > 0)
                                {
                                    healthmultiplier *= Mathf.Pow(Hbbuff.Value, itemCount);
                                }
                                /*int itemCount2 = cb.master.inventory.GetItemCount(HerBurden3.itemIndex);
                                if (itemCount2 > 0)
                                {
                                    healthmultiplier *= Mathf.Pow(Hbdebuff.Value, itemCount2);
                                }*/
                            }
                            return healthmultiplier;
                        });
                        c.Emit(OpCodes.Mul);
                        c.Emit(OpCodes.Callvirt, typeof(CharacterBody).GetMethod("set_maxHealth", BindingFlags.Instance | BindingFlags.NonPublic));

                        // this.attackSpeed
                        c.Emit(OpCodes.Ldarg_0);
                        c.Emit(OpCodes.Ldarg_0);
                        c.Emit(OpCodes.Callvirt, typeof(CharacterBody).GetMethod("get_attackSpeed")); // 1426 0DC7	call	instance float32 RoR2.CharacterBody::get_attackSpeed()

                        // get the inventory count for the item, calculate multiplier, return a float value
                        // This is essentially `this.attackSpeed *= multiplier;`
                        c.Emit(OpCodes.Ldarg_0);
                        c.EmitDelegate<Func<CharacterBody, float>>((cb) =>
                        {
                            float attackSpeedmultiplier = 1;
                            /*if (cb.master && cb.master.inventory)
                            {
                                int itemCount = cb.master.inventory.GetItemCount(HerBurden3.itemIndex);
                                if (itemCount > 0)
                                {
                                    attackSpeedmultiplier *= Mathf.Pow(Hbbuff.Value, itemCount);
                                }
                                int itemCount2 = cb.master.inventory.GetItemCount(HerBurden5.itemIndex);
                                if (itemCount2 > 0)
                                {
                                    attackSpeedmultiplier *= Mathf.Pow(Hbdebuff.Value, itemCount2);
                                }
                            }*/
                            return attackSpeedmultiplier;
                        });
                        c.Emit(OpCodes.Mul);
                        c.Emit(OpCodes.Callvirt, typeof(CharacterBody).GetMethod("set_attackSpeed", BindingFlags.Instance | BindingFlags.NonPublic));

                        // this.moveSpeed
                        c.Emit(OpCodes.Ldarg_0);
                        c.Emit(OpCodes.Ldarg_0);
                        c.Emit(OpCodes.Callvirt, typeof(CharacterBody).GetMethod("get_moveSpeed")); // 1406	0D8A	call	instance float32 RoR2.CharacterBody::get_moveSpeed()

                        // get the inventory count for the item, calculate multiplier, return a float value
                        // This is essentially `this.moveSpeed *= multiplier;`
                        c.Emit(OpCodes.Ldarg_0);
                        c.EmitDelegate<Func<CharacterBody, float>>((cb) =>
                        {
                            float moveSpeedmultiplier = 1;
                            if (cb.master && cb.master.inventory)
                            {
                                /*int itemCount = cb.master.inventory.GetItemCount(HerBurden4.itemIndex);
                                if (itemCount > 0)
                                {
                                    moveSpeedmultiplier *= Mathf.Pow(Hbbuff.Value, itemCount);
                                }*/
                                int itemCount2 = cb.master.inventory.GetItemCount(HerBurden.itemIndex);
                                if (itemCount2 > 0)
                                {
                                    moveSpeedmultiplier *= Mathf.Pow(Hbdebuff.Value, itemCount2);
                                }
                            }
                            return moveSpeedmultiplier;
                        });
                        c.Emit(OpCodes.Mul);
                        c.Emit(OpCodes.Callvirt, typeof(CharacterBody).GetMethod("set_moveSpeed", BindingFlags.Instance | BindingFlags.NonPublic));

                        // this.armor
                        c.Emit(OpCodes.Ldarg_0);
                        c.Emit(OpCodes.Ldarg_0);
                        c.Emit(OpCodes.Callvirt, typeof(CharacterBody).GetMethod("get_armor")); // 1438 0DE8	call	instance float32 RoR2.CharacterBody::get_armor()

                        // get the inventory count for the item, calculate multiplier, return a float value
                        // This is essentially `this.armor *= multiplier;`
                        c.Emit(OpCodes.Ldarg_0);
                        c.EmitDelegate<Func<CharacterBody, float>>((cb) =>
                        {
                            float armormultiplier = 1;
                            /*if (cb.master && cb.master.inventory)
                            {
                                int itemCount = cb.master.inventory.GetItemCount(HerBurden2.itemIndex);
                                if (itemCount > 0)
                                {
                                    armormultiplier *= Mathf.Pow(Hbbuff.Value, itemCount);
                                }
                                int itemCount2 = cb.master.inventory.GetItemCount(HerBurden4.itemIndex);
                                if (itemCount2 > 0)
                                {
                                    armormultiplier *= Mathf.Pow(Hbdebuff.Value, itemCount2);
                                }
                            }*/
                            return armormultiplier;
                        });
                        c.Emit(OpCodes.Mul);
                        c.Emit(OpCodes.Callvirt, typeof(CharacterBody).GetMethod("set_armor", BindingFlags.Instance | BindingFlags.NonPublic));

                        // this.damage
                        c.Emit(OpCodes.Ldarg_0);
                        c.Emit(OpCodes.Ldarg_0);
                        c.Emit(OpCodes.Callvirt, typeof(CharacterBody).GetMethod("get_damage")); // 1444 0DFD	call	instance float32 RoR2.CharacterBody::get_damage()

                        // get the inventory count for the item, calculate multiplier, return a float value
                        // This is essentially `this.damage *= multiplier;`
                        c.Emit(OpCodes.Ldarg_0);
                        c.EmitDelegate<Func<CharacterBody, float>>((cb) =>
                        {
                            float damagemultiplier = 1;
                            /*if (cb.master && cb.master.inventory)
                            {
                                int itemCount = cb.master.inventory.GetItemCount(HerBurden5.itemIndex);
                                if (itemCount > 0)
                                {
                                    damagemultiplier *= Mathf.Pow(Hbbuff.Value, itemCount);
                                }
                                int itemCount2 = cb.master.inventory.GetItemCount(HerBurden6.itemIndex);
                                if (itemCount2 > 0)
                                {
                                    damagemultiplier *= Mathf.Pow(Hbdebuff.Value, itemCount2);
                                }
                            }*/
                            return damagemultiplier;
                        });
                        c.Emit(OpCodes.Mul);
                        c.Emit(OpCodes.Callvirt, typeof(CharacterBody).GetMethod("set_damage", BindingFlags.Instance | BindingFlags.NonPublic));

                        // this.regen
                        c.Emit(OpCodes.Ldarg_0);
                        c.Emit(OpCodes.Ldarg_0);
                        c.Emit(OpCodes.Callvirt, typeof(CharacterBody).GetMethod("get_regen")); // 1450 0E0F	call	instance float32 RoR2.CharacterBody::get_regen()

                        // get the inventory count for the item, calculate multiplier, return a float value
                        // This is essentially `this.regen *= multiplier;`
                        c.Emit(OpCodes.Ldarg_0);
                        c.EmitDelegate<Func<CharacterBody, float>>((cb) =>
                        {
                            float regenmultiplier = 1;
                            /*if (cb.master && cb.master.inventory)
                            {
                                int itemCount = cb.master.inventory.GetItemCount(HerBurden6.itemIndex);
                                if (itemCount > 0)
                                {
                                    regenmultiplier *= Mathf.Pow(Hbbuff.Value, itemCount);
                                }
                                int itemCount2 = cb.master.inventory.GetItemCount(HerBurden2.itemIndex);
                                if (itemCount2 > 0)
                                {
                                    regenmultiplier *= Mathf.Pow(Hbdebuff.Value, itemCount2);
                                }
                            }*/
                            return regenmultiplier;
                        });
                        c.Emit(OpCodes.Mul);
                        c.Emit(OpCodes.Callvirt, typeof(CharacterBody).GetMethod("set_regen", BindingFlags.Instance | BindingFlags.NonPublic));

                        c.MarkLabel(dioend); // end label
                    }

                }
                catch (Exception ex) { base.Logger.LogError(ex); }
            };
        }
    }
}