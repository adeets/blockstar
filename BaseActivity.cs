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
    internal class BaseActivity : Activity
    {
        internal BaseActivity() {}

        /// <summary>Called when the Activity is created or the orientation changes.</summary>
        protected override void OnCreate(Bundle bundle)
        {
            // Always call base class.
            base.OnCreate(bundle);

            // Initialize the music manager if it hasn't been done yet.
            if (!MusicManager.Initialized)
            {
                MusicManager.Initialize(this.BaseContext, 100);

                // Play some music!
                MusicManager.Play(MusicManager.MusicType.Menu);
            }
        }

        /// <summary>Called when the Activity is once again visible.</summary>
        protected override void OnResume()
        {
            // Always call base class.
            base.OnResume();

            // Resume doesn't do anything if something is already playing, 
            // so we are fine when it's called after OnCreate() finishes.
            MusicManager.Resume();
        }

        /// <summary>Called when the Activity is no longer visible.</summary>
        protected override void OnDestroy()
        {
            // Always call base class.
            base.OnDestroy();

            MusicManager.Destroy();
        }

        /// <summary>Called when the Activity is no longer visible.</summary>
        protected override void OnPause()
        {
            // Always call base class.
            base.OnPause();

            MusicManager.Pause();
        }
    }
}