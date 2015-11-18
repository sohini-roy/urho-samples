//
// Copyright (c) 2008-2015 the Urho3D project.
// Copyright (c) 2015 Xamarin Inc
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//

namespace Urho.Samples
{
	public class HelloGUI : Sample
	{
		Window window;
		UIElement uiRoot;
		IntVector2 dragBeginPosition;
		Button draggableFish;

		public HelloGUI(Context ctx) : base(ctx)
		{
			uiRoot = UI.Root;
			dragBeginPosition = new IntVector2(0, 0);
		}

		public override void Start()
		{
			base.Start();
			Input.SetMouseVisible(true, false);
			// Load XML file containing default UI style sheet
			var cache = ResourceCache;
			XmlFile style = cache.GetXmlFile("UI/DefaultStyle.xml");

			// Set the loaded style as default style
			uiRoot.SetDefaultStyle(style);

			// Initialize Window
			InitWindow();

			// Create and add some controls to the Window
			InitControls();

			// Create a draggable Fish
			CreateDraggableFish();
		}

		void InitControls()
		{
			// Create a CheckBox
			CheckBox checkBox = new CheckBox(Context);
			checkBox.Name = "CheckBox";

			// Create a Button
			Button button = new Button(Context);
			button.Name = "Button";
			button.MinHeight = 24;

			// Create a LineEdit
			LineEdit lineEdit = new LineEdit(Context);
			lineEdit.Name = "LineEdit";
			lineEdit.MinHeight = 24;

			// Add controls to Window
			window.AddChild(checkBox);
			window.AddChild(button);
			window.AddChild(lineEdit);

			// Apply previously set default style
			checkBox.SetStyleAuto(null);
			button.SetStyleAuto(null);
			lineEdit.SetStyleAuto(null);
		}

		void InitWindow()
		{
			// Create the Window and add it to the UI's root node
			window = new Window(Context);
			uiRoot.AddChild(window);

			// Set Window size and layout settings
			window.SetMinSize(384, 192);
			window.SetLayout(LayoutMode.Vertical, 6, new IntRect(6, 6, 6, 6));
			window.SetAlignment(HorizontalAlignment.Center, VerticalAlignment.Center);
			window.Name = "Window";

			// Create Window 'titlebar' container
			UIElement titleBar = new UIElement(Context);
			titleBar.SetMinSize(0, 24);
			titleBar.VerticalAlignment = VerticalAlignment.Top;
			titleBar.LayoutMode = LayoutMode.Horizontal;

			// Create the Window title Text
			var windowTitle = new Text(Context);
			windowTitle.Name = "WindowTitle";
			windowTitle.Value = "Hello GUI!";

			// Create the Window's close button
			Button buttonClose = new Button(Context);
			buttonClose.Name = "CloseButton";

			// Add the controls to the title bar
			titleBar.AddChild(windowTitle);
			titleBar.AddChild(buttonClose);

			// Add the title bar to the Window
			window.AddChild(titleBar);

			// Apply styles
			window.SetStyleAuto(null);
			windowTitle.SetStyleAuto(null);
			buttonClose.SetStyle("CloseButton", null);

			buttonClose.SubscribeToReleased (args => Engine.Exit ());
				
			// Subscribe also to all UI mouse clicks just to see where we have clicked
			UI.SubscribeToUIMouseClick(HandleControlClicked);
		}

		void CreateDraggableFish()
		{
			var cache = ResourceCache;
			var graphics = Graphics;

			// Create a draggable Fish button
			draggableFish = new Button(Context);
			draggableFish.Texture = cache.GetTexture2D("Textures/UrhoDecal.dds"); // Set texture
			draggableFish.BlendMode = BlendMode.Add;
			draggableFish.SetSize(128, 128);
			draggableFish.SetPosition((graphics.Width - draggableFish.Width)/2, 200);
			draggableFish.Name = "Fish";
			uiRoot.AddChild(draggableFish);

			// Add a tooltip to Fish button
			ToolTip toolTip = new ToolTip(Context);
			draggableFish.AddChild(toolTip);
			toolTip.Position = new IntVector2(draggableFish.Width + 5, draggableFish.Width/2);
			// slightly offset from close button
			BorderImage textHolder = new BorderImage(Context);
			toolTip.AddChild(textHolder);
			textHolder.SetStyle("ToolTipBorderImage", null);
			var toolTipText = new Text(Context);
			textHolder.AddChild(toolTipText);
			toolTipText.SetStyle("ToolTipText", null);
			toolTipText.Value = "Please drag me!";

			// Subscribe draggableFish to Drag Events (in order to make it draggable)
			// See "Event list" in documentation's Main Page for reference on available Events and their eventData
			UI.SubscribeToDragBegin(HandleDragBegin);
			UI.SubscribeToDragMove(HandleDragMove);
			UI.SubscribeToDragEnd(HandleDragEnd);
		}

		void HandleDragBegin(DragBeginEventArgs args)
		{
			if (args.Element == draggableFish)
			{
				// Get UIElement relative position where input (touch or click) occured (top-left = IntVector2(0,0))
				dragBeginPosition = new IntVector2(args.ElementX, args.ElementY);
			}
		}

		void HandleDragMove(DragMoveEventArgs args)
		{
			if (args.Element == draggableFish)
			{
				IntVector2 dragCurrentPosition = new IntVector2(args.X, args.Y);
				args.Element.Position = dragCurrentPosition - dragBeginPosition;
			}
		}

		void HandleDragEnd(DragEndEventArgs args) // For reference (not used here)
		{
		}

		void HandleControlClicked(UIMouseClickEventArgs args)
		{
			// Get the Text control acting as the Window's title
			var windowTitle = window.GetChild("WindowTitle", true) as Text;

			// Get control that was clicked
			UIElement clicked = args.Element;

			string name = "...?";
			if (clicked != null)
			{
				// Get the name of the control that was clicked
				name = clicked.Name;
			}

			// Update the Window's title text
			windowTitle.Value = "Hello " + name + "!";
		}

		protected override string JoystickLayoutPatch => JoystickLayoutPatches.Hidden;
	}
}
