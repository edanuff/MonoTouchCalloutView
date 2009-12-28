
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using MonoTouch.CoreGraphics;
using MonoTouch.CoreAnimation;
using MonoTouch.ObjCRuntime;

namespace MonoTouchCalloutView
{


	public partial class TestView : UIView
	{

		NSTimer timer;
		
		CalloutView callout;
		
		PointF touchPoint;
		
		// The IntPtr and NSCoder constructors are required for controllers that need 
		// to be able to be created from a xib rather than from managed code

		public TestView (IntPtr handle) : base (handle) {
			Initialize();
		}

		[Export ("initWithCoder:")]
		public TestView (NSCoder coder) : base (coder) {
			Initialize();
		}

		public void Initialize() {
			Console.WriteLine("TestView.Initialize");
		}
		
		public override void TouchesBegan (NSSet touches, UIEvent evt)
		{
			Console.WriteLine("TestView.TouchesBegan");
			
			StopTimer();

			if (callout != null) {
				callout.RemoveFromSuperview();
				callout = null;
			}
							
			touchPoint = ((UITouch)touches.AnyObject).LocationInView(this);
			timer = NSTimer.CreateScheduledTimer(.5, this, new Selector("expand:"), null, false);
			
			base.TouchesBegan (touches, evt);
		}
		
		public override void TouchesMoved (NSSet touches, UIEvent evt)
		{
			Console.WriteLine("TestView.TouchesMoved");
			
			StopTimer();
			
			base.TouchesMoved (touches, evt);
		}
		
		public override void TouchesCancelled (NSSet touches, UIEvent evt)
		{
			Console.WriteLine("TestView.TouchesCancelled");
			
			StopTimer();
			
			base.TouchesCancelled (touches, evt);
		}

		public override void TouchesEnded (NSSet touches, UIEvent evt)
		{
			Console.WriteLine("TestView.TouchesEnded");
			
			StopTimer();
			
			base.TouchesEnded (touches, evt);
		}
		
		public void StopTimer() {
			if (timer != null) {
				if (timer.IsValid) timer.Invalidate();
				timer = null;
			}
		}
			
		[Export ("expand:")]
		public void Expand(NSTimer timer) {			
			ShowCalloutAt(touchPoint);
		}
		
		[Export ("handleCalloutClick:")]
		public void HandleCalloutClick(UIControl control) {
			Console.WriteLine("TestView.HandleCalloutClick");
			if (callout != null) {
				callout.RemoveFromSuperview();
				callout = null;
			}
		}
		
		public void ShowCalloutAt(PointF point) {
			if (callout != null) {
				callout.RemoveFromSuperview();
				callout = null;
			}
			
			callout = CalloutView.AddCalloutView(this, "Teleport", point, this, new Selector("handleCalloutClick:"));
		}
					
		
	}
}
