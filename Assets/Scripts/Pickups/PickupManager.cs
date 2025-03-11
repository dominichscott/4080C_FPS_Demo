using IT4080C;
using Unity.Burst;
using Unity.Entities;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
partial struct PickupManager : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PickupSpawner>();
        state.RequireForUpdate<NetworkTime>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var prefab = SystemAPI.GetSingleton<PickupSpawner>().PickupObjAsEnt;
        
        EntityCommandBuffer ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
        NetworkTime networkTime = SystemAPI.GetSingleton<NetworkTime>();

        foreach ((
            var pickupSpawner,
            var localTransform)
            in SystemAPI.Query<
            RefRW<PickupSpawner>,
            RefRO<LocalTransform>>().WithAll<Simulate>())
        {

            if (networkTime.IsFirstTimeFullyPredictingTick)
            {
                if (pickupSpawner.ValueRO.hasObject != 1)
                {
                    
                    Debug.LogWarning("Spawn HP Obj 2");
                    Entity hpBoxEntity = ecb.Instantiate(prefab);


                    ecb.SetComponent(hpBoxEntity, LocalTransform.FromPositionRotation(localTransform.ValueRO.Position, localTransform.ValueRO.Rotation));

                    pickupSpawner.ValueRW.hasObject = 1;
                    pickupSpawner.ValueRW.timer = 10f;
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
