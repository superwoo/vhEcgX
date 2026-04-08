//
//  CollectDataViewController.cs
//  vhEcgX
//
//  Converted from Objective-C to C#
//  Copyright © 2018 谷山丰. All rights reserved.
//

using System;
using Foundation;
using UIKit;
using CoreGraphics;

namespace vhEcgX
{
    public class CollectDataViewController : UIViewController, IUIScrollViewDelegate
    {
        private UIScrollView scrollV;
        private UIView holder;
        private LeadView lead;
        private nfloat mmPerMV, pixelsPerMm, voltageScale, mmPerSecond, pixelsPerUV, pixelsPerPoint, uVpb;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            View.BackgroundColor = UIColor.White;
            Title = "Collect data";
            SetupUI();
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            NSTimer.CreateScheduledTimer(TimeSpan.FromSeconds(10.0), (timer) =>
            {
                BLEManager.SharedInstance.StopReadDataWithCharacteristic();
                DrawLine();
            });
        }

        private void SetupUI()
        {
            scrollV = new UIScrollView();
            scrollV.BackgroundColor = UIColor.FromRGB(240f / 255f, 240f / 255f, 240f / 255f);
            View.AddSubview(scrollV);

            // Using frame-based layout (Masonry equivalent would need a library)
            scrollV.Frame = View.Bounds;
            scrollV.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;

            scrollV.Delegate = this;
            scrollV.ShowsVerticalScrollIndicator = false;
            scrollV.ShowsHorizontalScrollIndicator = false;

            holder = new UIView();
            holder.BackgroundColor = UIColor.FromRGB(240f / 255f, 240f / 255f, 240f / 255f);
            scrollV.AddSubview(holder);
            holder.Frame = scrollV.Bounds;
            holder.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;

            scrollV.MaximumZoomScale = 2.0f;
            scrollV.MinimumZoomScale = 0.5f;

            int pointsPerSecond = 500; // 采样率 每秒多个点
            pixelsPerMm = 6.16f; // 每毫米 6.16 像素
            mmPerMV = 10; // 每毫伏 10 毫米
            pixelsPerUV = 5 * 10.0f / 1000; // 每微伏 多少 个像素
            mmPerSecond = 25; // 每秒 25 毫米
            pixelsPerPoint = (pixelsPerMm * mmPerSecond) / pointsPerSecond; // 每个点 多少 像素
        }

        private void DrawLine()
        {
            var dataCount = BLEManager.SharedInstance.ReciveDataArray.Count;
            scrollV.ContentSize = new CGSize(pixelsPerPoint * dataCount, View.Frame.Size.Height);

            lead = new LeadView(new CGRect(0, 100, pixelsPerPoint * dataCount, 400));
            holder.AddSubview(lead);

            lead.PointsArray = BLEManager.SharedInstance.ReciveDataArray;
            lead.TransitionToPoint();
            lead.SetNeedsDisplay();
        }

        #region UIScrollViewDelegate

        [Export("viewForZoomingInScrollView:")]
        public UIView ViewForZoomingInScrollView(UIScrollView scrollView)
        {
            return holder;
        }

        #endregion
    }
}
