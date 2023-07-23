using ChessChallenge.API;
using System;

namespace ChessChallenge.Example
{

    public class EvilBot : IChessBot
    {
        public Move Think(Board board, Timer timer)
        {
            Move[] moves = board.GetLegalMoves();
            return moves[0];
        }
    }
}