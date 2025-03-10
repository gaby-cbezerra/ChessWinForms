using System;
using System.Windows.Forms;

namespace ChessWinForms
{
    public class Rook
    {
        public bool IsValidMove(PictureBox from, PictureBox to, PictureBox[,] board)
        {
            string fromTag = from.Tag as string;
            string toTag = to.Tag as string;

            if (fromTag == null || !fromTag.EndsWith("rook"))
                return false;

            int fromRow = int.Parse(from.Name.Split('_')[1]);
            int fromCol = int.Parse(from.Name.Split('_')[2]);
            int toRow = int.Parse(to.Name.Split('_')[1]);
            int toCol = int.Parse(to.Name.Split('_')[2]);

            // Movimento horizontal ou vertical
            if (fromRow == toRow || fromCol == toCol)
            {
                int rowStep = (toRow > fromRow) ? 1 : (toRow < fromRow) ? -1 : 0;
                int colStep = (toCol > fromCol) ? 1 : (toCol < fromCol) ? -1 : 0;

                // Verifica se há peças no caminho
                int currentRow = fromRow + rowStep;
                int currentCol = fromCol + colStep;
                while (currentRow != toRow || currentCol != toCol)
                {
                    if (board[currentRow, currentCol].Image != null)
                        return false;

                    currentRow += rowStep;
                    currentCol += colStep;
                }

                // Verifica se a célula destino está vazia ou contém uma peça de cor oposta
                if (toTag == null || toTag[0] != fromTag[0])
                    return true;
            }

            return false;
        }
    }
}