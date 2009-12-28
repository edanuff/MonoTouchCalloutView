using System;
using System.Drawing;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using MonoTouch.CoreGraphics;
using MonoTouch.CoreAnimation;
using MonoTouch.ObjCRuntime;

namespace MonoTouchCalloutView
{

	partial class CalloutView : UIView
	{

		static float CENTER_IMAGE_WIDTH = 31;
		static float CALLOUT_HEIGHT = 70;
		static float MIN_LEFT_IMAGE_WIDTH = 16;
		static float MIN_RIGHT_IMAGE_WIDTH = 16;
		static float ANCHOR_X = 32;
		static float ANCHOR_Y = 60;
		static float CENTER_IMAGE_ANCHOR_OFFSET_X = 15;
		static float MIN_ANCHOR_X = MIN_LEFT_IMAGE_WIDTH + CENTER_IMAGE_ANCHOR_OFFSET_X;
		static float BUTTON_WIDTH = 29;
		static float BUTTON_Y = 10;
		static float LABEL_HEIGHT = 48;
		static float LABEL_FONT_SIZE = 18;

		static UIImage CALLOUT_LEFT_IMAGE;
		static UIImage CALLOUT_CENTER_IMAGE;
		static UIImage CALLOUT_RIGHT_IMAGE;

		static RectangleF _initframe = new RectangleF (0, 0, 100, CALLOUT_HEIGHT);

		public UIImageView CalloutLeft;
		public UIImageView CalloutCenter;
		public UIImageView CalloutRight;
		public UIButton CalloutButton;
		public UILabel CalloutLabel;

		// The IntPtr and NSCoder constructors are required for controllers that need 
		// to be able to be created from a xib rather than from managed code

		public CalloutView (IntPtr handle) : base(handle)
		{
			Initialize ();
		}

		[Export("initWithCoder:")]
		public CalloutView (NSCoder coder) : base(coder)
		{
			Initialize ();
		}

		[Export("initWithFrame:")]
		public CalloutView (System.Drawing.RectangleF frame) : base(frame)
		{
			Initialize ();
		}

		public CalloutView (PointF pt) : base(_initframe)
		{
			SetAnchorPoint (pt);
			Initialize ();
		}


		public CalloutView (string text, PointF pt) : base(_initframe)
		{
			SetAnchorPoint (pt);
			Initialize ();
			Text = text;
		}

		public CalloutView (NSObjectFlag t) : base(t)
		{
		}

		public CalloutView (string text, PointF pt, NSObject target, Selector sel) : base(_initframe)
		{
			SetAnchorPoint (pt);
			Initialize ();
			Text = text;
			AddButtonTarget (target, sel);
		}

		public static CalloutView AddCalloutView (UIView parent, string text, PointF pt, NSObject target, Selector sel)
		{
			CalloutView callout = new CalloutView (text, pt, target, sel);
			callout.ShowWithAnimation (parent);
			return callout;
		}

		public void ShowWithAnimation (UIView parent)
		{
			Transform = CGAffineTransform.MakeScale (0.8f, 0.8f);
			BeginAnimations (null);
			SetAnimationDelegate (this);
			SetAnimationWillStartSelector (new Selector ("animationWillStart:context:"));
			SetAnimationDidStopSelector (new Selector ("animationDidStop:finished:context:"));
			SetAnimationDuration (0.1f);
			parent.AddSubview (this);
			Transform = CGAffineTransform.MakeScale (1.2f, 1.2f);
			CommitAnimations ();
		}

		[MonoTouch.Foundation.Export("animationWillStart:context:")]
		public void AnimationStarted (CAAnimation anim, IntPtr context)
		{
		}

		[MonoTouch.Foundation.Export("animationDidStop:finished:context:")]
		public void AnimationStopped (CAAnimation anim, bool finished, IntPtr context)
		{
			this.Transform = CGAffineTransform.MakeIdentity ();
		}

		public string Text {
			get { return CalloutLabel.Text; }
			set {
				CalloutLabel.Text = value;
				this.SetNeedsLayout ();
			}
		}

