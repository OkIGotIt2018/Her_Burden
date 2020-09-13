using UnityEngine;

namespace CustomItemPlugin
{
    internal class PrefabSizeScript : MonoBehaviour
    {
        private static Vector3 originalScale = new Vector3(1f, 1f, 1f);
        //These could be set in a config if you wanted to
        internal static float maxSizeMultiplier = 10f, stackSizeMultiplier = 0.5f;
        private Transform thisTransform;

        private void OnEnable()
        {
            thisTransform = gameObject.transform;
            SizeHandoffManager.prefabSizeScripts.Add(this);
        }

        //This handles all of the item size changes, and is called by BodySizeScript
        internal void UpdateStacks(int newStacks)
        {
            float testSizeMultiplier = 1 + (newStacks * stackSizeMultiplier);
            if(testSizeMultiplier <= maxSizeMultiplier)
            {
                thisTransform.localScale = originalScale * testSizeMultiplier;
            }
            else
            {
                thisTransform.localScale = originalScale * maxSizeMultiplier;
            }
        }
    }
}