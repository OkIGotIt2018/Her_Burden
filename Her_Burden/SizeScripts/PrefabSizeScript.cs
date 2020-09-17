using UnityEngine;

namespace CustomItemPlugin
{
    internal class PrefabSizeScript : MonoBehaviour
    {
        //This Vector will need to match the generalScale used in AddLocation
        private Vector3 originalScale;
        private Vector3 newScale;
        internal float characterSizeMultiplier;
        //These could be set in a config if you wanted to
        internal static float maxSizeMultiplier = 2f, stackSizeMultiplier = 0.049375f;

        private void OnEnable()
        {
            originalScale = new Vector3(.0125f, .0125f, .0125f);
            newScale = new Vector3(0.05f, 0.05f, 0.05f);
            characterSizeMultiplier = 1f;
            SizeHandoffManager.prefabSizeScripts.Add(this);
        }

        private void Update()
        {
            transform.localScale = newScale;
        }

        private void OnDisable()
        {
            SizeHandoffManager.prefabSizeScripts.Remove(this);
        }

        //This handles all of the item size changes, and is called by BodySizeScript
        internal void UpdateStacks(int newStacks)
        {
            float testSizeMultiplier = 1 + (newStacks * stackSizeMultiplier);
            if (testSizeMultiplier <= maxSizeMultiplier)
            {
                newScale = originalScale * characterSizeMultiplier * testSizeMultiplier;
            }
            else
            {
                newScale = originalScale * characterSizeMultiplier * maxSizeMultiplier;
            }
        }
    }
}