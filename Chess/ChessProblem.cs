namespace Chess
{
	public class ChessProblem
	{
		private static Board board;
		public static ChessStatus ChessStatus;

		public static void LoadFrom(string[] lines)
		{
			board = new BoardParser().ParseBoard(lines);
		}

		// Определяет мат, шах или пат белым.
		public static void CalculateChessStatus()
		{
			var isCheck = IsCheckForWhite();
			var hasMoves = false;
			foreach (var locFrom in board.GetPiecesLocations(PieceColor.White))
			{
				foreach (var locTo in board.GetPiece(locFrom).GetMoves(locFrom, board))
				{
					var currentPiece = board.GetPiece(locTo);

					MakeStep(locTo, locFrom);

				    hasMoves = !IsCheckForWhite();
                    MakeStep(locFrom, locTo, currentPiece);
				}
			}
			if (isCheck)
				if (hasMoves)
					ChessStatus = ChessStatus.Check;
				else ChessStatus = ChessStatus.Mate;
			else if (hasMoves) ChessStatus = ChessStatus.Ok;
			else ChessStatus = ChessStatus.Stalemate;
		}

	    private static void MakeStep(Location locTo, Location locFrom, Piece pieceOnLocFrom = null)
	    {
	        board.Set(locTo, board.GetPiece(locFrom));
	        board.Set(locFrom, pieceOnLocFrom);
	    }

	    // check — это шах
		private static bool IsCheckForWhite()
		{
			foreach (var loc in board.GetPiecesLocations(PieceColor.Black))
			{
				var piece = board.GetPiece(loc);
				var moves = piece.GetMoves(loc, board);
				foreach (var destination in moves)
					if (board.GetPiece(destination).Is(PieceColor.White, PieceType.King))
						return true;
			}
			return false;
		}
	}
}