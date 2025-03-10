using System;
using System.Windows.Forms;

namespace ChessWinForms
{
    public class Bishop
    {
        public bool IsValidMove(PictureBox from, PictureBox to, PictureBox[,] board)
        {
            string fromTag = from.Tag as string;
            string toTag = to.Tag as string;

            if (fromTag == null || !fromTag.EndsWith("bishop"))
                return false;

            int fromRow = int.Parse(from.Name.Split('_')[1]);
            int fromCol = int.Parse(from.Name.Split('_')[2]);
            int toRow = int.Parse(to.Name.Split('_')[1]);
            int toCol = int.Parse(to.Name.Split('_')[2]);

            int rowDiff = Math.Abs(toRow - fromRow);
            int colDiff = Math.Abs(toCol - fromCol);

            // Movimento diagonal: a diferença entre as linhas e colunas deve ser igual
            if (rowDiff == colDiff)
            {
                int rowStep = (toRow > fromRow) ? 1 : -1;
                int colStep = (toCol > fromCol) ? 1 : -1;

                // Verifica se há peças no caminho
                for (int i = 1; i < rowDiff; i++)
                {
                    if (board[fromRow + i * rowStep, fromCol + i * colStep].Image != null)
                        return false;
                }

                // Verifica se a célula destino está vazia ou contém uma peça de cor oposta
                if (toTag == null || toTag[0] != fromTag[0])
                    return true;
            }

            return false;
        }
    }
}