namespace MagicVillageDash.Character.CharacterAnimator
{
    public interface IExpressionAnimator
    {
        void Excited(bool value);

        // Idle expressions (triggers)
        void Dig();
        void Scratch();
        void Smell();
        void Yawn();

        // Tap expressions
        void Bark();   // trigger
        void Howl();   // trigger
        void Lie(bool value);    // bool
        void Sleep(bool value);  // bool
    }
}