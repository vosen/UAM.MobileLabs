using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.Graphics;

namespace ComicsViewer.Views
{
    public class TouchImageView : ImageView
    {
        static float[] ZoomSteps = new float[] { 0.25f, 0.33f, 0.5f, 0.67f, 0.75f, 0.9f, 1.0f, 1.25f, 1.5f, 1.75f, 2.5f, 2.5f, 3.0f, 4.0f, 5.0f, 6.0f, 7.0f };
        const int ClickThreshold = 3;
        int CurrentStep;
        float centerX, centerY;

        float lastX, lastY;
        float startX, startY;
        float currentScale = 1f;
        float right, bottom, origWidth, origHeight, bmWidth, bmHeight;
        float[] rawMatrix = new float[9];
        Bitmap ImageSource { get; set; }

        float redundantXSpace, redundantYSpace;

        public TouchImageView(Context context, IAttributeSet attrs) :
            base(context, attrs)
        {
            Initialize();
        }

        public TouchImageView(Context context, IAttributeSet attrs, int defStyle) :
            base(context, attrs, defStyle)
        {
            Initialize();
        }

        private void Initialize()
        {
            Clickable = true;
            SetScaleType(ScaleType.Matrix);
            CurrentStep = 3;
            this.Touch += (src, args) => OnTouch(args);
        }

        protected override IParcelable OnSaveInstanceState()
        {
            IParcelable parcel = base.OnSaveInstanceState();
            return new TouchImageViewState(parcel, centerX, centerY, CurrentStep);
        }

        protected override void OnRestoreInstanceState(IParcelable state)
        {
            TouchImageViewState savedState = state as TouchImageViewState;
            if (savedState != null)
            {
                centerX = savedState.CenterX;
                centerY = savedState.CenterY;
                CurrentStep = savedState.ScaleStep;
            }
            base.OnRestoreInstanceState(savedState.SuperState);
        }

        public void ZoomOut()
        {
            CurrentStep = Math.Max(CurrentStep - 1, 0);
            Zoom(ZoomSteps[CurrentStep]);
            RefreshLayout(ZoomSteps[CurrentStep], centerX, centerY);
        }

        public void ZoomIn()
        {
            CurrentStep = Math.Min(CurrentStep + 1, ZoomSteps.Length - 1);
            RefreshLayout(ZoomSteps[CurrentStep], centerX, centerY);
        }

        private void Zoom(float factor)
        {
            ImageMatrix.SetScale(ZoomSteps[CurrentStep], ZoomSteps[CurrentStep]);
            ImageMatrix.PostTranslate(
                centerX - ZoomSteps[CurrentStep] * (ImageSource.Width / 2f),
                centerY - ZoomSteps[CurrentStep] * (ImageSource.Height / 2f));
            //RefreshLayout();
            Invalidate();
        }

        private void Move(float x, float y)
        {
            centerX += x;
            centerY += y;
            RefreshLayout(ZoomSteps[CurrentStep], centerX, centerY);
        }


        void OnTouch(View.TouchEventArgs ev)
        {
            switch (ev.Event.Action)
            {
                case MotionEventActions.Down:
                    lastX = ev.Event.GetX();
                    lastY = ev.Event.GetY();
                    break;
                case MotionEventActions.Move:
                    centerX += ev.Event.GetX() - lastX;
                    centerY += ev.Event.GetY() - lastY;
                    lastX = ev.Event.GetX();
                    lastY = ev.Event.GetY();
                    RefreshLayout(ZoomSteps[CurrentStep], centerX, centerY);
                    break;
            }
            ev.Handled = true;
        }

        public override void SetImageBitmap(Bitmap bm)
        {
            base.SetImageBitmap(bm);
            if (bm == null)
                return;
            if (ImageSource != null)
            {
                // subsequent image changes
            }
            this.ImageSource = bm;
        }

        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            base.OnMeasure(widthMeasureSpec, heightMeasureSpec);
            if (ImageSource == null)
                return;

            centerX = Width / 2f;
            centerY = Height / 2f;
            // just center the image
            //ImageMatrix.SetRectToRect(new RectF(0, 0, ImageSource.Width, ImageSource.Height), new RectF(0, 0, Width, Height), Matrix.ScaleToFit.Center);
            RefreshLayout(ZoomSteps[CurrentStep], centerX, centerY);
        }

        private void RefreshLayout(float scale, float centerX, float centerY)
        {
            ImageMatrix.SetScale(scale, scale);
            ImageMatrix.PostTranslate(
                centerX - scale * (ImageSource.Width / 2f),
                centerY - scale * (ImageSource.Height / 2f));
            ImageMatrix = ImageMatrix;
            Invalidate();
        }


    }
}