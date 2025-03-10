using System;
using System.Windows.Forms;

namespace ChessWinForms
{
    public class King
    {
        public bool IsValidMove(PictureBox from, PictureBox to)
        {
            string fromTag = from.Tag as string;
            string toTag = to.Tag as string;

            if (fromTag == null || !fromTag.EndsWith("king"))
                return false;

            int fromRow = int.Parse(from.Name.Split('_')[1]);
            int fromCol = int.Parse(from.Name.Split('_')[2]);
            int toRow = int.Parse(to.Name.Split('_')[1]);
            int toCol = int.Parse(to.Name.Split('_')[2]);

            int rowDiff = Math.Abs(toRow - fromRow);
            int colDiff = Math.Abs(toCol - fromCol);

            // Movimento do rei: uma casa em qualquer direção
            if (rowDiff <= 1 && colDiff <= 1)
            {
                // Verifica se a célula destino está vazia ou contém uma peça de cor oposta
                if (toTag == null || toTag[0] != fromTag[0])
                    return true;
            }

            return false;
        }
    }
}