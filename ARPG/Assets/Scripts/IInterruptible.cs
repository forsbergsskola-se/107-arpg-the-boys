public interface IInterruptible
{
    void Interrupt();

    public enum AttackState
    {
        NoAttack,
        LightAttack,
        HeavyAttack,
        Guard
    }
    
    // public void ChangeAttackState(AttackState attackState)
    // {
    //     CurrentAttackState = attackState;
    // }
    AttackState CurrentAttackState { get; set; }
}