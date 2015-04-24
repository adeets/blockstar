using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Views.Animations;
using System;

namespace com.axxies.blockstar
{
    [Activity(MainLauncher = true, ScreenOrientation = ScreenOrientation.Portrait)]
    internal sealed class IntroActivity : BaseActivity, View.IOnClickListener
    {
        private View m_startButton;
        private View m_optionsButton;
        private View m_background;
        private Animation m_flickerAnimation;
        private Animation m_fadeOutAnimation;
        private Animation m_slideLeftAnimation;
        private Animation m_slideRightAnimation;
        private EventHandler<Animation.AnimationEndEventArgs> m_eventAnimationEnded;

        /// <summary>Called when the Activity is created or the orientation changes.</summary>
        protected override void OnCreate(Bundle bundle)
        {
            // Always call base class.
            base.OnCreate(bundle);

            // Set the view to the view we defined in screen_intro.xml.
            SetContentView(Resource.Layout.screen_intro);

            // Initialize our view variables.
            m_startButton = FindViewById(Resource.Id.start_button);
            m_optionsButton = FindViewById(Resource.Id.option_button);
            m_background = FindViewById(Resource.Id.intro_background);

            // Initialize our animation variables.
            m_flickerAnimation = AnimationUtils.LoadAnimation(this, Resource.Animation.flicker);
            m_fadeOutAnimation = AnimationUtils.LoadAnimation(this, Resource.Animation.fade_out);
            m_slideLeftAnimation = AnimationUtils.LoadAnimation(this, Resource.Animation.slide_right);
            m_slideRightAnimation = AnimationUtils.LoadAnimation(this, Resource.Animation.slide_left);
            m_eventAnimationEnded = null;

            // Debug asserts for making sure our view variables were initialized.
            System.Diagnostics.Debug.Assert(null != m_startButton, "Start button is null.");
            System.Diagnostics.Debug.Assert(null != m_optionsButton, "Options button is null.");
            System.Diagnostics.Debug.Assert(null != m_background, "Background view is null.");
            System.Diagnostics.Debug.Assert(null != m_flickerAnimation, "Flicker animation is null.");
            System.Diagnostics.Debug.Assert(null != m_fadeOutAnimation, "Fade out animation is null.");

            // Set click listeners on the clickable buttons in this view.
            m_startButton.SetOnClickListener(this);
            m_optionsButton.SetOnClickListener(this);
        }

        /// <summary>Called when the Activity is once again visible.</summary>
        protected override void OnResume()
        {
            // Always call base class.
            base.OnResume();

            // Reset animation state.
            ClearAnimations();

            // Start the button slide animations.
            m_startButton.StartAnimation(m_slideLeftAnimation);
            m_optionsButton.StartAnimation(m_slideRightAnimation);
        }

        /// <summary>Called when the Activity is no longer visible.</summary>
        protected override void OnStop()
        {
            base.OnStop();

            ClearAnimations();
        }

        /// <summary>Required by implementing the View.IOnClickListener.</summary>
        public void OnClick(View view)
        {
            // If the animation ending event isn't null, then an animation is currently happening.
            if (null != m_eventAnimationEnded)
            {
                return;
            }

            // Depending on which view was clicked, start the correct Activity.
            if (view.Id == m_startButton.Id)
            {
                //TODO: Ignore this for Part 1.
                //Intent intent = new Intent(BaseContext, PickLevelActivity.class);
                //m_eventAnimationEnded = delegate { ChangeActivity(new Intent(BaseContext, typeof(IntroActivity))); };
                //m_fadeOutAnimation.AnimationEnd += m_eventAnimationEnded;

                // Flicker the start button and fade out the background and options button.
                m_startButton.StartAnimation(m_flickerAnimation);
                m_optionsButton.StartAnimation(m_fadeOutAnimation);
                m_background.StartAnimation(m_fadeOutAnimation);
            }
            else if (view.Id == m_optionsButton.Id)
            {
                //TODO: Ignore this for Part 1.
                //Intent intent = new Intent(BaseContext, OptionsActivity.class);
                //m_eventAnimationEnded = delegate { ChangeActivity(new Intent(BaseContext, typeof(IntroActivity))); };
                //m_fadeOutAnimation.AnimationEnd += m_eventAnimationEnded;

                // Flicker the options button and fade out the background and start button.
                m_optionsButton.StartAnimation(m_flickerAnimation);
                m_startButton.StartAnimation(m_fadeOutAnimation);
                m_background.StartAnimation(m_fadeOutAnimation);
            }
        }

        /// <summary>Clear any active animations.</summary>
        private void ClearAnimations()
        {
            // Clear animations.
            m_startButton.ClearAnimation();
            m_optionsButton.ClearAnimation();
            m_background.ClearAnimation();

            // TODO: Ignore this for Part 1.
            // Remove the animation end event handler if it's not already removed.
            if (null != m_eventAnimationEnded)
            {
                m_fadeOutAnimation.AnimationEnd -= m_eventAnimationEnded;
                m_eventAnimationEnded = null;
            }
        }

        /// <summary>Change to a new Activity specified by the given intent.</summary>
        private void ChangeActivity(Intent intent)
        {
            // TODO: Ignore this for Part 1.
            // Make sure given intent is valid.
            if (null == intent)
            {
                System.Diagnostics.Debug.Assert(false, "ChangeActivity() called with null Intent.");
                return;
            }

            // Clear animation state and then start the new activity.
            ClearAnimations();
            StartActivity(intent);
        }
    }
}
