// Scripts/Entities/Enemy.cs
using UnityEngine;

public class Enemy : Entity
{
    [SerializeField] private EnemyData data;

    public EnemyData Data => data;
    public EnemyIntent CurrentIntent { get; private set; }

    private int _turnIndex = 0;

    public void Initialize(EnemyData enemyData)
    {
        data = enemyData;
        maxHealth = data.MaxHealth;
        CurrentHealth = maxHealth;
        gameObject.name = data.EnemyName;
        CalculateNextIntent();
    }

    public void CalculateNextIntent()
    {
        CurrentIntent = data.GetIntent(_turnIndex);
    }

    public void ExecuteIntent(Player target)
    {
        switch (CurrentIntent.Type)
        {
            case IntentType.Attack:
                int dmg = CurrentIntent.Value;
                // Güç bonusu ekle
                dmg += GetEffectStacks<StrengthEffect>();
                target.TakeDamage(dmg);
                break;
            case IntentType.Block:
                AddBlock(CurrentIntent.Value);
                break;
            case IntentType.Buff:
                ApplyEffect(new StrengthEffect(CurrentIntent.Value));
                break;
        }

        _turnIndex++;
        ProcessEffectsOnTurnStart();
        CalculateNextIntent();
    }

    protected override void OnDeath()
    {
        base.OnDeath();
        // Reward sistemi tetiklenir
        CombatManager.Instance.OnEnemyDied(this);
    }
}