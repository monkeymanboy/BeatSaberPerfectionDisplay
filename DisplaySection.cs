using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace PerfectionDisplay
{
    class DisplaySection : MonoBehaviour
    {
        TextMeshPro scoreMesh;
        public string title;
        public string color;

        void Awake()
        {
            scoreMesh = this.gameObject.AddComponent<TextMeshPro>();
            scoreMesh.text = "";
            scoreMesh.fontSize = 3;
            scoreMesh.lineSpacing = -25f;
            scoreMesh.lineSpacingAdjustment = -25f;
            scoreMesh.enableAutoSizing = false;
            scoreMesh.paragraphSpacing = -25f;
            scoreMesh.color = Color.white;
            scoreMesh.font = Resources.Load<TMP_FontAsset>("Teko-Medium SDF No Glow");
            scoreMesh.alignment = TextAlignmentOptions.Center;
        }
        public void UpdateText(int score, string percent)
        {
            string text = "<color=" + color + ">" + title+"\n";
            if (PerfectDisplay.showNumbers) text += score + "\n";
            if (PerfectDisplay.showPercent) text += percent + "%\n";
            scoreMesh.text = text;
            scoreMesh.ForceMeshUpdate();
        }
        public void UpdatePosition(float x)
        {
            transform.localPosition = PerfectDisplay.displayPosition+new Vector3(x,0,0);
        }

        public float GetWidth()
        {
            return scoreMesh.GetRenderedValues().x+0.1f;
        }
    }
}
