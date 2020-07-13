using Unity.Transforms;
using Unity.Mathematics;
using Unity.Entities;

public class DestructionSystem : SystemBase
{
    float thresholdDistance = 2f;

    protected override void OnUpdate()
    {
        float3 playerPosition = (float3)GameManager.GetPlayerPosition();

        Entities.WithAll<EnemyTag>()
            .WithStructuralChanges()
            .ForEach((Entity enemy, ref Translation enemyPos) =>
            {
                playerPosition.y = enemyPos.Value.y;

                if (math.distance(enemyPos.Value, playerPosition) <= thresholdDistance)
                {
                    //FXManager.Instance.CreateExplosion(enemyPos.Value);
                    //FXManager.Instance.CreateExplosion(playerPosition);
                    //GameManager.EndGame();
                    EntityManager.DestroyEntity(enemy);
                
                    //PostUpdateCommands.DestroyEntity(enemy);
                }

                //float3 enemyPosition = enemyPos.Value;

                //Entities.WithAll<BulletTag>().ForEach((Entity bullet, ref Translation bulletPos) =>
                //{
                //    if (math.distance(enemyPosition, bulletPos.Value) <= thresholdDistance)
                //    {
                //        //PostUpdateCommands.DestroyEntity(enemy);
                //        //PostUpdateCommands.DestroyEntity(bullet);

                //        //FXManager.Instance.CreateExplosion(enemyPosition);
                //        //GameManager.AddScore(1);
                //    }
                //}).ScheduleParallel();
            }).Run();
    }
}
