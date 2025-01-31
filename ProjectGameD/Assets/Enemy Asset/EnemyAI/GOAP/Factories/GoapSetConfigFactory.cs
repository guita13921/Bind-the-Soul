using CrashKonijn.Goap.Behaviours;
using CrashKonijn.Goap.Classes;
using CrashKonijn.Goap.Classes.Builders;
using CrashKonijn.Goap.Configs.Interfaces;
using CrashKonijn.Goap.Enums;
using CrashKonijn.Goap.Resolver;
using EnemyAI.GOAP.Actions;
using EnemyAI.GOAP.Goals;
using EnemyAI.GOAP.Sensors;
using EnemyAI.GOAP.Targets;
using EnemyAI.GOAP.WorldKeys;
using EnemyAI.GOPA.Actions;
using UnityEngine;

namespace EnemyAI.GOAP.Factories
{
    [RequireComponent(typeof(DependencyInjector))]
    public class GoapSetConfigFactory : GoapSetFactoryBase
    {

        private DependencyInjector injector;

        public override IGoapSetConfig Create(){
            injector = GetComponent<DependencyInjector>();
            GoapSetBuilder builder = new("LlamaSet");

            BuildGoals(builder);
            BuildAction(builder);
            BuildSensors(builder);

            return builder.Build();
        }

        private void BuildSensors(GoapSetBuilder builder)
        {
            builder.AddTargetSensor<WanderTargetSensor>()
                .SetTarget<WanderTargets>();

            builder.AddTargetSensor<PlayerTargetSensor>()
                .SetTarget<PlayerTarget>();
        }

        private void BuildAction(GoapSetBuilder builder)
        {
            builder.AddAction<WanderAction>()
            .SetTarget<WanderTargets>()
            .AddEffect<IsWandering>(EffectType.Increase)
            .SetBaseCost(5)
            .SetInRange(10);

            builder.AddAction<MeleeAction>()
            .SetTarget<PlayerTarget>()
            //.AddEffect<PlayerHealth>(EffectType.Decrease)
            .SetBaseCost(injector.AttackConfig.MeleeAttackCost)
            .SetInRange(injector.AttackConfig.SensorRadius);
        }

        private void BuildGoals(GoapSetBuilder builder)
        {
            builder.AddGoal<WanderGoal>()
            .AddCondition<IsWandering>(Comparison.GreaterThanOrEqual, 1);

            builder.AddGoal<KillPlayer>();
            //.AddCondition<PlayerHealth>(Comparison.SmallerThanOrEqual, 0);
        }
    }
}