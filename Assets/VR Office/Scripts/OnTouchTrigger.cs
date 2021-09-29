using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace ChiliGames
{
    //Simple On Trigger Enter with a cooldown
    public class OnTouchTrigger : MonoBehaviour
    {
        public UnityEvent onTriggerEnter;
        public float cooldown;
        bool ready = true;
        private void OnTriggerEnter(Collider other)
        {
            if (!ready) return;
            onTriggerEnter.Invoke();
            if (cooldown != 0)
                StartCoroutine(WaitCooldown());
        }

        IEnumerator WaitCooldown()
        {
            ready = false;
            yield return new WaitForSeconds(cooldown);
            ready = true;
        }
    }
}
