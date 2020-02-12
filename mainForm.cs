using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;

namespace Othello
{
	public class MainForm : System.Windows.Forms.Form
	{
		public Board board;
		public System.Windows.Forms.StatusBarPanel statusBarTurn;
		public System.Windows.Forms.StatusBarPanel statusBarBlackScore;
		public System.Windows.Forms.StatusBarPanel statusBarWhiteScore;

		private System.Windows.Forms.StatusBar statusBar;
		private System.Windows.Forms.MainMenu mainMenu1;
		private System.Windows.Forms.MenuItem leaglMovesItem;
		private System.Windows.Forms.MenuItem newGameItem;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.MenuItem humanPlayerItem;
		private System.Windows.Forms.MenuItem intermediateItem;
		private System.Windows.Forms.MenuItem beginnerItem;
		private System.Windows.Forms.MenuItem undoItem;

		private System.ComponentModel.Container components = null;

		public MainForm()
		{
			InitializeComponent();

			board = new Board(this);
			board.UpdateBoardSize(ClientSize.Width, ClientSize.Height - statusBar.Height);
		}

		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(MainForm));
			this.statusBar = new System.Windows.Forms.StatusBar();
			this.statusBarTurn = new System.Windows.Forms.StatusBarPanel();
			this.statusBarBlackScore = new System.Windows.Forms.StatusBarPanel();
			this.statusBarWhiteScore = new System.Windows.Forms.StatusBarPanel();
			this.mainMenu1 = new System.Windows.Forms.MainMenu();
			this.newGameItem = new System.Windows.Forms.MenuItem();
			this.leaglMovesItem = new System.Windows.Forms.MenuItem();
			this.menuItem1 = new System.Windows.Forms.MenuItem();
			this.humanPlayerItem = new System.Windows.Forms.MenuItem();
			this.beginnerItem = new System.Windows.Forms.MenuItem();
			this.intermediateItem = new System.Windows.Forms.MenuItem();
			this.undoItem = new System.Windows.Forms.MenuItem();
			((System.ComponentModel.ISupportInitialize)(this.statusBarTurn)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.statusBarBlackScore)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.statusBarWhiteScore)).BeginInit();
			this.SuspendLayout();
			// 
			// statusBar
			// 
			this.statusBar.Location = new System.Drawing.Point(0, 362);
			this.statusBar.Name = "statusBar";
			this.statusBar.Panels.AddRange(new System.Windows.Forms.StatusBarPanel[] {
																												 this.statusBarTurn,
																												 this.statusBarBlackScore,
																												 this.statusBarWhiteScore});
			this.statusBar.ShowPanels = true;
			this.statusBar.Size = new System.Drawing.Size(424, 22);
			this.statusBar.TabIndex = 0;
			// 
			// statusBarTurn
			// 
			this.statusBarTurn.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Spring;
			this.statusBarTurn.Text = "Turn";
			this.statusBarTurn.Width = 317;
			// 
			// statusBarBlackScore
			// 
			this.statusBarBlackScore.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Contents;
			this.statusBarBlackScore.Text = "Black:";
			this.statusBarBlackScore.Width = 45;
			// 
			// statusBarWhiteScore
			// 
			this.statusBarWhiteScore.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Contents;
			this.statusBarWhiteScore.Text = "White:";
			this.statusBarWhiteScore.Width = 46;
			// 
			// mainMenu1
			// 
			this.mainMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																											 this.newGameItem,
																											 this.leaglMovesItem,
																											 this.menuItem1,
																											 this.undoItem});
			// 
			// newGameItem
			// 
			this.newGameItem.Index = 0;
			this.newGameItem.Text = "&New Game";
			this.newGameItem.Click += new System.EventHandler(this.newGameItem_Click);
			// 
			// leaglMovesItem
			// 
			this.leaglMovesItem.Index = 1;
			this.leaglMovesItem.Text = "Show Legal &Moves";
			this.leaglMovesItem.Click += new System.EventHandler(this.leaglMovesItem_Click);
			// 
			// menuItem1
			// 
			this.menuItem1.Index = 2;
			this.menuItem1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																											 this.humanPlayerItem,
																											 this.beginnerItem,
																											 this.intermediateItem});
			this.menuItem1.Text = "&Opponent";
			// 
			// humanPlayerItem
			// 
			this.humanPlayerItem.Checked = true;
			this.humanPlayerItem.Index = 0;
			this.humanPlayerItem.Text = "Another &Human";
			this.humanPlayerItem.Click += new System.EventHandler(this.humanPlayerItem_Click);
			// 
			// beginnerItem
			// 
			this.beginnerItem.Index = 1;
			this.beginnerItem.Text = "&Beginner";
			this.beginnerItem.Click += new System.EventHandler(this.beginerItem_Click);
			// 
			// intermediateItem
			// 
			this.intermediateItem.Index = 2;
			this.intermediateItem.Text = "&Intermediate";
			this.intermediateItem.Click += new System.EventHandler(this.intermediateItem_Click);
			// 
			// undoItem
			// 
			this.undoItem.Index = 3;
			this.undoItem.Text = "&Undo";
			this.undoItem.Click += new System.EventHandler(this.undoItem_Click);
			// 
			// MainForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(424, 384);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																							 this.statusBar});
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Menu = this.mainMenu1;
			this.Name = "MainForm";
			this.Text = "Othello";
			this.Resize += new System.EventHandler(this.OnResize);
			this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.MainForm_MouseDown);
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.OnPaint);
			((System.ComponentModel.ISupportInitialize)(this.statusBarTurn)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.statusBarBlackScore)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.statusBarWhiteScore)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion

		[STAThread]
		static void Main() 
		{
			Application.Run(new MainForm());
		}

		private void OnPaint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
			board.Draw(e.Graphics);
		}

		private void OnResize(object sender, System.EventArgs e)
		{
			board.UpdateBoardSize(ClientSize.Width, ClientSize.Height - statusBar.Height);
			Invalidate();
		}

		private void MainForm_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			board.Click(e);
			statusBar.Invalidate();
		}

		private void leaglMovesItem_Click(object sender, System.EventArgs e)
		{
			board.ShowLegalMoves();
			Invalidate();
		}

		private void newGameItem_Click(object sender, System.EventArgs e)
		{
			board.NewGame();
			Invalidate();
		}

		private void beginerItem_Click(object sender, System.EventArgs e)
		{
			board.ComputerPlayer = new ComputerPlayer();
			board.ComputerPlayer.Level = LevelEnum.Beginner;
			board.ComputerPlayer.AmIWhite = true;
			board.ComputerPlayer.Board = board;

			humanPlayerItem.Checked = false;
			beginnerItem.Checked = true;
			intermediateItem.Checked = false;

			board.NewGame();
			Invalidate();
		}

		private void intermediateItem_Click(object sender, System.EventArgs e)
		{
			board.ComputerPlayer = new ComputerPlayer();
			board.ComputerPlayer.Level = LevelEnum.Intermediate;
			board.ComputerPlayer.AmIWhite = true;
			board.ComputerPlayer.Board = board;

			humanPlayerItem.Checked = false;
			beginnerItem.Checked = false;
			intermediateItem.Checked = true;

			board.NewGame();
			Invalidate();
		}

		private void humanPlayerItem_Click(object sender, System.EventArgs e)
		{
			board.ComputerPlayer = null;

			humanPlayerItem.Checked = true;
			beginnerItem.Checked = false;
			intermediateItem.Checked = false;
		}

		private void undoItem_Click(object sender, System.EventArgs e)
		{
			board.Undo();
			Invalidate();
		}
	}
}
