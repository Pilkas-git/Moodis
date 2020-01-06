using Android.Content;
using Android.Graphics.Drawables;
using Android.Support.Constraints;
using Android.Views;

namespace Moodis.Extensions
{
    public static class AnimationExtension
    {
        private const int EXIT_FADE_DURATION = 3500;
        private const int ENTER_FADE_DURATION = 10;

        public static void AnimateBackground(View view)
        {
            /*AnimationDrawable animationDrawable;
            var constraintLayout = (ConstraintLayout)view;
            animationDrawable = (AnimationDrawable)constraintLayout.Background;
            animationDrawable.SetEnterFadeDuration(ENTER_FADE_DURATION);
            animationDrawable.SetExitFadeDuration(EXIT_FADE_DURATION);
            animationDrawable.Start();*/
        }

    }
}