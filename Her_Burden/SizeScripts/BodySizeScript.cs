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

        //This is passed the stack count from Inventory, and hands it to the PrefabSizeScript
        internal void UpdateStacks(int newStacks)
        {
            SizeHandoffManager.prefabSizeScripts.ElementAt(personalIndex).UpdateStacks(newStacks);
        }
    }
}