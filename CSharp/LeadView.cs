//
//  LeadView.cs
//  vhEcgX
//
//  Converted from Objective-C to C#
//  Copyright © 2018 Jun Gao. All rights reserved.
//

using System;
using System.Collections.Generic;
using Foundation;
using UIKit;
using CoreGraphics;

namespace vhEcgX
{
    public class LeadView : UIView
    {
        private nfloat lineWidth;
        private CGPoint[] drawingPoints = new CGPoint[100000];
        private nfloat mmPerMV, pixelsPerMm, voltageScale, mmPerSecond, pixelsPerUV, pixelsPerPoint, uVpb;

        public List<NSNumber> PointsArray { get; set; }

        public LeadView(CGRect frame) : base(frame)
        {
            BackgroundColor = UIColor.White;
            ClearsContextBeforeDrawing = true;

            int pointsPerSecond = 500; // 采样率 每秒多个点
            pixelsPerMm = 6.16f; // 每毫米 6.16 像素
            mmPerMV = 10; // 每毫伏 10 毫米
            pixelsPerUV = mmPerMV * pixelsPerMm / 1000.0f; // 每微伏 多少 个像素
            mmPerSecond = 25; // 每秒 25 毫米
            pixelsPerPoint = (pixelsPerMm * mmPerSecond) / pointsPerSecond; // 每个点 多少 像素
            lineWidth = 1.2f;
            uVpb = 1.0f; // 1.272;
        }

        public override void Draw(CGRect rect)
        {
            var context = UIGraphics.GetCurrentContext();
            DrawGrid(context);
            DrawCurve(context);
        }

        public void TransitionToPoint()
        {
            nfloat pos_x = 0;
            nfloat pos_y = 0;
            nfloat pos_x_offset = 0.0f;
            nfloat full_height = Frame.Size.Height;

            for (int i = 0; i < PointsArray.Count; i++)
            {
                pos_x = pos_x + pos_x_offset + pixelsPerPoint;
                pos_y = full_height / 2 - PointsArray[i].Int32Value * uVpb * pixelsPerUV;
                drawingPoints[i] = new CGPoint(pos_x, pos_y);
            }
        }

        private void DrawGrid(CGContext ctx)
        {
            nfloat full_height = Frame.Size.Height;
            nfloat full_width = Frame.Size.Width;
            nfloat cell_square_width = 6.16f * 5;

            ctx.SetLineWidth(0.2f);
            ctx.SetStrokeColor(UIColor.LightGray.CGColor);

            nfloat pos_x = 1;
            while (pos_x < full_width)
            {
                ctx.MoveTo(pos_x, 1);
                ctx.AddLineToPoint(pos_x, full_height);
                pos_x += cell_square_width;
                ctx.StrokePath();
            }

            nfloat pos_y = 1;
            while (pos_y <= full_height)
            {
                ctx.SetLineWidth(0.2f);
                ctx.MoveTo(1, pos_y);
                ctx.AddLineToPoint(full_width, pos_y);
                pos_y += cell_square_width;
                ctx.StrokePath();
            }

            ctx.SetLineWidth(0.1f);

            cell_square_width = cell_square_width / 5;
            pos_x = 1 + cell_square_width;
            while (pos_x < full_width)
            {
                ctx.MoveTo(pos_x, 1);
                ctx.AddLineToPoint(pos_x, full_height);
                pos_x += cell_square_width;
                ctx.StrokePath();
            }

            pos_y = 1 + cell_square_width;
            while (pos_y <= full_height)
            {
                ctx.MoveTo(1, pos_y);
                ctx.AddLineToPoint(full_width, pos_y);
                pos_y += cell_square_width;
                ctx.StrokePath();
            }
        }

        private void DrawCurve(CGContext ctx)
        {
            if (PointsArray == null || PointsArray.Count <= 0)
            {
                return;
            }

            ctx.SetLineWidth(lineWidth);
            ctx.SetStrokeColor(UIColor.Black.CGColor);
            ctx.AddLines(drawingPoints, PointsArray.Count);
            ctx.StrokePath();
        }
    }
}
