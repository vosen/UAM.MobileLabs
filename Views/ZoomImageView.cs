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
    public class ZoomImageView : ImageView
    {
        static float[] ZoomSteps = new float[] { 0.25f, 0.33f, 0.5f, 0.67f, 0.75f, 0.9f, 1.0f, 1.25f, 1.5f, 1.75f, 2.5f, 2.5f, 3.0f, 4.0f, 5.0f, 6.0f, 7.0f };
        public int CurrentStep { get; set; }
        Bitmap ImageSource { get; set; }
        private float CurrentScale { get { return ZoomSteps[CurrentStep]; } }

        float transX;
        public float TranslationX
        {
            get
            {
                return transX;
            }
            set
            {
                transX = NormalizedTranslationX(value);
            }
        }

        float transY;


        public float TranslationY
        {
            get
            {
                return transY;
            }
            set
            {
                transY = NormalizedTranslationY(value);
            }
        }

        private float NormalizedTranslationY(float value)
        {
            if (ImageSource == null)
                return value;
            float imageHeight = (ImageSource.Height * CurrentScale);
            float bottomSpace = Height - value - imageHeight;
            if (value > 0 && bottomSpace <= 0)
            {
                if (imageHeight >= Height)
                    return 0;
                else
                    return Height - imageHeight;
            }
            if (value <= 0 && bottomSpace > 0)
            {
                if (imageHeight >= Height)
                    return Height - imageHeight;
                else
                    return 0;
            }
            return value;
        }

        private float NormalizedTranslationX(float value)
        {
            if (ImageSource == null)
                return value;
            float imageWidth = (ImageSource.Width * CurrentScale);
            float rightSpace = Width - value - imageWidth;
            if (value > 0 && rightSpace <= 0)
            {
                if (imageWidth >= Width)
                    // snap to left
                    return 0;
                else
                    // snap to right
                    return Width - imageWidth;
            }
            if (value <= 0 && rightSpace > 0)
            {
                if (imageWidth >= Width)
                    // snap to right
                    return Width - imageWidth;
                else
                    // snap to left
                    return 0;
            }
            return value;
        }

        float lastX, lastY;

        public ZoomImageView(Context context, IAttributeSet attrs) :
            base(context, attrs)
        {
            Initialize();
        }

        public ZoomImageView(Context context, IAttributeSet attrs, int defStyle) :
            base(context, attrs, defStyle)
        {
            Initialize();
        }

        private void Initialize()
        {
            Clickable = true;
            SetScaleType(ScaleType.Matrix);
            CurrentStep = 6;
            TranslationX = 0;
            TranslationY = 0;
            this.Touch += (src, args) => OnTouch(args);
        }

        protected override IParcelable OnSaveInstanceState()
        {
            IParcelable parcel = base.OnSaveInstanceState();
            return new ZoomImageViewState(parcel, TranslationX, TranslationY, CurrentStep);
        }

        protected override void OnRestoreInstanceState(IParcelable state)
        {
            ZoomImageViewState savedState = state as ZoomImageViewState;
            if (savedState != null)
            {
                TranslationX = savedState.CenterX;
                TranslationY = savedState.CenterY;
                CurrentStep = savedState.ScaleStep;
            }
            base.OnRestoreInstanceState(savedState.SuperState);
        }

        public void ZoomOut()
        {
            float oldFactor = ZoomSteps[CurrentStep];
            CurrentStep = Math.Max(CurrentStep - 1, 0);
            float newFactor = ZoomSteps[CurrentStep];
            Zoom(oldFactor, newFactor);
        }

        public void ZoomIn()
        {
            float oldFactor = ZoomSteps[CurrentStep];
            CurrentStep = Math.Min(CurrentStep + 1, ZoomSteps.Length - 1);
            float newFactor = ZoomSteps[CurrentStep];
            Zoom(oldFactor, newFactor);
        }

        private void Zoom(float oldFactor, float newFactor)
        {
            float oldDistLeft = (Width / 2) - TranslationX;
            float oldDistTop = (Height / 2) - TranslationY;
            float newDistLeft = (newFactor / oldFactor) * oldDistLeft;
            float newDistTop = (newFactor / oldFactor) * oldDistTop;
            TranslationX += (oldDistLeft - newDistLeft);
            TranslationY += (oldDistTop - newDistTop);
            RefreshLayout(ZoomSteps[CurrentStep], TranslationX, TranslationY);
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
                    TranslationX += ev.Event.GetX() - lastX;
                    TranslationY += ev.Event.GetY() - lastY;
                    lastX = ev.Event.GetX();
                    lastY = ev.Event.GetY();
                    RefreshLayout(ZoomSteps[CurrentStep], TranslationX, TranslationY);
                    break;
            }
            ev.Handled = true;
        }

        public override void SetImageBitmap(Bitmap bm)
        {
            base.SetImageBitmap(bm);
            this.ImageSource = bm;
        }

        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            base.OnMeasure(widthMeasureSpec, heightMeasureSpec);
            if (ImageSource == null)
                return;
            RefreshLayout(ZoomSteps[CurrentStep], TranslationX, TranslationY);
        }

        public void RefreshLayout()
        {
            RefreshLayout(CurrentScale, TranslationX, TranslationY);
        }

        private void RefreshLayout(float scale, float centerX, float centerY)
        {
            ImageMatrix.SetScale(scale, scale);
            ImageMatrix.PostTranslate(centerX, centerY);
            ImageMatrix = ImageMatrix;
            Invalidate();
        }


    }
}