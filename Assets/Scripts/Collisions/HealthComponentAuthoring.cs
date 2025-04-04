using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

namespace IT4080C
{
    /// <summary>
    /// Flag component to mark an entity as a Bullet.
    /// </summary>
    [GhostComponent]
    public struct HealthComponent : IComponentData
    {
        [GhostField] public float CurrentHealth;
        [GhostField] public float MaxHealth;
        [GhostField] public float ownerNetworkID;
        [GhostField] public int kills;
        [GhostField] public int deaths;
    }

    /// <summary>
    /// The authoring component for the Bullet.
    /// </summary>
    [DisallowMultipleComponent]
    public class HealthComponentAuthoring : MonoBehaviour
    {
        class Baker : Baker<HealthComponentAuthoring>
        {
            public override void Bake(HealthComponentAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent<HealthComponent>(entity, new HealthComponent
                {
                    CurrentHealth = 100f,
                    MaxHealth = 100f,
                    ownerNetworkID = -1f,
                    kills = 0,
                    deaths = 0
                });
            }
        }
    }
}
