namespace MagicVillageDash.Character.CharacterAnimator
{
    public interface IDeathAnimator
    {
        void Die();          
        void Revive();       
        void SetDead(bool isDead); 
    }
}