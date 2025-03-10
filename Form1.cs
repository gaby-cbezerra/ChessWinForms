using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace ChessWinForms
{
    public partial class Form1 : Form
    {
        private TableLayoutPanel tableLayoutPanel;
        private PictureBox[,] board = new PictureBox[8, 8];
        private Dictionary<string, Image> pieceImages = new Dictionary<string, Image>();
        private PictureBox selectedPiece = null;
        private Pawn pawn = new Pawn();
        private Knight knight = new Knight();
        private Bishop bishop = new Bishop();
        private Rook rook = new Rook();
        private Queen queen = new Queen();
        private King king = new King();
        private bool isWhiteTurn = true; // Variável para rastrear o turno atual

        public Form1()
        {
            InitializeComponent();
            InitializeBoard();
            LoadPieceImages();
            SetupInitialPositions();
            DetermineStartingPlayer(); // Chama o método para determinar o jogador inicial
        }

        private void InitializeComponent()
        {
            this.Text = "Chess Game";
            this.ClientSize = new Size(480, 480);

            tableLayoutPanel = new TableLayoutPanel();
            tableLayoutPanel.RowCount = 8;
            tableLayoutPanel.ColumnCount = 8;
            tableLayoutPanel.Dock = DockStyle.Fill;

            // Configura as linhas e colunas para terem tamanho igual (12,5% cada)
            for (int i = 0; i < 8; i++)
            {
                tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 12.5f));
                tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 12.5f));
            }

            this.Controls.Add(tableLayoutPanel);
        }

        private void InitializeBoard()
        {
            // Cria um grid 8x8 de PictureBox e define as cores das casas
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    PictureBox pb = new PictureBox();
                    pb.Dock = DockStyle.Fill;
                    pb.SizeMode = PictureBoxSizeMode.StretchImage;
                    
                    // Define a cor da casa (alternando entre duas cores)
                    if ((row + col) % 2 == 0)
                        pb.BackColor = Color.BurlyWood;
                    else
                        pb.BackColor = Color.SaddleBrown;

                    // Registra o evento de clique
                    pb.Click += Cell_Click;
                    
                    // Armazena a posição no Name (ex.: "cell_0_0")
                    pb.Name = $"cell_{row}_{col}";

                    board[row, col] = pb;
                    tableLayoutPanel.Controls.Add(pb, col, row);
                }
            }
        }

        private void LoadPieceImages()
        {
            // As imagens devem estar na pasta "Images" do projeto.
            // Certifique-se de que essas imagens sejam copiadas para o diretório de saída.
            try
            {
                pieceImages["b_rook"] = Image.FromFile("Images/rook_black.png");
                pieceImages["b_knight"] = Image.FromFile("Images/knight_black.png");
                pieceImages["b_bishop"] = Image.FromFile("Images/bishop_black.png");
                pieceImages["b_queen"] = Image.FromFile("Images/queen_black.png");
                pieceImages["b_king"] = Image.FromFile("Images/king_black.png");
                pieceImages["b_pawn"] = Image.FromFile("Images/pawn_black.png");

                pieceImages["w_rook"] = Image.FromFile("Images/rook_white.png");
                pieceImages["w_knight"] = Image.FromFile("Images/knight_white.png");
                pieceImages["w_bishop"] = Image.FromFile("Images/bishop_white.png");
                pieceImages["w_queen"] = Image.FromFile("Images/queen_white.png");
                pieceImages["w_king"] = Image.FromFile("Images/king_white.png");
                pieceImages["w_pawn"] = Image.FromFile("Images/pawn_white.png");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao carregar imagens: " + ex.Message);
            }
        }

        private void SetupInitialPositions()
        {
            // Configura as peças pretas na primeira e segunda linhas
            board[0, 0].Image = pieceImages["b_rook"];   board[0, 0].Tag = "b_rook";
            board[0, 1].Image = pieceImages["b_knight"]; board[0, 1].Tag = "b_knight";
            board[0, 2].Image = pieceImages["b_bishop"]; board[0, 2].Tag = "b_bishop";
            board[0, 3].Image = pieceImages["b_queen"];  board[0, 3].Tag = "b_queen";
            board[0, 4].Image = pieceImages["b_king"];   board[0, 4].Tag = "b_king";
            board[0, 5].Image = pieceImages["b_bishop"]; board[0, 5].Tag = "b_bishop";
            board[0, 6].Image = pieceImages["b_knight"]; board[0, 6].Tag = "b_knight";
            board[0, 7].Image = pieceImages["b_rook"];   board[0, 7].Tag = "b_rook";

            for (int col = 0; col < 8; col++)
            {
                board[1, col].Image = pieceImages["b_pawn"];
                board[1, col].Tag = "b_pawn";
            }

            // Configura as peças brancas na última e penúltima linhas
            for (int col = 0; col < 8; col++)
            {
                board[6, col].Image = pieceImages["w_pawn"];
                board[6, col].Tag = "w_pawn";
            }

            board[7, 0].Image = pieceImages["w_rook"];   board[7, 0].Tag = "w_rook";
            board[7, 1].Image = pieceImages["w_knight"]; board[7, 1].Tag = "w_knight";
            board[7, 2].Image = pieceImages["w_bishop"]; board[7, 2].Tag = "w_bishop";
            board[7, 3].Image = pieceImages["w_queen"];  board[7, 3].Tag = "w_queen";
            board[7, 4].Image = pieceImages["w_king"];   board[7, 4].Tag = "w_king";
            board[7, 5].Image = pieceImages["w_bishop"]; board[7, 5].Tag = "w_bishop";
            board[7, 6].Image = pieceImages["w_knight"]; board[7, 6].Tag = "w_knight";
            board[7, 7].Image = pieceImages["w_rook"];   board[7, 7].Tag = "w_rook";
        }

        private void Cell_Click(object sender, EventArgs e)
        {
            PictureBox clickedCell = sender as PictureBox;
            if (clickedCell == null)
                return;

            // Se nenhuma peça estiver selecionada e a célula clicada contiver uma peça, seleciona-a.
            if (selectedPiece == null)
            {
                if (clickedCell.Image != null)
                {
                    // Verifica se é o turno correto para a peça selecionada
                    string pieceTag = clickedCell.Tag as string;
                    if ((isWhiteTurn && pieceTag.StartsWith("b_")) || (!isWhiteTurn && pieceTag.StartsWith("w_")))
                    {
                        MessageBox.Show("Não é o seu turno!");
                        return;
                    }

                    selectedPiece = clickedCell;
                    // Destaca a célula selecionada (por exemplo, alterando a borda)
                    clickedCell.BorderStyle = BorderStyle.Fixed3D;
                }
            }
            else
            {
                // Se o usuário clicar novamente na mesma célula, deseleciona
                if (clickedCell == selectedPiece)
                {
                    selectedPiece.BorderStyle = BorderStyle.None;
                    selectedPiece = null;
                    return;
                }

                string selectedTag = selectedPiece.Tag as string;
                if (selectedTag != null && selectedTag.EndsWith("pawn"))
                {
                    if (!pawn.IsValidMove(selectedPiece, clickedCell))
                    {
                        MessageBox.Show("Movimento inválido para o peão!");
                        return;
                    }
                }
                else if (selectedTag != null && selectedTag.EndsWith("knight"))
                {
                    if (!knight.IsValidMove(selectedPiece, clickedCell))
                    {
                        MessageBox.Show("Movimento inválido para o cavalo!");
                        return;
                    }
                }
                else if (selectedTag != null && selectedTag.EndsWith("bishop"))
                {
                    if (!bishop.IsValidMove(selectedPiece, clickedCell, board))
                    {
                        MessageBox.Show("Movimento inválido para o bispo!");
                        return;
                    }
                }
                else if (selectedTag != null && selectedTag.EndsWith("rook"))
                {
                    if (!rook.IsValidMove(selectedPiece, clickedCell, board))
                    {
                        MessageBox.Show("Movimento inválido para a torre!");
                        return;
                    }
                }
                else if (selectedTag != null && selectedTag.EndsWith("queen"))
                {
                    if (!queen.IsValidMove(selectedPiece, clickedCell, board))
                    {
                        MessageBox.Show("Movimento inválido para a rainha!");
                        return;
                    }
                }
                else if (selectedTag != null && selectedTag.EndsWith("king"))
                {
                    if (!king.IsValidMove(selectedPiece, clickedCell))
                    {
                        MessageBox.Show("Movimento inválido para o rei!");
                        return;
                    }
                }

                // Verifica se a célula destino possui uma peça
                if (clickedCell.Image != null)
                {
                    string destinationTag = clickedCell.Tag as string;

                    // Se a peça de destino for de cor oposta, captura-a
                    if (selectedTag != null && destinationTag != null && selectedTag[0] != destinationTag[0])
                    {
                        clickedCell.Image = selectedPiece.Image;
                        clickedCell.Tag = selectedTag;
                        selectedPiece.Image = null;
                        selectedPiece.Tag = null;
                    }
                    else
                    {
                        // Se for da mesma cor, o movimento não é efetuado (você pode customizar esse comportamento)
                        // MessageBox.Show("Movimento inválido! Não pode capturar peça da mesma cor.");
                    }
                }
                else
                {
                    // Move a peça para uma célula vazia
                    clickedCell.Image = selectedPiece.Image;
                    clickedCell.Tag = selectedPiece.Tag;
                    selectedPiece.Image = null;
                    selectedPiece.Tag = null;
                }

                // Remove o destaque da peça selecionada e limpa a seleção
                selectedPiece.BorderStyle = BorderStyle.None;
                selectedPiece = null;

                // Alterna o turno
                isWhiteTurn = !isWhiteTurn;
            }
        }

        private void DetermineStartingPlayer()
        {
            // Supondo que o jogador com as peças brancas sempre começa
            string startingPlayer = "Brancas";
            MessageBox.Show($"{startingPlayer} começam o jogo!");
        }
    }
}
