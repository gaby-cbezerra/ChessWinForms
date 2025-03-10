using System;
using System.Windows.Forms;

namespace ChessWinForms
{
    public class Pawn
    {
        public bool IsValidMove(PictureBox from, PictureBox to)
        {
            string fromTag = from.Tag as string;
            string toTag = to.Tag as string;

            if (fromTag == null || !fromTag.EndsWith("pawn"))
                return false;

            int fromRow = int.Parse(from.Name.Split('_')[1]);
            int fromCol = int.Parse(from.Name.Split('_')[2]);
            int toRow = int.Parse(to.Name.Split('_')[1]);
            int toCol = int.Parse(to.Name.Split('_')[2]);

            bool isWhite = fromTag.StartsWith("w");
            int direction = isWhite ? -1 : 1;

            // Movimento simples para frente
            if (toCol == fromCol && toRow == fromRow + direction && toTag == null)
                return true;

            // Movimento inicial de dois passos
            if ((isWhite && fromRow == 6 || !isWhite && fromRow == 1) && toCol == fromCol && toRow == fromRow + 2 * direction && toTag == null)
                return true;

            // Captura diagonal
            if (Math.Abs(toCol - fromCol) == 1 && toRow == fromRow + direction && toTag != null && toTag[0] != fromTag[0])
                return true;

            return false;
        }
    }
}