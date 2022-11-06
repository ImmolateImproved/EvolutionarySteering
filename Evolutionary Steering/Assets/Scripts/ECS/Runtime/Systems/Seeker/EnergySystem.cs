using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

public struct DebugInfo
{
    public int id;
    public int enemyId;
    public float type;
    public float targetType;
    public float foodPrefBefore;
    public float currentFoodPref;
    public float targetTypeMinusFoodPref;
    public float coef;
    public int targetCount;
    public float energyBefore;
    public float energyAfter;
}

[BurstCompile]
[UpdateInGroup(typeof(SimulationSystemGroup), OrderLast = true)]
public partial struct EnergySystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {

    }

    public void OnDestroy(ref SystemState state)
    {

    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var dt = SystemAPI.Time.DeltaTime;

        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        var colorLookup = SystemAPI.GetComponentLookup<URPMaterialPropertyBaseColor>();

        var energyLookup = SystemAPI.GetComponentLookup<Energy>(true);

        new TargetInDistanceJob
        {
            energyLookup = energyLookup

        }.ScheduleParallel();

        new HealthJob
        {
            ecb = ecb.AsParallelWriter(),
            dt = dt

        }.ScheduleParallel();

        new HealthViewJob
        {
            colorLookup = colorLookup

        }.ScheduleParallel();
    }

    //[BurstCompile]
    [WithAll(typeof(TargetInRange))]
    partial struct TargetInDistanceJob : IJobEntity
    {
        [ReadOnly]
        [NativeDisableContainerSafetyRestriction]
        public ComponentLookup<Energy> energyLookup;

        public void Execute(Entity e, ref Energy energy, ref TargetSeeker targetSeeker, in UnitType unitType)
        {
            var targetType = targetSeeker.targetType;
            var foodPrefBefore = targetSeeker.foodPreference;

            var coef = (targetType - targetSeeker.foodPreference) / math.abs(targetSeeker.foodPreference) + 0.01f;
            if (math.abs(coef) > 1f)
            {
                coef = 1f / coef;
            }

            targetSeeker.foodPreference = math.max(targetSeeker.foodPreference + coef, 0);

            //if (targetType < 9)
            //{
            //    Debug.LogError(targetType);
            //    return;
            //}
            if (math.abs(coef) < 1f)
            {
                coef = 1f - coef;
            }
            var energyBefore = energy.current;

            energy.current += energy.foodDigestion * coef * energyLookup[targetSeeker.target].current;

            energy.current = math.clamp(energy.current, 0, energy.max);

            var debugInfo = new DebugInfo
            {
                id = e.Index,
                enemyId = targetSeeker.target.Index,
                foodPrefBefore = foodPrefBefore,
                currentFoodPref = targetSeeker.foodPreference,
                targetType = targetType,
                targetTypeMinusFoodPref = targetType - targetSeeker.foodPreference,
                coef = coef,
                targetCount = targetSeeker.targetCount,
                energyAfter = energy.current,
                energyBefore = energyBefore,
                type = unitType.value
            };

            var json = JsonUtility.ToJson(debugInfo, true);

            //Debug.Log(json);
        }
    }

    [BurstCompile]
    partial struct HealthJob : IJobEntity
    {
        public EntityCommandBuffer.ParallelWriter ecb;

        public float dt;

        public void Execute(Entity e, [ChunkIndexInQuery] int chunkIndex, ref Energy energy)
        {
            energy.current -= energy.decreasePerSeconds * dt;

            if (energy.current <= 0)
            {
                ecb.DestroyEntity(chunkIndex, e);
            }
        }
    }

    [BurstCompile]
    partial struct HealthViewJob : IJobEntity
    {
        [NativeDisableParallelForRestriction]
        public ComponentLookup<URPMaterialPropertyBaseColor> colorLookup;

        public void Execute(in DynamicBuffer<Child> children, in Energy health, in TargetSeeker seeker)
        {
            if (!health.colorChange)
            {
                return;
            }

            for (int i = 0; i < children.Length; i++)
            {
                var color = (Vector4)Color.Lerp(Color.red, health.fullHpColor, seeker.foodPreference / 80f);
                colorLookup[children[i].Value] = new URPMaterialPropertyBaseColor { Value = color };
            }
        }
    }
}