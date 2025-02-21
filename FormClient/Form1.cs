using Chess;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace FormClient
{
    public partial class Form1 : Form
    {
        ChessBoard chessBoard = new ChessBoard(false);
        Chess.Point selectedPiece = new Chess.Point();
        int selectedPlayer = -1;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form_Load(object sender, EventArgs e)
        {
            for (int x = 1; x < boardLayoutPanel.ColumnCount; x++)
            {
                for (int y = 1; y < boardLayoutPanel.RowCount; y++)
                {
                    Button button = new Button();
                    button.Dock = DockStyle.Fill;
                    button.Margin = new Padding(0);
                    button.FlatStyle = FlatStyle.Flat;
                    button.FlatAppearance.BorderSize = 0;
                    if ((x + y) % 2 == 1) button.BackColor = Color.White;
                    else button.BackColor = Color.AntiqueWhite;
                    boardLayoutPanel.Controls.Add(button);
                    button.Click += Click_Board;
                }
            }

            DrawPieces(chessBoard);
        }

        private void Click_Board(object s, EventArgs e)
        {
            DrawPieces(chessBoard);
            if (!(s is Button)) return;
            Button button = (Button)s;
            button.FlatStyle = FlatStyle.Standard;
            TableLayoutPanelCellPosition a = boardLayoutPanel.GetPositionFromControl((Control)s);

            if (!(button.Tag is ChessPiece))
            {
                if (selectedPlayer > -1)
                {
                    chessBoard.ActionPiece(selectedPiece.x, selectedPiece.y, a.Column - 1, a.Row - 1);
                    selectedPlayer = -1;
                    DrawPieces(chessBoard);
                }
                return;
            }

            ChessPiece chessPiece = (ChessPiece) button.Tag;
            Console.WriteLine("({2}, {3}) - {0} from team {1}", chessPiece.GetType(), chessPiece.Player, a.Column - 1, a.Row - 1);

            if (selectedPlayer > -1 && selectedPlayer != chessPiece.Player)
            {
                chessBoard.ActionPiece(selectedPiece.x, selectedPiece.y, a.Column - 1, a.Row - 1);
                selectedPlayer = -1;
                DrawPieces(chessBoard);
            }
            else
            {
                selectedPlayer = chessPiece.Player;
                selectedPiece.x = a.Column - 1;
                selectedPiece.y = a.Row - 1;
                foreach (Chess.Point point in chessBoard.PieceActions(a.Column - 1, a.Row - 1))
                {
                    Button actionButton = (Button)boardLayoutPanel.GetControlFromPosition(point.x + 1, point.y + 1);
                    actionButton.FlatStyle = FlatStyle.Standard;
                    Console.WriteLine("~({0}, {1})", point.x, point.y);
                }
                Console.WriteLine();
            }
        }

        private void DrawPieces(ChessBoard board)
        {
            for (int x = 0; x < board.GetLength(0); x++)
            {
                for (int y = 0; y < board.GetLength(1); y++)
                {
                    Button button = (Button)boardLayoutPanel.GetControlFromPosition(x + 1, y + 1);
                    button.FlatStyle = FlatStyle.Flat;
                    if (board[x, y] != null)
                    {
                        ChessPiece chessPiece = board[x, y];
                        button.Tag = chessPiece;
                        button.Text = chessPiece.ToString().Replace("Chess.", "");
                        if (chessPiece.Player == 1) button.Font = new Font(button.Font, FontStyle.Regular); //button.ForeColor = Color.White; 
                        else button.Font = new Font(button.Font, FontStyle.Bold); //button.ForeColor = Color.Black;
                    }
                    else
                    {
                        button.Text = "";
                        button.Tag = null;
                    }
                    this.coordinates.SetToolTip(button, String.Format("({0}, {1})", x, y));
                }
            }
        }

        private void boardLayoutPanel_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
            {
                chessBoard = new ChessBoard(true);
            } else
            {
                chessBoard = new ChessBoard(false);
            }

            selectedPiece = new Chess.Point();
            selectedPlayer = -1;
            DrawPieces(chessBoard);
        }
    }
}
