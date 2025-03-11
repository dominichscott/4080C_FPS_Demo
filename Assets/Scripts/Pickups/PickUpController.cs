using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Windows;
namespace IT4080C
{
    [UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
    public partial struct PickUpController : ISystem
    {
        public float yDegrees;
        public float rSpeed;
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            yDegrees = 0f;
            rSpeed = 1f;
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            EntityCommandBuffer ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

            foreach ((
                RefRW<LocalTransform> localTransform,
                RefRW<HPBox> hpBox,
                Entity entity)
                in SystemAPI.Query<
                    RefRW<LocalTransform>,
                    RefRW<HPBox>>().WithEntityAccess().WithAll<Simulate>())
            {


                //change movement to the direction of player
                //rotate object

                
                yDegrees += rSpeed;

                Quaternion rRot = Quaternion.Euler(0, yDegrees, 0);

                localTransform.ValueRW.Rotation = rRot;


                if (state.World.IsServer())
                {

                    if (hpBox.ValueRO.destroy)
                    {
                        Debug.Log("Destroying HPPickup");
                        ecb.DestroyEntity(entity);
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

