using Unity.Burst;
using Unity.Entities;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;
namespace IT4080C
{
    [UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
    partial struct ShootManager : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BulletSpawner>();
        }
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            // Ensure NetworkTime singleton exists before continuing
            state.RequireForUpdate<Unity.NetCode.NetworkTime>();
            
            NetworkTime networkTime = SystemAPI.GetSingleton<NetworkTime>();
            var prefab = SystemAPI.GetSingleton<BulletSpawner>().Bullet;
            EntityCommandBuffer ecb = new
                EntityCommandBuffer(Unity.Collections.Allocator.Temp);
            foreach ((
                         var playerInput,
                         var localTransform,
                         var ghostOwner)
                     in SystemAPI.Query<
                         RefRO<CubeInput>,
                         RefRO<LocalTransform>,
                         RefRO<GhostOwner>>().WithAll<Simulate>())
            {
                if (networkTime.IsFirstTimeFullyPredictingTick)
                {
                    if (playerInput.ValueRO.shoot.IsSet)
                    {
                        Debug.LogWarning("Shoot Input");
                        Entity bulletEntity = ecb.Instantiate(prefab);
                        ecb.SetComponent(bulletEntity, LocalTransform.FromPositionRotation(localTransform.ValueRO.Position, localTransform.ValueRO.Rotation));
                        ecb.SetComponent(bulletEntity, new GhostOwner { NetworkId = ghostOwner.ValueRO.NetworkId });
                    }
                }
            }
            ecb.Playback(state.EntityManager);
        }
        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }
    }
}