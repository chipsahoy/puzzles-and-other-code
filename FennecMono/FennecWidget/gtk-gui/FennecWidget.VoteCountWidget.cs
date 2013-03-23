
// This file has been generated by the GUI designer. Do not modify.
namespace FennecWidget
{
	public partial class VoteCountWidget
	{
		private global::Gtk.VBox vbox2;
		private global::Gtk.Table table1;
		private global::Gtk.Button btnCopyCount;
		private global::Gtk.Button btnEOD;
		private global::Gtk.Button btnGetPosts;
		private global::Gtk.Button btnPlayerList;
		private global::Gtk.Button btnPostIt;
		private global::Gtk.Label label2;
		private global::Gtk.Label label4;
		private global::Gtk.Entry TxtEndPost;
		private global::Gtk.Entry txtEndPostTime;
		private global::Gtk.Entry txtStartPostTime;
		private global::Gtk.Entry udStartPost;
		private global::Gtk.ScrolledWindow GtkScrolledWindow;
		private global::Gtk.TreeView grdVotes;
		private global::Gtk.Statusbar statusbar1;
		private global::Gtk.Label lblStatus;
		private global::Gtk.Button btnHide;
		private global::Gtk.Button btnUnhide;
		
		protected virtual void Build ()
		{
			global::Stetic.Gui.Initialize (this);
			// Widget FennecWidget.VoteCountWidget
			global::Stetic.BinContainer.Attach (this);
			this.Name = "FennecWidget.VoteCountWidget";
			// Container child FennecWidget.VoteCountWidget.Gtk.Container+ContainerChild
			this.vbox2 = new global::Gtk.VBox ();
			this.vbox2.Name = "vbox2";
			this.vbox2.Spacing = 6;
			// Container child vbox2.Gtk.Box+BoxChild
			this.table1 = new global::Gtk.Table (((uint)(2)), ((uint)(6)), false);
			this.table1.Name = "table1";
			this.table1.RowSpacing = ((uint)(6));
			this.table1.ColumnSpacing = ((uint)(6));
			// Container child table1.Gtk.Table+TableChild
			this.btnCopyCount = new global::Gtk.Button ();
			this.btnCopyCount.CanFocus = true;
			this.btnCopyCount.Name = "btnCopyCount";
			this.btnCopyCount.UseUnderline = true;
			this.btnCopyCount.Label = global::Mono.Unix.Catalog.GetString ("Copy Count");
			this.table1.Add (this.btnCopyCount);
			global::Gtk.Table.TableChild w1 = ((global::Gtk.Table.TableChild)(this.table1 [this.btnCopyCount]));
			w1.TopAttach = ((uint)(1));
			w1.BottomAttach = ((uint)(2));
			w1.LeftAttach = ((uint)(4));
			w1.RightAttach = ((uint)(5));
			w1.XOptions = ((global::Gtk.AttachOptions)(4));
			w1.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.btnEOD = new global::Gtk.Button ();
			this.btnEOD.CanFocus = true;
			this.btnEOD.Name = "btnEOD";
			this.btnEOD.UseUnderline = true;
			this.btnEOD.Label = global::Mono.Unix.Catalog.GetString ("Set EOD");
			this.table1.Add (this.btnEOD);
			global::Gtk.Table.TableChild w2 = ((global::Gtk.Table.TableChild)(this.table1 [this.btnEOD]));
			w2.TopAttach = ((uint)(1));
			w2.BottomAttach = ((uint)(2));
			w2.LeftAttach = ((uint)(3));
			w2.RightAttach = ((uint)(4));
			w2.XOptions = ((global::Gtk.AttachOptions)(4));
			w2.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.btnGetPosts = new global::Gtk.Button ();
			this.btnGetPosts.CanFocus = true;
			this.btnGetPosts.Name = "btnGetPosts";
			this.btnGetPosts.UseUnderline = true;
			this.btnGetPosts.Label = global::Mono.Unix.Catalog.GetString ("Refresh");
			this.table1.Add (this.btnGetPosts);
			global::Gtk.Table.TableChild w3 = ((global::Gtk.Table.TableChild)(this.table1 [this.btnGetPosts]));
			w3.LeftAttach = ((uint)(5));
			w3.RightAttach = ((uint)(6));
			w3.XOptions = ((global::Gtk.AttachOptions)(4));
			w3.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.btnPlayerList = new global::Gtk.Button ();
			this.btnPlayerList.CanFocus = true;
			this.btnPlayerList.Name = "btnPlayerList";
			this.btnPlayerList.UseUnderline = true;
			this.btnPlayerList.Label = global::Mono.Unix.Catalog.GetString ("Player List");
			this.table1.Add (this.btnPlayerList);
			global::Gtk.Table.TableChild w4 = ((global::Gtk.Table.TableChild)(this.table1 [this.btnPlayerList]));
			w4.LeftAttach = ((uint)(3));
			w4.RightAttach = ((uint)(4));
			w4.XOptions = ((global::Gtk.AttachOptions)(4));
			w4.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.btnPostIt = new global::Gtk.Button ();
			this.btnPostIt.CanFocus = true;
			this.btnPostIt.Name = "btnPostIt";
			this.btnPostIt.UseUnderline = true;
			this.btnPostIt.Label = global::Mono.Unix.Catalog.GetString ("Post Count");
			this.table1.Add (this.btnPostIt);
			global::Gtk.Table.TableChild w5 = ((global::Gtk.Table.TableChild)(this.table1 [this.btnPostIt]));
			w5.TopAttach = ((uint)(1));
			w5.BottomAttach = ((uint)(2));
			w5.LeftAttach = ((uint)(5));
			w5.RightAttach = ((uint)(6));
			w5.XOptions = ((global::Gtk.AttachOptions)(4));
			w5.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.label2 = new global::Gtk.Label ();
			this.label2.Name = "label2";
			this.label2.LabelProp = global::Mono.Unix.Catalog.GetString ("Start:");
			this.label2.Justify = ((global::Gtk.Justification)(1));
			this.table1.Add (this.label2);
			global::Gtk.Table.TableChild w6 = ((global::Gtk.Table.TableChild)(this.table1 [this.label2]));
			w6.XOptions = ((global::Gtk.AttachOptions)(4));
			w6.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.label4 = new global::Gtk.Label ();
			this.label4.Name = "label4";
			this.label4.LabelProp = global::Mono.Unix.Catalog.GetString ("End:");
			this.label4.Justify = ((global::Gtk.Justification)(1));
			this.table1.Add (this.label4);
			global::Gtk.Table.TableChild w7 = ((global::Gtk.Table.TableChild)(this.table1 [this.label4]));
			w7.TopAttach = ((uint)(1));
			w7.BottomAttach = ((uint)(2));
			w7.XOptions = ((global::Gtk.AttachOptions)(4));
			w7.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.TxtEndPost = new global::Gtk.Entry ();
			this.TxtEndPost.Sensitive = false;
			this.TxtEndPost.CanFocus = true;
			this.TxtEndPost.Name = "TxtEndPost";
			this.TxtEndPost.IsEditable = true;
			this.TxtEndPost.WidthChars = 6;
			this.TxtEndPost.InvisibleChar = '●';
			this.table1.Add (this.TxtEndPost);
			global::Gtk.Table.TableChild w8 = ((global::Gtk.Table.TableChild)(this.table1 [this.TxtEndPost]));
			w8.TopAttach = ((uint)(1));
			w8.BottomAttach = ((uint)(2));
			w8.LeftAttach = ((uint)(1));
			w8.RightAttach = ((uint)(2));
			w8.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.txtEndPostTime = new global::Gtk.Entry ();
			this.txtEndPostTime.Sensitive = false;
			this.txtEndPostTime.CanFocus = true;
			this.txtEndPostTime.Name = "txtEndPostTime";
			this.txtEndPostTime.IsEditable = true;
			this.txtEndPostTime.WidthChars = 5;
			this.txtEndPostTime.InvisibleChar = '●';
			this.table1.Add (this.txtEndPostTime);
			global::Gtk.Table.TableChild w9 = ((global::Gtk.Table.TableChild)(this.table1 [this.txtEndPostTime]));
			w9.TopAttach = ((uint)(1));
			w9.BottomAttach = ((uint)(2));
			w9.LeftAttach = ((uint)(2));
			w9.RightAttach = ((uint)(3));
			w9.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.txtStartPostTime = new global::Gtk.Entry ();
			this.txtStartPostTime.Sensitive = false;
			this.txtStartPostTime.CanFocus = true;
			this.txtStartPostTime.Name = "txtStartPostTime";
			this.txtStartPostTime.IsEditable = true;
			this.txtStartPostTime.WidthChars = 5;
			this.txtStartPostTime.InvisibleChar = '●';
			this.table1.Add (this.txtStartPostTime);
			global::Gtk.Table.TableChild w10 = ((global::Gtk.Table.TableChild)(this.table1 [this.txtStartPostTime]));
			w10.LeftAttach = ((uint)(2));
			w10.RightAttach = ((uint)(3));
			w10.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.udStartPost = new global::Gtk.Entry ();
			this.udStartPost.Sensitive = false;
			this.udStartPost.CanFocus = true;
			this.udStartPost.Name = "udStartPost";
			this.udStartPost.IsEditable = true;
			this.udStartPost.WidthChars = 5;
			this.udStartPost.InvisibleChar = '●';
			this.table1.Add (this.udStartPost);
			global::Gtk.Table.TableChild w11 = ((global::Gtk.Table.TableChild)(this.table1 [this.udStartPost]));
			w11.LeftAttach = ((uint)(1));
			w11.RightAttach = ((uint)(2));
			w11.YOptions = ((global::Gtk.AttachOptions)(4));
			this.vbox2.Add (this.table1);
			global::Gtk.Box.BoxChild w12 = ((global::Gtk.Box.BoxChild)(this.vbox2 [this.table1]));
			w12.Position = 0;
			w12.Expand = false;
			w12.Fill = false;
			// Container child vbox2.Gtk.Box+BoxChild
			this.GtkScrolledWindow = new global::Gtk.ScrolledWindow ();
			this.GtkScrolledWindow.Name = "GtkScrolledWindow";
			this.GtkScrolledWindow.ShadowType = ((global::Gtk.ShadowType)(1));
			// Container child GtkScrolledWindow.Gtk.Container+ContainerChild
			this.grdVotes = new global::Gtk.TreeView ();
			this.grdVotes.CanFocus = true;
			this.grdVotes.Name = "grdVotes";
			this.GtkScrolledWindow.Add (this.grdVotes);
			this.vbox2.Add (this.GtkScrolledWindow);
			global::Gtk.Box.BoxChild w14 = ((global::Gtk.Box.BoxChild)(this.vbox2 [this.GtkScrolledWindow]));
			w14.Position = 1;
			// Container child vbox2.Gtk.Box+BoxChild
			this.statusbar1 = new global::Gtk.Statusbar ();
			this.statusbar1.Name = "statusbar1";
			this.statusbar1.Spacing = 6;
			// Container child statusbar1.Gtk.Box+BoxChild
			this.lblStatus = new global::Gtk.Label ();
			this.lblStatus.Name = "lblStatus";
			this.lblStatus.LabelProp = global::Mono.Unix.Catalog.GetString ("status");
			this.statusbar1.Add (this.lblStatus);
			global::Gtk.Box.BoxChild w15 = ((global::Gtk.Box.BoxChild)(this.statusbar1 [this.lblStatus]));
			w15.Position = 1;
			w15.Expand = false;
			w15.Fill = false;
			// Container child statusbar1.Gtk.Box+BoxChild
			this.btnHide = new global::Gtk.Button ();
			this.btnHide.Sensitive = false;
			this.btnHide.CanFocus = true;
			this.btnHide.Name = "btnHide";
			this.btnHide.UseUnderline = true;
			this.btnHide.Label = global::Mono.Unix.Catalog.GetString ("Hide");
			this.statusbar1.Add (this.btnHide);
			global::Gtk.Box.BoxChild w16 = ((global::Gtk.Box.BoxChild)(this.statusbar1 [this.btnHide]));
			w16.Position = 2;
			w16.Expand = false;
			w16.Fill = false;
			// Container child statusbar1.Gtk.Box+BoxChild
			this.btnUnhide = new global::Gtk.Button ();
			this.btnUnhide.Sensitive = false;
			this.btnUnhide.CanFocus = true;
			this.btnUnhide.Name = "btnUnhide";
			this.btnUnhide.UseUnderline = true;
			this.btnUnhide.Label = global::Mono.Unix.Catalog.GetString ("Unhide");
			this.statusbar1.Add (this.btnUnhide);
			global::Gtk.Box.BoxChild w17 = ((global::Gtk.Box.BoxChild)(this.statusbar1 [this.btnUnhide]));
			w17.Position = 3;
			w17.Expand = false;
			w17.Fill = false;
			this.vbox2.Add (this.statusbar1);
			global::Gtk.Box.BoxChild w18 = ((global::Gtk.Box.BoxChild)(this.vbox2 [this.statusbar1]));
			w18.Position = 3;
			w18.Expand = false;
			w18.Fill = false;
			this.Add (this.vbox2);
			if ((this.Child != null)) {
				this.Child.ShowAll ();
			}
			this.Hide ();
			this.btnPostIt.Clicked += new global::System.EventHandler (this.OnBtnPostItClicked);
			this.btnPlayerList.Clicked += new global::System.EventHandler (this.OnBtnPlayerListClicked);
			this.btnGetPosts.Clicked += new global::System.EventHandler (this.OnBtnGetPostsClicked);
			this.btnEOD.Clicked += new global::System.EventHandler (this.OnBtnEODClicked);
			this.btnCopyCount.Clicked += new global::System.EventHandler (this.OnBtnCopyCountClicked);
		}
	}
}
