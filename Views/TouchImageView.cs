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
        const int ClickThreshold = 3;
        const float MaxScale = 10f;
        const float MinScale = 0.1f;
        Matrix matrix;
        float lastX, lastY;
        float startX, startY;
        float width, height;
        float currentScale = 1f;
        float right, bottom, origWidth, origHeight, bmWidth, bmHeight;
        float[] rawMatrix;

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
            matrix = new Matrix();
            matrix.SetTranslate(1.0f, 1.0f);
            rawMatrix = new float[9];
            ImageMatrix = matrix;
            SetScaleType(ScaleType.Matrix);
            this.Touch += (src, args) => OnTouch(args);
        }

        public void Scale(float factor)
        {
            float originalScale = currentScale;
            currentScale *= factor;
            if (currentScale > MaxScale)
            {
                currentScale = MaxScale;
                factor = MaxScale / originalScale;
            }
            else if (currentScale < MinScale)
            {
                currentScale = MinScale;
                factor = MinScale / originalScale;
            }
            right = width * currentScale - width - (2 * redundantXSpace * currentScale);
            bottom = height * currentScale - height - (2 * redundantYSpace * currentScale);
            if (origWidth * currentScale <= width || origHeight * currentScale <= height)
            {
                matrix.PostScale(factor, factor, width / 2, height / 2);
                if (factor < 1)
                {
                    matrix.GetValues(rawMatrix);
                    float x = rawMatrix[Matrix.MtransX];
                    float y = rawMatrix[Matrix.MtransY];
                    if (factor < 1)
                    {
                        if (Math.Round(origWidth * currentScale) < width)
                        {
                            if (y < -bottom)
                                matrix.PostTranslate(0, -(y + bottom));
                            else if (y > 0)
                                matrix.PostTranslate(0, -y);
                        }
                        else
                        {
                            if (x < -right)
                                matrix.PostTranslate(-(x + right), 0);
                            else if (x > 0)
                                matrix.PostTranslate(-x, 0);
                        }
                    }
                }
            }
            else
            {
                matrix.PostScale(factor, factor, Width/2, Height/2);
                matrix.GetValues(rawMatrix);
                float x = rawMatrix[Matrix.MtransX];
                float y = rawMatrix[Matrix.MtransY];
                if (factor < 1)
                {
                    if (x < -right)
                        matrix.PostTranslate(-(x + right), 0);
                    else if (x > 0)
                        matrix.PostTranslate(-x, 0);
                    if (y < -bottom)
                        matrix.PostTranslate(0, -(y + bottom));
                    else if (y > 0)
                        matrix.PostTranslate(0, -y);
                }
            }
            ImageMatrix = matrix;
        }

        void OnTouch(View.TouchEventArgs ev)
        {
            float[] raw = new float[9];
            matrix.GetValues(raw);
            float x = raw[Matrix.MtransX];
            float y = raw[Matrix.MtransY];
            switch (ev.Event.Action)
            {
                case MotionEventActions.Down:
                    lastX = ev.Event.GetX();
                    lastY = ev.Event.GetY();
                    startX = lastX;
                    startY = lastY;
                    break;
                case MotionEventActions.Move:
                    float deltaX = ev.Event.GetX() - lastX;
                    float deltaY = ev.Event.GetY() - lastY;
                    float scaleWidth = (float)Math.Round((double)(origWidth * currentScale));
                    float scaleHeight = (float)Math.Round((double)(origHeight * currentScale));
                    if (scaleWidth < width)
                    {
                        deltaX = 0;
                        if (y + deltaY > 0)
                            deltaY = -y;
                        else if (y + deltaY < -bottom)
                            deltaY = -(y + bottom);
                    }
                    else if (scaleHeight < height)
                    {
                        deltaY = 0;
                        if (x + deltaX > 0)
                            deltaX = -x;
                        else if (x + deltaX < -right)
                            deltaX = -(x + right);
                    }
                    else
                    {
                        if (x + deltaX > 0)
                            deltaX = -x;
                        else if (x + deltaX < -right)
                            deltaX = -(x + right);

                        if (y + deltaY > 0)
                            deltaY = -y;
                        else if (y + deltaY < -bottom)
                            deltaY = -(y + bottom);
                    }
                    matrix.PostTranslate(deltaX, deltaY);
                    lastX = ev.Event.GetX();
                    lastY = ev.Event.GetY();
                    break;
                case MotionEventActions.Up:
                    int xDiff = (int)Math.Abs(ev.Event.GetX() - startX);
                    int yDiff = (int)Math.Abs(ev.Event.GetY() - startY);
                    if (xDiff < ClickThreshold && yDiff < ClickThreshold)
                        PerformClick();
                    break;
            }
            ImageMatrix = matrix;
            Invalidate();
            ev.Handled = true;
        }

        public override void SetImageBitmap(Bitmap bm)
        {
            base.SetImageBitmap(bm);
            if (bm != null)
            {
                bmWidth = bm.Width;
                bmHeight = bm.Height;
            }
        }

        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            base.OnMeasure(widthMeasureSpec, heightMeasureSpec);
            width = MeasureSpec.GetSize(widthMeasureSpec);
            height = MeasureSpec.GetSize(heightMeasureSpec);
            //Fit to screen.
            float scale = 0;
            float scaleX = (float)width / (float)bmWidth;
            float scaleY = (float)height / (float)bmHeight;
            scale = Math.Min(scaleX, scaleY);
            matrix.SetScale(scale, scale);
            ImageMatrix = matrix;
            currentScale = 1f;
            // Center the image
            redundantYSpace = (float)height - (scale * (float)bmHeight);
            redundantXSpace = (float)width - (scale * (float)bmWidth);
            redundantYSpace /= (float)2;
            redundantXSpace /= (float)2;

            matrix.PostTranslate(redundantXSpace, redundantYSpace);

            origWidth = width - 2 * redundantXSpace;
            origHeight = height - 2 * redundantYSpace;
            right = width * currentScale - width - (2 * redundantXSpace * currentScale);
            bottom = height * currentScale - height - (2 * redundantYSpace * currentScale);
            ImageMatrix = matrix;
        }


    }
}