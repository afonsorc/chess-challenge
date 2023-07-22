using ChessChallenge.API;
using System;
using System.Linq;
using System.Numerics;
using System.Collections.Generic;
using System.IO;

public class MyBot : IChessBot{

    float[] pieceValue = { 1.0f, 3.0f, 3.5f, 5.0f, 9.0f, 100.0f };

    public Move Think(Board board, Timer timer)
    {
        float bestEval = 0.0f;
        if (board.IsWhiteToMove)
        {
            bestEval = -1000.0f;
        }
        else
        {
            bestEval = 1000.0f;
        }
        Console.WriteLine((board.PlyCount + 1) / 2);
        Move[] moves = board.GetLegalMoves();
        Move bestMove = Move.NullMove;
        foreach (Move m in moves){
            //Console.WriteLine(m);
            board.MakeMove(m);
            float newEval = GetPositionEvaluation(board);

            if (board.IsInCheckmate())
            {
                bestMove = m;
                break;
            }

            if (board.IsDraw())
            {
                newEval = 0.0f;
            }
            if (!board.IsWhiteToMove && bestEval < newEval){
                bestMove = m;
                bestEval = newEval;
            }
            else if (board.IsWhiteToMove && bestEval > newEval)
            {
                bestMove = m;
                bestEval = newEval;
            }
            if(board.IsInCheck()){
                bestMove = m;
            }
            //Console.WriteLine(m + " " + newEval.ToString("F"));
            board.UndoMove(m);
        }
        Console.WriteLine(bestMove);

        return bestMove;
    }


    public float GetPositionEvaluation(Board board){

        float whiteEval = 0.0f;
        float blackEval = 0.0f;
        for(int pl = 0; pl < 6; pl++){
            whiteEval += pieceValue[pl] * board.GetAllPieceLists()[pl].Count;
            //Console.WriteLine(pieceValue[pl] + " " + board.GetAllPieceLists()[pl].Count);
            blackEval += pieceValue[pl] * board.GetAllPieceLists()[pl + 6].Count;
            //Console.WriteLine(pieceValue[pl] + " " + board.GetAllPieceLists()[pl + 6].Count);
        }
        return whiteEval - blackEval;
    }
}