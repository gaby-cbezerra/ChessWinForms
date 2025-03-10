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
            CheckGameState();
        }

        private void DetermineStartingPlayer()
        {
            // Supondo que o jogador com as peças brancas sempre começa
            string startingPlayer = "Brancas";
            MessageBox.Show($"{startingPlayer} começam o jogo!");
        }

        private bool IsKingInCheck(string kingTag, PictureBox[,] board)
        {
            // Encontra a posição do rei
            PictureBox kingPosition = null;
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    if (board[row, col].Tag as string == kingTag)
                    {
                        kingPosition = board[row, col];
                        break;
                    }
                }
            }

            if (kingPosition == null)
                return false; // Rei não encontrado (erro crítico)

            // Verifica se alguma peça adversária pode atacar o rei
            foreach (var piece in board)
            {
                if (piece.Image != null && piece.Tag != null)
                {
                    string pieceTag = piece.Tag as string;
                    if (pieceTag[0] != kingTag[0]) // Se for uma peça adversária
                    {
                        if (IsValidMove(piece, kingPosition))
                        {
                            return true; // O rei está sob ataque
                        }
                    }
                }
            }

            return false;
        }

        private bool IsCheckmate(string kingTag, PictureBox[,] board)
        {
            if (!IsKingInCheck(kingTag, board))
                return false; // Se o rei não está em xeque, não pode ser xeque-mate

            // Testa todos os movimentos possíveis do rei
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    if (board[row, col].Tag as string == kingTag)
                    {
                        PictureBox king = board[row, col];

                        // Verifica movimentos possíveis do rei
                        int[] dRow = { -1, -1, -1, 0, 0, 1, 1, 1 };
                        int[] dCol = { -1, 0, 1, -1, 1, -1, 0, 1 };

                        for (int i = 0; i < 8; i++)
                        {
                            int newRow = row + dRow[i];
                            int newCol = col + dCol[i];
                            if (newRow >= 0 && newRow < 8 && newCol >= 0 && newCol < 8)
                            {
                                PictureBox target = board[newRow, newCol];
                                string originalTag = target.Tag as string;
                                Image originalImage = target.Image;

                                // Simula o movimento
                                target.Tag = king.Tag;
                                target.Image = king.Image;
                                king.Tag = null;
                                king.Image = null;

                                bool stillInCheck = IsKingInCheck(kingTag, board);

                                // Reverte a simulação
                                king.Tag = kingTag;
                                king.Image = target.Image;
                                target.Tag = originalTag;
                                target.Image = originalImage;

                                if (!stillInCheck)
                                {
                                    return false; // Se o rei puder se mover para um local seguro, não é xeque-mate
                                }
                            }
                        }
                    }
                }
            }

            return true; // Se nenhum movimento do rei o salva, é xeque-mate
        }

        private void CheckGameState()
        {
            string kingWhite = "w_king";
            string kingBlack = "b_king";

            if (IsCheckmate(kingWhite, board))
            {
                MessageBox.Show("Xeque-mate! Pretas venceram!");
                Application.Exit();
            }
            else if (IsCheckmate(kingBlack, board))
            {
                MessageBox.Show("Xeque-mate! Brancas venceram!");
                Application.Exit();
            }
            else if (IsKingInCheck(kingWhite, board))
            {
                MessageBox.Show("Rei branco está em xeque!");
            }
            else if (IsKingInCheck(kingBlack, board))
            {
                MessageBox.Show("Rei preto está em xeque!");
            }
        }

        private bool IsValidMove(PictureBox from, PictureBox to)
        {
            string fromTag = from.Tag as string;
            string toTag = to.Tag as string;

            if (fromTag == null || toTag == null)
                return false;

            if (fromTag.EndsWith("pawn"))
                return pawn.IsValidMove(from, to);
            if (fromTag.EndsWith("knight"))
                return knight.IsValidMove(from, to);
            if (fromTag.EndsWith("bishop"))
                return bishop.IsValidMove(from, to, board);
            if (fromTag.EndsWith("rook"))
                return rook.IsValidMove(from, to, board);
            if (fromTag.EndsWith("queen"))
                return queen.IsValidMove(from, to, board);
            if (fromTag.EndsWith("king"))
                return king.IsValidMove(from, to);

            return false;
        }
    }
}
