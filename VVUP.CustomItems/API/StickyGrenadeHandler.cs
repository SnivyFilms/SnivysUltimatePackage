/*using System;
using Exiled.API.Features;
using InventorySystem.Items.ThrowableProjectiles;
using Mirror;
using UnityEngine;

namespace VVUP.CustomItems.API
{
    public class StickyGrenadeHandler : MonoBehaviour
    {
        private bool initialized;
        public GameObject Owner { get; private set; }
        public EffectGrenade Grenade { get; private set; }
        public void Init(GameObject owner, ThrownProjectile grenade)
        {
            Owner = owner;
            Grenade = (EffectGrenade)grenade;
            initialized = true;
        }

        private void OnCollisionEnter(Collision collision)
        {
            try
            {
                if (!initialized)
                    return;
                if (Owner == null)
                    Log.Error($"Owner is null!");
                if (Grenade == null)
                    Log.Error("Grenade is null!");
                if (collision is null)
                    Log.Error("wat");
                if (!collision.collider)
                    Log.Error("water");
                if (collision.collider.gameObject == null)
                    Log.Error("pepehm");
                if (collision.collider.gameObject == Owner || collision.collider.gameObject.TryGetComponent<EffectGrenade>(out _))
                    return;

                Rigidbody rb = Grenade.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.isKinematic = true;
                    rb.velocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
    
                    Grenade.transform.SetParent(collision.transform);
                    
                    Grenade.transform.position = collision.contacts[0].point;
                    
                    Collider grenadeCollider = Grenade.GetComponent<Collider>();
                    if (grenadeCollider != null)
                        grenadeCollider.enabled = false;
                }
            }
            catch (Exception exception)
            {
                Log.Error($"{nameof(StickyGrenadeHandler)} error:\n{exception}");
                Destroy(this);
            }
        }
    }
}*/
