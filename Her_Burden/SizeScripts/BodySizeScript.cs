using System.Linq;
using UnityEngine;

namespace CustomItemPlugin
{
    internal class BodySizeScript : MonoBehaviour
    {
        private static int globalIndexTick = 0;
        private int personalIndex;

        private void OnEnable()
        {
            personalIndex = globalIndexTick;
            globalIndexTick++;
        }

        private void OnDisable()
        {
            globalIndexTick--;
        }

        //This is passed the stack count from Inventory, and hands it to the PrefabSizeScript
        internal void UpdateStacks(int newStacks)
        {
            SizeHandoffManager.prefabSizeScripts.ElementAt(personalIndex).UpdateStacks(newStacks);
        }

        //This will override the display rules, since scale is now dynamically set
        internal void SetBodyMultiplier(string nameToken)
        {
            float bodySizeMultiplier;
            //If you need to add rules, model it after the existing ones below
            switch (nameToken)
            {
                case "CROCO_BODY_NAME":
                    bodySizeMultiplier = 10;
                    break;
                case "TOOLBOT_BODY_NAME":
                    bodySizeMultiplier = 10;
                    break;
                case "TREEBOT_BODY_NAME":
                    bodySizeMultiplier = 2;
                    break;
                //If there isn't a special rule, default to default
                default:
                    bodySizeMultiplier = 1;
                    break;
            }
            SizeHandoffManager.prefabSizeScripts.ElementAt(personalIndex).characterSizeMultiplier = bodySizeMultiplier;
        }
    }
}