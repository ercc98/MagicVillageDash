using ErccDev.Foundation.Core.Tutorial;
using UnityEngine;

namespace MagicVillageDash.Tutorial
{
    [RequireComponent(typeof(Collider))]
    public sealed class TutorialTriggerStep : TutorialTriggerStepBase
    {
        protected override void Awake()
        {
            base.Awake();
        }

        protected override void OnTriggerEnter(Collider other)
        {
            base.OnTriggerEnter(other);
        }
    }
}