		void Initialize ()
		{
			this.BackgroundColor = UIColor.Clear;
			this.Opaque = false;
			
			if (CALLOUT_LEFT_IMAGE == null) {
				CALLOUT_LEFT_IMAGE = UIImage.FromFile ("images/callout_left.png").StretchableImage (15, 0);
			}
			
			if (CALLOUT_CENTER_IMAGE == null) {
				CALLOUT_CENTER_IMAGE = UIImage.FromFile ("images/callout_center.png");
			}
			
			if (CALLOUT_RIGHT_IMAGE == null) {
				CALLOUT_RIGHT_IMAGE = UIImage.FromFile ("images/callout_right.png").StretchableImage (1, 0);
			}
			
			RectangleF frame = this.Frame;
			frame.Height = CALLOUT_HEIGHT;
			if (frame.Width < 100)
				frame.Width = 100;
			this.Frame = frame;
			
			if (ANCHOR_X < MIN_ANCHOR_X)
				ANCHOR_X = MIN_ANCHOR_X;
			
			float left_width = ANCHOR_X - CENTER_IMAGE_ANCHOR_OFFSET_X;
			float right_width = frame.Width - left_width - CENTER_IMAGE_WIDTH;
			
			CalloutLeft = new UIImageView (new RectangleF (0, 0, left_width, CALLOUT_HEIGHT));
			CalloutLeft.Image = CALLOUT_LEFT_IMAGE;
			AddSubview (CalloutLeft);
			
			CalloutCenter = new UIImageView (new RectangleF (left_width, 0, CENTER_IMAGE_WIDTH, CALLOUT_HEIGHT));
			CalloutCenter.Image = CALLOUT_CENTER_IMAGE;
			AddSubview (CalloutCenter);
			
			CalloutRight = new UIImageView (new RectangleF (left_width + CENTER_IMAGE_WIDTH, 0, right_width, CALLOUT_HEIGHT));
			CalloutRight.Image = CALLOUT_RIGHT_IMAGE;
			AddSubview (CalloutRight);
			
			CalloutLabel = new UILabel (new RectangleF (MIN_LEFT_IMAGE_WIDTH, 0, frame.Width - MIN_LEFT_IMAGE_WIDTH - BUTTON_WIDTH - MIN_RIGHT_IMAGE_WIDTH - 2, LABEL_HEIGHT));
			CalloutLabel.Font = UIFont.BoldSystemFontOfSize (LABEL_FONT_SIZE);
			CalloutLabel.TextColor = UIColor.White;
			CalloutLabel.BackgroundColor = UIColor.Clear;
			AddSubview (CalloutLabel);
			
			CalloutButton = UIButton.FromType (UIButtonType.DetailDisclosure);
			RectangleF f = CalloutButton.Frame;
			f.X = frame.Width - BUTTON_WIDTH - MIN_RIGHT_IMAGE_WIDTH + 4;
			f.Y = BUTTON_Y;
			CalloutButton.Frame = f;
			CalloutButton.AdjustsImageWhenHighlighted = false;
			AddSubview (CalloutButton);
			
		}

		public override void LayoutSubviews ()
		{
			base.LayoutSubviews ();
			
			SizeF size = this.StringSize (CalloutLabel.Text, CalloutLabel.Font);
			RectangleF frame = this.Frame;
			frame.Width = size.Width + MIN_LEFT_IMAGE_WIDTH + BUTTON_WIDTH + MIN_RIGHT_IMAGE_WIDTH + 3;
			frame.Height = CALLOUT_HEIGHT;
			this.Frame = frame;
			
			if (ANCHOR_X < MIN_ANCHOR_X)
				ANCHOR_X = MIN_ANCHOR_X;
			
			float left_width = ANCHOR_X - CENTER_IMAGE_ANCHOR_OFFSET_X;
			float right_width = frame.Width - left_width - CENTER_IMAGE_WIDTH;
			
			CalloutLeft.Frame = new RectangleF (0, 0, left_width, CALLOUT_HEIGHT);
			CalloutCenter.Frame = new RectangleF (left_width, 0, CENTER_IMAGE_WIDTH, CALLOUT_HEIGHT);
			CalloutRight.Frame = new RectangleF (left_width + CENTER_IMAGE_WIDTH, 0, right_width, CALLOUT_HEIGHT);
			
			CalloutLabel.Frame = new RectangleF (MIN_LEFT_IMAGE_WIDTH, 0, size.Width, LABEL_HEIGHT);
			
			RectangleF f = CalloutButton.Frame;
			f.X = frame.Width - BUTTON_WIDTH - MIN_RIGHT_IMAGE_WIDTH + 4;
			f.Y = BUTTON_Y;
			CalloutButton.Frame = f;
			
		}

		public void SetAnchorPoint (PointF pt)
		{
			RectangleF frame = this.Frame;
			frame.X = pt.X - ANCHOR_X;
			frame.Y = pt.Y - ANCHOR_Y;
			this.Frame = frame;
		}

		public void AddButtonTarget (NSObject target, Selector sel)
		{
			CalloutButton.AddTarget (target, sel, UIControlEvent.TouchUpInside);
		}
		
		
	}
}
